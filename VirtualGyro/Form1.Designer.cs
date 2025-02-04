namespace VirtualGyro
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button_EnableServer = new Button();
            button_EnableMouseHook = new Button();
            textBox_IPAddress = new TextBox();
            textBox_Port = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // button_EnableServer
            // 
            button_EnableServer.Location = new Point(12, 41);
            button_EnableServer.Name = "button_EnableServer";
            button_EnableServer.Size = new Size(95, 23);
            button_EnableServer.TabIndex = 3;
            button_EnableServer.Text = "启用服务器";
            button_EnableServer.UseVisualStyleBackColor = true;
            button_EnableServer.Click += button_EnableServer_Click;
            // 
            // button_EnableMouseHook
            // 
            button_EnableMouseHook.Location = new Point(113, 41);
            button_EnableMouseHook.Name = "button_EnableMouseHook";
            button_EnableMouseHook.Size = new Size(95, 23);
            button_EnableMouseHook.TabIndex = 4;
            button_EnableMouseHook.Text = "启用鼠标钩子";
            button_EnableMouseHook.UseVisualStyleBackColor = true;
            button_EnableMouseHook.Click += button_EnableMouseHook_Click;
            // 
            // textBox_IPAddress
            // 
            textBox_IPAddress.Location = new Point(12, 12);
            textBox_IPAddress.Name = "textBox_IPAddress";
            textBox_IPAddress.Size = new Size(95, 23);
            textBox_IPAddress.TabIndex = 5;
            textBox_IPAddress.Text = "127.0.0.1";
            // 
            // textBox_Port
            // 
            textBox_Port.Location = new Point(113, 12);
            textBox_Port.Name = "textBox_Port";
            textBox_Port.Size = new Size(95, 23);
            textBox_Port.TabIndex = 6;
            textBox_Port.Text = "26760";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(111, 67);
            label1.Name = "label1";
            label1.Size = new Size(97, 17);
            label1.TabIndex = 7;
            label1.Text = "F3 禁用鼠标钩子";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(220, 93);
            Controls.Add(label1);
            Controls.Add(textBox_Port);
            Controls.Add(textBox_IPAddress);
            Controls.Add(button_EnableMouseHook);
            Controls.Add(button_EnableServer);
            Name = "Form1";
            Text = "Virtual Gyro";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button button_EnableServer;
        private Button button_EnableMouseHook;
        private TextBox textBox_IPAddress;
        private TextBox textBox_Port;
        private Label label1;
    }
}
