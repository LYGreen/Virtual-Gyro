using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGyro.MouseMotion
{
    // 【未完成的类】
    internal class MouseMotion
    {
        // 我数学没学好原谅我！！！

        // 右手坐标系：
        // x 向右，z 向电脑屏幕里面，y 向上

        // 当前鼠标位置
        private Point currentMousePosition;

        // 敏感度
        private float _sensitivity;
        public float Sensitivity
        {
            get => _sensitivity;
            set => _sensitivity = value;
        }

        // 时间间隔
        long deltaTimestampTicks;

        // 时间戳
        long currentTimestampTicks = 0;

        // 球坐标系：
        // θ为与 xoz 平面( y 轴向上)的夹角
        // ϕ 为与 x 轴夹角
        // 对象坐标系，或者是局部坐标系
        Matrix3 objectCoordinateSystem = Matrix3.Identity;

        // 对象角度(球坐标系 (r, θ, ϕ) 中的 θ, ϕ)
        SphericalCoordinates angle = new SphericalCoordinates();

        // 对象的角速度(球坐标系 (r, θ, ϕ) 中的 θ, ϕ 变化速度)
        SphericalCoordinates angleVelocity = SphericalCoordinates.Zero;

        // 球坐标
        public struct SphericalCoordinates
        {
            public SphericalCoordinates() { }
            public SphericalCoordinates(float r, float theta, float phi) { }

            public static readonly SphericalCoordinates Identity = new SphericalCoordinates(1, 0, 0);
            public static readonly SphericalCoordinates Zero = new SphericalCoordinates(0, 0, 0);

            //public Vector3 PositionInCartesianCoordinateSystem
            //{
            //    get => new Vector3(R * MathF.Sin())
            //}

            public float R;
            public float Theta;
            public float Phi;
        }

        //public void NextMousePosition(Point point)
        //{
        //    angleVelocity.X = (point.X - currentMousePosition.X) * _sensitivity;
        //    angleVelocity.Y = (point.Y - currentMousePosition.Y) * _sensitivity;

        //    deltaTimestampTicks = DateTime.UtcNow.Ticks - currentTimestampTicks;
        //    double deltaTime = deltaTimestampTicks / 1000000.0;
        //    currentTimestampTicks = DateTime.UtcNow.Ticks;

            

        //    currentMousePosition = point;
        //}

        public SphericalCoordinates GetAngle()
        {
            return angle;
        }

        public long GetDeltaTimeTicks()
        {
            return deltaTimestampTicks;
        }

        public double GetDeltaTime()
        {
            return deltaTimestampTicks / 1000000.0;
        }
    }
}
