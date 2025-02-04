using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using System.Runtime.InteropServices;
using VirtualGyro.Cemuhook;
using VirtualGyro.Hook;
using VirtualGyro.OpenGL;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VirtualGyro
{
    public partial class Form1 : Form
    {
        CharacterBody body = new CharacterBody();
        OpenGLControl openGLControl = new OpenGLControl();
        CemuhookServer cemuhookServer = new CemuhookServer();

        Vector2 deltaAngle = new Vector2();
        Point fixedMousePos = new Point();
        float sensitivity = 0.8f;
        UInt64 timestamp = 0;
        public struct POINT
        {
            public int X;
            public int Y;
        }
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        public Form1()
        {
            InitializeComponent();
            //InitializeOpenGLControl();
        }

        //private void InitializeOpenGLControl()
        //{
        //    openGLControl.Dock = DockStyle.Fill;
        //    openGLControl.Load += openGLControl_Load; ;
        //    openGLControl.Paint += openGLControl_Paint;
        //    openGLControl.Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;

        //    //panel1.Controls.Add(openGLControl);
        //}

        //private void openGLControl_Load(object? sender, EventArgs e)
        //{
        //    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        //    timer.Interval = 16;
        //    timer.Tick += (sender, e) =>
        //    {
        //        if (!openGLControl.IsRunning)
        //        {
        //            timer.Dispose();
        //            return;
        //        }
        //        openGLControl.NextFrame();

        //    };
        //    timer.Enabled = true;
        //}

        //private void openGLControl_Paint(object? sender, PaintEventArgs e)
        //{
        //    openGLControl.NextFrame();
        //}

        bool isServerEnabled = false;
        private void button_EnableServer_Click(object sender, EventArgs e)
        {
            if (isServerEnabled)
            {
                cemuhookServer.Stop();
                button_EnableServer.Text = "启用服务器";
                isServerEnabled = false;
                return;
            }
            isServerEnabled = true;
            button_EnableServer.Text = "禁用服务器";

            cemuhookServer.Start(textBox_IPAddress.Text, Convert.ToInt32(textBox_Port.Text));
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1;
            timer.Tick += (sender, e) =>
            {
                if (!cemuhookServer.IsRunning || !isServerEnabled)
                {
                    timer.Dispose();
                    return;
                }
                byte[] paddata = new byte[80];
                int curIdx = 0;

                paddata[curIdx++] = 0; // 插槽
                paddata[curIdx++] = 2; // 插槽状态：0 未连接, 1 待定(?), 2 已连接
                paddata[curIdx++] = 2; // Device model: 0 if not applicable, 1 if no or partial gyro 2 for full gyro. Value 3 exist but should not be used (go with VR, guys).
                paddata[curIdx++] = 2; // 连接类型：0 不适用, 1 USB, 2 蓝牙.

                // MAC 地址
                paddata[curIdx + 0] = 0x00;
                paddata[curIdx + 1] = 0x01;
                paddata[curIdx + 2] = 0x02;
                paddata[curIdx + 3] = 0x03;
                paddata[curIdx + 4] = 0x04;
                paddata[curIdx + 5] = 0x05;
                curIdx += 6;

                // 电池状态
                Array.Copy(BitConverter.GetBytes(0x05), 0, paddata, curIdx, 1);
                curIdx += 1;

                // 时间戳
                curIdx += 48 - 11;
                double deltaTime = (Convert.ToUInt64(DateTime.UtcNow.Ticks) - timestamp) / 1000000.0;
                timestamp = Convert.ToUInt64(DateTime.UtcNow.Ticks);
                Array.Copy(BitConverter.GetBytes(timestamp), 0, paddata, curIdx, 8);
                curIdx += 8;

                // Accel
                //float accelX = 0.01f;
                //Array.Copy(BitConverter.GetBytes(-deltaAngle.Y), 0, paddata, curIdx, 4);
                //curIdx += 4;
                //float accelY = 0.0f;
                //Array.Copy(BitConverter.GetBytes(deltaAngle.X), 0, paddata, curIdx, 4);
                //curIdx += 4;
                //float accelZ = 0.0f;
                //Array.Copy(BitConverter.GetBytes(accelZ), 0, paddata, curIdx, 4);
                //curIdx += 4;

                // pitch yaw roll
                curIdx = 68;
                Array.Copy(BitConverter.GetBytes(deltaAngle.Y), 0, paddata, curIdx, 4);
                curIdx += 4;
                Array.Copy(BitConverter.GetBytes(-deltaAngle.X), 0, paddata, curIdx, 4);
                curIdx += 4;
                float roll = 0.0f;
                Array.Copy(BitConverter.GetBytes(roll), 0, paddata, curIdx, 4);
                curIdx += 4;

                byte[] packet = Protocol.BuildPacket(paddata, Protocol.DataType.PadData, cemuhookServer.ServerId);
                cemuhookServer.SendPadData(paddata);

                //Console.WriteLine(string.Join(" ", packet.Select(b => b.ToString("X2"))));
            };
            timer.Enabled = true;
        }

        bool isMouseHookEnabled = false;
        private void button_EnableMouseHook_Click(object sender, EventArgs e)
        {
            if (isMouseHookEnabled)
            {
                isMouseHookEnabled = false;
                button_EnableMouseHook.Text = "启用鼠标钩子";
                MouseHook.Stop();
                KeyboardHook.Stop();
                return;
            }
            button_EnableMouseHook.Text = "禁用鼠标钩子";
            isMouseHookEnabled = true;

            fixedMousePos = Cursor.Position;
            MouseHook.Callback += UpdateAngle;
            KeyboardHook.Callback += ListenHotkey;
            MouseHook.Start();
            KeyboardHook.Start();
        }

        private IntPtr UpdateAngle(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if ((int)wParam == MouseHook.WM_MOUSEMOVE)
            {
                int x = Marshal.ReadInt32(lParam);
                int y = Marshal.ReadInt32(new IntPtr(lParam.ToInt64() + 4));

                deltaAngle.X = (x - fixedMousePos.X) * sensitivity;
                deltaAngle.Y = (y - fixedMousePos.Y) * sensitivity;
            }

            return IntPtr.Zero;
        }

        private IntPtr ListenHotkey(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int keyCode = Marshal.ReadInt32(lParam);
            if ((int)wParam == KeyboardHook.WM_KEYDOWN || (int)wParam == KeyboardHook.WM_SYSKEYDOWN)
            {
                isMouseHookEnabled = false;
                button_EnableMouseHook.Text = "启用鼠标钩子";
                MouseHook.Stop();
                KeyboardHook.Stop();
            }

            return IntPtr.Zero;
        }
    }
}
