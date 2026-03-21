using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeRxKEYs
{
    public class TrackedObject
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector3 AngularVelocity { get; set; }
        public Vector3 AngularAcceleration { get; set; }

        public Vector3 LastPosition { get; set; }
        public Quaternion LastRotation { get; set; }

        public Vector3 LastVelocity { get; set; }
        public Vector3 LastAngularVelocity { get; set; }
        public Vector3 LastAcceleration { get; set; }
        public Vector3 LastAngularAcceleration { get; set; }

        public TrackedObject()
        {
            Position = new Vector3();
            Rotation = new Quaternion();
            LastPosition = new Vector3();
            LastRotation = new Quaternion();

            Velocity = new Vector3();
            Acceleration = new Vector3();
            AngularVelocity = new Vector3();
            AngularAcceleration = new Vector3();
        }

        public TrackedObject(string name)
        {
            Name = name;
            Position = new Vector3();
            Rotation = new Quaternion();
            LastPosition = new Vector3();
            LastRotation = new Quaternion();

            Velocity = new Vector3();
            Acceleration = new Vector3();
            AngularVelocity = new Vector3();
            AngularAcceleration = new Vector3();
        }

        public TrackedObject(string name, Vector3 position, Quaternion rotation)
        {
            Name = name;
            Position = position;
            Rotation = rotation;
            LastPosition = new Vector3();
            LastRotation = new Quaternion();

            Velocity = new Vector3();
            Acceleration = new Vector3();
            AngularVelocity = new Vector3();
            AngularAcceleration = new Vector3();
        }

        public void Update(Vector3 pos, Quaternion rot, float deltaTime = 0)
        {
            LastVelocity = Velocity;
            LastAngularVelocity = AngularVelocity;
            LastAcceleration = Acceleration;
            LastAngularAcceleration = AngularAcceleration;
            LastPosition = Position;
            LastRotation = Rotation;

            Position = pos;
            Rotation = rot;

            if (deltaTime <= 0)
            {
                Velocity = LastVelocity;
                Acceleration = LastAcceleration;
                AngularVelocity = LastAngularVelocity;
                AngularAcceleration = LastAngularAcceleration;
                return;
            }

            Velocity = (Position - LastPosition) / deltaTime;
            Acceleration = (Velocity - LastVelocity) / deltaTime;

            Quaternion deltaRotation = Rotation * LastRotation.Inverse();
            deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 axis);
            Vector3 angularDisplacement = axis * angleInDegrees * MathHelper.DegToRad;

            AngularVelocity = angularDisplacement / deltaTime;
            AngularAcceleration = (AngularVelocity - LastAngularVelocity) / deltaTime;
        }
    }
}
