using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGyro.OpenGL
{
    internal abstract class OpenGLBase
    {
        public virtual void Ready() { }
        public virtual void Process(float deltaTime) { }
        public virtual void PhysicsProcess(float deltaTime) { }
        public virtual void Finish() { }
        public virtual void Draw() { }
    }
}
