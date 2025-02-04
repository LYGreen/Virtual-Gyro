using OpenTK.GLControl;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGyro.OpenGL
{
    internal class OpenGLControl : GLControl
    {
        private List<OpenGLBase> objects = new List<OpenGLBase>();
        private double timeStep = 1.0 / 60.0; // 时间步长，每秒 60 帧
        private double lastTime = 0.0; // 上一次物理帧刷新时间
        private Stopwatch stopwatch = new Stopwatch();
        private bool exit = false;
        public bool IsRunning
        {
            get => !exit;
        }
        public OpenGLControl(): base()
        {

        }

        public void Init()
        {
            stopwatch.Start();
            MakeCurrent();
            Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
        }

        public void NextFrame()
        {
            if (exit)
            {
                return;
            }

            MakeCurrent();

            double currentTime = stopwatch.Elapsed.TotalSeconds;
            double deltaTime = currentTime - lastTime;

            // 物理刷新率
            if (deltaTime >= timeStep)
            {
                PhysicsProcess(Convert.ToSingle(deltaTime));
                lastTime = currentTime;
            }

            // 刷新率
            Process(Convert.ToSingle(deltaTime));

            // 画出图像
            DrawGraphics();

            Thread.Sleep(16); // 1000 / 60 ≈ 16.66667
        }

        //public void Run()
        //{
        //    Loop();
        //}

        public void Exit()
        {
            exit = true;
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].Finish();
            }
            objects.Clear();
            stopwatch.Stop();
        }

        public void AddObject(OpenGLBase obj)
        {
            if (!objects.Contains(obj))
            {
                objects.Add(obj);
                obj.Ready();
            }
        }

        public void RemoveObject(OpenGLBase obj)
        {
            if (objects.Contains(obj))
            {
                objects.Remove(obj);
                obj.Finish();
            }
        }

        //private void Loop()
        //{
        //    stopwatch.Start();
        //    MakeCurrent();
        //    Profile = OpenTK.Windowing.Common.ContextProfile.Compatability;
        //    while (!exit)
        //    {
        //        double currentTime = stopwatch.Elapsed.TotalSeconds;
        //        double deltaTime = currentTime - lastTime;

        //        // 物理刷新率
        //        if (deltaTime >= timeStep)
        //        {
        //            PhysicsProcess(Convert.ToSingle(deltaTime));
        //            lastTime = currentTime;
        //        }

        //        // 刷新率
        //        Process(Convert.ToSingle(deltaTime));

        //        // 画出图像
        //        DrawGraphics();

        //        Thread.Sleep(16); // 1000 / 60 ≈ 16.66667
        //    }
        //    stopwatch.Stop();
        //}

        private void Process(float deltaTime)
        {
            foreach (OpenGLBase obj in objects)
            {
                obj.Process(deltaTime);
            }
        }

        private void PhysicsProcess(float deltaTime)
        {
            foreach (OpenGLBase obj in objects)
            {
                obj.PhysicsProcess(deltaTime);
            }
        }

        private void DrawGraphics()
        {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            foreach (OpenGLBase obj in objects)
            {
                obj.Draw();
            }
            SwapBuffers();
        }
    }
}
