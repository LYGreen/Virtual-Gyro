using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGyro.OpenGL
{
    internal class CharacterBody : Transform
    {
        private Vector3 _velocity;
        public Vector3 Velocity
        {
            get => _velocity;
            set => _velocity = value;
        }

        public override void PhysicsProcess(float deltaTime)
        {
            base.PhysicsProcess(deltaTime);
            Position += Velocity * deltaTime;
        }
    }
}
