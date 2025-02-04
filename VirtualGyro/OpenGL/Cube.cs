using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace VirtualGyro.OpenGL
{
    internal class Cube : CharacterBody
    {
        private Vector3 _size;
        public Vector3 Size
        {
            get => _size;
            set => _size = value;
        }

        public override void Ready()
        {
            base.Ready();
        }

        public override void Process(float deltaTime)
        {
            base.Process(deltaTime);
        }

        public override void PhysicsProcess(float deltaTime)
        {
            base.PhysicsProcess(deltaTime);
        }

        public override void Draw()
        {
            base.Draw();

            GL.Begin(PrimitiveType.Points);
            GL.Vertex3(Position);
            GL.End();


            //GL.Begin(PrimitiveType.Quads);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.End();

            //GL.Begin(PrimitiveType.Quads);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.End();

            //GL.Begin(PrimitiveType.Quads);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.End();

            //GL.Begin(PrimitiveType.Quads);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.End();

            //GL.Begin(PrimitiveType.Quads);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y + Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.End();

            //GL.Begin(PrimitiveType.Quads);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z - Size.Z / 2);
            //GL.Vertex3(Position.X + Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.Vertex3(Position.X - Size.X / 2f, Position.Y - Size.Y / 2f, Position.Z + Size.Z / 2);
            //GL.End();

        }
    }
}
