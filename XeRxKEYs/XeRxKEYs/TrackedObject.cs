using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeRxKEYs
{
    public class TrackedObject
    {
        public static TrackedObject hmdObj = null;

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

        private bool shakeTriggered = false;
        private bool stabTriggered = false;
        private bool swipeTriggered = false;
        private bool twistTriggered = false;
        private bool slashTriggered = false;

        private int shakeDirections;
        private float timeSinceLastDirectionChange;
        private int stabDirections;
        private float timeSinceLastStabChange;
        private int swipeDirections;
        private float timeSinceLastSwipeChange;
        private int twistDirections;
        private float timeSinceLastTwistChange;
        private float slashCooldown;

        //TODO: Feed these settings in from above, since TrackedObject data isn't stored to disk
        //Shake tweakable settings
        //This is when you shake the controller or your head physically up/down, it is position based
        private const float SHAKE_ACCELERATION_THRESHOLD = 5.0f;
        private const float SHAKE_DIRECTION_CHANGE_TIME = 0.3f;
        private const int SHAKE_REQUIRED_DIRECTIONS = 3;

        //Stab tweakable settings
        //This is when you move forward and backward, like a stabbing motion, it is position based
        private const float STAB_ACCELERATION_THRESHOLD = 8.0f;
        private const float STAB_DIRECTION_CHANGE_TIME = 0.25f;
        private const int STAB_REQUIRED_DIRECTIONS = 2;

        //Swipe tweakable settings
        //This is when you move a device left and right like swiping a card in a horizontal reader, it is position based
        private const float SWIPE_ACCELERATION_THRESHOLD = 7.0f;
        private const float SWIPE_DIRECTION_CHANGE_TIME = 0.3f;
        private const int SWIPE_REQUIRED_DIRECTIONS = 2;

        //Twist tweakable settings
        //This is when you twist your controller or head like turning a door handle (NOT position based)
        private const float TWIST_ANGULAR_VELOCITY_THRESHOLD = 180.0f;
        private const float TWIST_DIRECTION_CHANGE_TIME = 0.4f;
        private const int TWIST_REQUIRED_DIRECTIONS = 2;

        //Slash tweakable settings
        //This is when you rotate your controller or head back and forward like a "sword slash" (NOT position based)
        private const float SLASH_ANGULAR_VELOCITY_THRESHOLD = 450.0f;
        private const float SLASH_COOLDOWN_TIME = 0.5f;

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
            //TODO: Consider motion smoothing amount on the data updates??
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

            if (!shakeTriggered && TrackedObject.hmdObj != null) shakeTriggered = CheckShake(deltaTime, TrackedObject.hmdObj.Rotation);
            if (!stabTriggered && TrackedObject.hmdObj != null) stabTriggered = CheckStab(deltaTime, TrackedObject.hmdObj.Rotation);
            if (!swipeTriggered && TrackedObject.hmdObj != null) swipeTriggered = CheckSwipe(deltaTime, TrackedObject.hmdObj.Rotation);
            if (!twistTriggered) twistTriggered = CheckTwist(deltaTime);
            if (!slashTriggered) slashTriggered = CheckSlash(deltaTime);
        }

        public bool DidShake()
        {
            return shakeTriggered;
        }

        public bool DidStab()
        {
            return stabTriggered;
        }

        public bool DidSwipe()
        {
            return swipeTriggered;
        }

        public bool DidTwist()
        {
            return twistTriggered;
        }

        public bool DidSlash()
        {
            return slashTriggered;
        }

        public void ClearTriggers()
        {
            shakeTriggered = false;
            stabTriggered = false;
            swipeTriggered = false;
            twistTriggered = false;
            slashTriggered = false;

            shakeDirections = 0;
            timeSinceLastDirectionChange = 0.0f;
            stabDirections = 0;
            timeSinceLastStabChange = 0.0f;
            swipeDirections = 0;
            timeSinceLastSwipeChange = 0.0f;
            twistDirections = 0;
            timeSinceLastTwistChange = 0.0f;
            slashCooldown = 0.0f;
        }

        private bool CheckShake(float deltaTime, Quaternion referenceRotation)
        {
            float verticalAccel = Acceleration.Y;
            float lastVerticalAccel = LastAcceleration.Y;

            timeSinceLastDirectionChange += deltaTime;

            if (Math.Abs(verticalAccel) > SHAKE_ACCELERATION_THRESHOLD)
            {
                if (Math.Sign(verticalAccel) != Math.Sign(lastVerticalAccel) && timeSinceLastDirectionChange < SHAKE_DIRECTION_CHANGE_TIME)
                {
                    shakeDirections++;
                    timeSinceLastDirectionChange = 0;
                }
            }

            if (timeSinceLastDirectionChange > SHAKE_DIRECTION_CHANGE_TIME)
            {
                shakeDirections = 0;
            }

            if (shakeDirections >= SHAKE_REQUIRED_DIRECTIONS)
            {
                shakeDirections = 0;
                return true;
            }

            return false;
        }

        private bool CheckStab(float deltaTime, Quaternion referenceRotation)
        {
            Quaternion inverseReference = referenceRotation.Inverse();
            Vector3 localAcceleration = inverseReference * Acceleration;
            Vector3 localLastAcceleration = inverseReference * LastAcceleration;

            float forwardAccel = localAcceleration.Z;

            timeSinceLastStabChange += deltaTime;

            if (Math.Abs(forwardAccel) > STAB_ACCELERATION_THRESHOLD)
            {
                if (Math.Sign(forwardAccel) != Math.Sign(localLastAcceleration.Z) && timeSinceLastStabChange < STAB_DIRECTION_CHANGE_TIME)
                {
                    stabDirections++;
                    timeSinceLastStabChange = 0;
                }
            }

            if (timeSinceLastStabChange > STAB_DIRECTION_CHANGE_TIME)
            {
                stabDirections = 0;
            }

            if (stabDirections >= STAB_REQUIRED_DIRECTIONS)
            {
                stabDirections = 0;
                return true;
            }

            return false;
        }

        private bool CheckSwipe(float deltaTime, Quaternion referenceRotation)
        {
            Quaternion inverseReference = referenceRotation.Inverse();
            Vector3 localAcceleration = inverseReference * Acceleration;
            Vector3 localLastAcceleration = inverseReference * LastAcceleration;

            float horizontalAccel = localAcceleration.X;

            timeSinceLastSwipeChange += deltaTime;

            if (Math.Abs(horizontalAccel) > SWIPE_ACCELERATION_THRESHOLD)
            {
                if (Math.Sign(horizontalAccel) != Math.Sign(localLastAcceleration.X) && timeSinceLastSwipeChange < SWIPE_DIRECTION_CHANGE_TIME)
                {
                    swipeDirections++;
                    timeSinceLastSwipeChange = 0;
                }
            }

            if (timeSinceLastSwipeChange > SWIPE_DIRECTION_CHANGE_TIME)
            {
                swipeDirections = 0;
            }

            if (swipeDirections >= SWIPE_REQUIRED_DIRECTIONS)
            {
                swipeDirections = 0;
                return true;
            }

            return false;
        }

        private bool CheckTwist(float deltaTime)
        {
            Quaternion inverseOwnRotation = this.Rotation.Inverse();
            Vector3 localAngularVelocity = inverseOwnRotation * this.AngularVelocity;

            Quaternion inverseLastOwnRotation = this.LastRotation.Inverse();
            Vector3 localLastAngularVelocity = inverseLastOwnRotation * this.LastAngularVelocity;

            float twistVelocity = localAngularVelocity.Z;

            timeSinceLastTwistChange += deltaTime;

            if (Math.Abs(twistVelocity) > TWIST_ANGULAR_VELOCITY_THRESHOLD)
            {
                if (Math.Sign(twistVelocity) != Math.Sign(localLastAngularVelocity.Z) && timeSinceLastTwistChange < TWIST_DIRECTION_CHANGE_TIME)
                {
                    twistDirections++;
                    timeSinceLastTwistChange = 0;
                }
            }

            if (timeSinceLastTwistChange > TWIST_DIRECTION_CHANGE_TIME)
            {
                twistDirections = 0;
            }

            if (twistDirections >= TWIST_REQUIRED_DIRECTIONS)
            {
                twistDirections = 0;
                return true;
            }

            return false;
        }

        private bool CheckSlash(float deltaTime)
        {
            if (slashCooldown > 0)
            {
                slashCooldown -= deltaTime;
            }

            Quaternion inverseOwnRotation = this.Rotation.Inverse();
            Vector3 localAngularVelocity = inverseOwnRotation * this.AngularVelocity;

            float slashVelocity = localAngularVelocity.X;

            if (Math.Abs(slashVelocity) > SLASH_ANGULAR_VELOCITY_THRESHOLD && slashCooldown <= 0)
            {
                slashCooldown = SLASH_COOLDOWN_TIME;
                return true;
            }

            return false;
        }
    }
}
