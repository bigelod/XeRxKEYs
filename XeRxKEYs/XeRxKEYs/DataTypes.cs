using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinApi;

namespace XeRxKEYs
{
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
            W = 0.0f;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }

    public class TrackedObject
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
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
        public string DisplayName { get; set; }
        public string SendValue { get; set; }
        public ushort RawValue { get; set; }

        public SendableKey(string name, string sendVal, ushort val = 0)
        {
            DisplayName = name;
            SendValue = sendVal;
            RawValue = val;
        }
    }

    public class SendableInputCombo
    {
        public List<SendableInput> ComboInputs { get; set; }

        public SendableInputCombo()
        {
            ComboInputs = new List<SendableInput>();
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
