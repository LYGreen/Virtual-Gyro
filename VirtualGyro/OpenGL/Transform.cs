using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGyro.OpenGL
{
    internal class Transform : OpenGLBase
    {
        private Vector3 _position;
        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        private Vector3 _rotation;
        public Vector3 Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        private Vector3 _scale;
        public Vector3 Scale
        {
            get => _scale;
            set => _scale = value;
        }
    }
}
