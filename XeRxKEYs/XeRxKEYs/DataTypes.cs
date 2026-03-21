using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using WinApi;

namespace XeRxKEYs
{
    public static class MathHelper
    {
        public const float DegToRad = (float)(Math.PI / 180.0);
        public const float RadToDeg = (float)(180.0 / Math.PI);

        public static float DegreesToRadians(float degrees)
        {
            return degrees * DegToRad;
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * RadToDeg;
        }

        public static T Clamp<T>(T value, T min, T max) where T : System.IComparable<T>
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }
    }

    public class EulerAngle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public bool IsRadians { get; set; }

        public EulerAngle(bool _radians = true)
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;

            IsRadians = _radians;
        }

        public EulerAngle(float x, float y, float z, bool _radians = true)
        {
            X = x;
            Y = y;
            Z = z;

            IsRadians = _radians;
        }

        public float XDegrees()
        {
            if (IsRadians) return MathHelper.RadiansToDegrees(X);

            return X;
        }

        public float YDegrees()
        {
            if (IsRadians) return MathHelper.RadiansToDegrees(Y);

            return Y;
        }

        public float ZDegrees()
        {
            if (IsRadians) return MathHelper.RadiansToDegrees(Z);

            return Z;
        }

        public float XRad()
        {
            if (!IsRadians) return MathHelper.DegreesToRadians(X);

            return X;
        }

        public float YRad()
        {
            if (!IsRadians) return MathHelper.DegreesToRadians(Y);

            return Y;
        }

        public float ZRad()
        {
            if (!IsRadians) return MathHelper.DegreesToRadians(Z);

            return Z;
        }

        public EulerAngle ToRadians()
        {
            if (IsRadians) return this;

            return new EulerAngle(XRad(), YRad(), ZRad());
        }

        public EulerAngle ToDegrees()
        {
            if (!IsRadians) return this;

            return new EulerAngle(XDegrees(), YDegrees(), ZDegrees(), false);
        }

        public Quaternion ToQuaternion()
        {
            Quaternion q = new Quaternion();

            double cy = Math.Cos(ZRad() * 0.5);
            double sy = Math.Sin(ZRad() * 0.5);
            double cp = Math.Cos(YRad() * 0.5);
            double sp = Math.Sin(YRad() * 0.5);
            double cr = Math.Cos(XRad() * 0.5);
            double sr = Math.Sin(XRad() * 0.5);

            q.W = (float)(cr * cp * cy + sr * sp * sy);
            q.X = (float)(sr * cp * cy - cr * sp * sy);
            q.Y = (float)(cr * sp * cy + sr * cp * sy);
            q.Z = (float)(cr * cp * sy - sr * sp * cy);

            return q;
        }

        public Vector3 GetForward()
        {
            Vector3 defaultForward = new Vector3(0, 0, 1);

            return ToQuaternion() * defaultForward;
        }

        public Vector3 GetUp()
        {
            return ToQuaternion() * new Vector3(0, 1, 0);
        }

        public Vector3 GetRight()
        {
            return ToQuaternion() * new Vector3(1, 0, 0);
        }

        public static EulerAngle Difference(EulerAngle startAngle, EulerAngle endAngle)
        {
            Quaternion startQuat = startAngle.ToQuaternion();
            Quaternion endQuat = endAngle.ToQuaternion();
            Quaternion deltaQuat = Quaternion.Difference(startQuat, endQuat);
            return deltaQuat.ToEuler();
        }

        public static EulerAngle operator -(EulerAngle a, EulerAngle b)
        {
            return new EulerAngle(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static EulerAngle operator +(EulerAngle a, EulerAngle b)
        {
            return new EulerAngle(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static EulerAngle operator /(EulerAngle a, float d)
        {
            if (d == 0)
            {
                return new EulerAngle(0, 0, 0);
            }
            return new EulerAngle(a.X / d, a.Y / d, a.Z / d);
        }

        public static EulerAngle operator *(EulerAngle a, float d)
        {
            if (d == 0)
            {
                return new EulerAngle(0, 0, 0);
            }
            return new EulerAngle(a.X * d, a.Y * d, a.Z * d);
        }
    }

    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        public Vector3 Normalized()
        {
            Vector3 ans = new Vector3(X, Y, Z);
            float magnitude = Magnitude();

            if (magnitude > 0)
            {
                ans.X /= magnitude;
                ans.Y /= magnitude;
                ans.Z /= magnitude;
            }

            return ans;
        }

        public static Vector3 Difference(Vector3 a, Vector3 b)
        {
            return a - b;
        }

        public static float Distance(Vector3 a, Vector3 b)
        {
            float dX = b.X - a.X;
            float dY = b.Y - a.Y;
            float dZ = b.Z - a.Z;

            return (float)Math.Sqrt((dX * dX) + (dY * dY) + (dZ * dZ));
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            if (d == 0)
            {
                return new Vector3(0, 0, 0);
            }
            return new Vector3(a.X / d, a.Y / d, a.Z / d);
        }

        public static Vector3 operator *(Vector3 a, float d)
        {
            if (d == 0)
            {
                return new Vector3(0, 0, 0);
            }
            return new Vector3(a.X * d, a.Y * d, a.Z * d);
        }
    }

    public class Quaternion
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public Quaternion()
        {
            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;
            W = 1.0f;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public EulerAngle ToEuler()
        {
            EulerAngle angles = new EulerAngle();

            double sinr_cosp = 2 * (W * X + Y * Z);
            double cosr_cosp = 1 - 2 * (X * X + Y * Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            double sinp = 2 * (W * Y - Z * X);
            if (Math.Abs(sinp) >= 1)
                angles.Y = (float)(Math.Sign(sinp) * Math.PI / 2);
            else
                angles.Y = (float)Math.Asin(sinp);

            double siny_cosp = 2 * (W * Z + X * Y);
            double cosy_cosp = 1 - 2 * (Y * Y + Z * Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);
            return angles;
        }

        public Vector3 GetForward()
        {
            Vector3 defaultForward = new Vector3(0, 0, 1);

            return this * defaultForward;
        }

        public Vector3 GetUp()
        {
            return this * new Vector3(0, 1, 0);
        }

        public Vector3 GetRight()
        {
            return this * new Vector3(1, 0, 0);
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        public Quaternion Normalized()
        {
            Quaternion ans = new Quaternion(X, Y, Z, W);

            float magnitude = Magnitude();
            if (magnitude > 0.0f)
            {
                ans.X /= magnitude;
                ans.Y /= magnitude;
                ans.Z /= magnitude;
                ans.W /= magnitude;
            }

            return ans;
        }

        public Quaternion Inverse()
        {
            return new Quaternion(-X, -Y, -Z, W);
        }

        public void ToAngleAxis(out float angle, out Vector3 axis)
        {
            float clampedW = MathHelper.Clamp(W, -1.0f, 1.0f);
            angle = 2.0f * (float)Math.Acos(clampedW);
            float scale = (float)Math.Sqrt(1.0f - clampedW * clampedW);
            if (scale < 0.0001f)
            {
                axis = new Vector3(0, 1, 0);
            }
            else
            {
                axis = new Vector3(X / scale, Y / scale, Z / scale);
            }

            angle *= (180.0f / (float)Math.PI);
        }

        public static Quaternion Difference(Quaternion a, Quaternion b)
        {
            a.Normalized();
            b.Normalized();

            return b * a.Inverse();
        }

        public static Vector3 operator *(Quaternion rotation, Vector3 point)
        {
            Vector3 result = new Vector3();

            float num1 = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num1;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.Y * num3;
            float num9 = rotation.Z * num1;
            float num10 = rotation.W * num1;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            result.X = (1f - (num5 + num6)) * point.X + (num7 - num12) * point.Y + (num9 + num11) * point.Z;
            result.Y = (num7 + num12) * point.X + (1f - (num4 + num6)) * point.Y + (num8 - num10) * point.Z;
            result.Z = (num9 - num11) * point.X + (num8 + num10) * point.Y + (1f - (num4 + num5)) * point.Z;
            return result;
        }

        public static Quaternion operator *(Quaternion q1, Quaternion q2)
        {
            float newX = q1.W * q2.X + q1.X * q2.W + q1.Y * q2.Z - q1.Z * q2.Y;
            float newY = q1.W * q2.Y - q1.X * q2.Z + q1.Y * q2.W + q1.Z * q2.X;
            float newZ = q1.W * q2.Z + q1.X * q2.Y - q1.Y * q2.X + q1.Z * q2.W;
            float newW = q1.W * q2.W - q1.X * q2.X - q1.Y * q2.Y - q1.Z * q2.Z;

            return new Quaternion(newX, newY, newZ, newW);
        }
    }

    //Sendable input mappings
    public enum SendType
    {
        Keyboard,
        MouseMove,
        MouseClick,
        MouseScroll
    }

    public class SendableInput
    {
        public SendType Type;
        public string Name { get; set; }
        public SendableKey SendKey { get; set; }
        public int MouseX { get; set; }
        public int MouseY { get; set; }
        public int MouseScrollAmount { get; set; }
        public int MouseButton { get; set; }
        public uint MouseButtonDownFlag { get; set; }
        public uint MouseButtonUpFlag { get; set; }

        public SendableInput(SendableKey key)
        {
            Name = key.DisplayName;
            Type = SendType.Keyboard;
            SendKey = key;
        }

        public SendableInput(int mouseX, int mouseY)
        {
            Name = "MOUSE MOVE TO " + mouseX.ToString() + "," + mouseY.ToString();
            Type = SendType.MouseMove;
            MouseX = Math.Abs(mouseX);
            MouseY = Math.Abs(mouseY);
        }

        public SendableInput(int mouseButton)
        {
            Type = SendType.MouseClick;
            MouseButton = mouseButton;

            switch (mouseButton)
            {
                case 1:
                    Name = "MOUSE RIGHT CLICK";
                    MouseButtonDownFlag = WindowsAPI.MOUSEEVENTF_RIGHTDOWN;
                    MouseButtonUpFlag = WindowsAPI.MOUSEEVENTF_RIGHTUP;
                    break;
                case 2:
                    Name = "MOUSE MIDDLE CLICK";
                    MouseButtonDownFlag = WindowsAPI.MOUSEEVENTF_MIDDLEDOWN;
                    MouseButtonUpFlag = WindowsAPI.MOUSEEVENTF_MIDDLEUP;
                    break;
                default:
                    Name = "MOUSE LEFT CLICK";
                    MouseButtonDownFlag = WindowsAPI.MOUSEEVENTF_LEFTDOWN;
                    MouseButtonUpFlag = WindowsAPI.MOUSEEVENTF_RIGHTUP;
                    break;
            }
        }

        public SendableInput(int mouseScroll, bool scrollDown)
        {
            int direction = 1;
            string directionStr = "UP";

            if (scrollDown)
            {
                direction = -1;
                directionStr = "DOWN";
            }

            Name = "MOUSE SCROLL WHEEL " + Math.Abs(mouseScroll).ToString() + " " + directionStr;
            Type = SendType.MouseScroll;
            MouseScrollAmount = Math.Abs(mouseScroll) * direction;
        }
    }

    public class SendableKey
    {
        public string DisplayName { get; set; } //The name to show in UI
        public string SendValue { get; set; } //The SendKeys rough equivalent
        public ushort RawValue { get; set; } //The raw value (if applicable) in Keys
        public string AltValue { get; set; } //The alternative value, used for WXR InputAPI for now

        public SendableKey(string name, string sendVal, ushort val = 0)
        {
            DisplayName = name;
            SendValue = sendVal;
            RawValue = val;
            AltValue = sendVal;
        }

        public SendableKey(string name, string sendVal, string altVal, ushort val = 0)
        {
            DisplayName = name;
            SendValue = sendVal;
            RawValue = val;
            AltValue = altVal;
        }
    }

    //Input combos for translation between load/save and in-memory format
    public class SendableInputCombo
    {
        public List<SendableInput> ComboInputs { get; set; }

        public SendableInputCombo()
        {
            ComboInputs = new List<SendableInput>();
        }

        public static SerializableSendableCombo ToSerializable(SendableInputCombo a)
        {
            SerializableSendableCombo ans = new SerializableSendableCombo();

            foreach (SendableInput input in a.ComboInputs)
            {
                ans.ComboInputs.Add(input.Name);
            }

            return ans;
        }
    }

    public class SerializableSendableCombo
    {
        public List<string> ComboInputs { get; set; }

        public SerializableSendableCombo()
        {
            ComboInputs = new List<string>();
        }
    }
}
