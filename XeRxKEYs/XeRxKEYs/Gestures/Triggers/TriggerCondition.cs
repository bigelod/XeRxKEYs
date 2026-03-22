using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace XeRxKEYs.Gestures.Triggers
{
    public enum TriggerConditionType
    {
        Shake_Vertical,
        Stab,
        Twist,
        Slash,
        Swipe,
        Proximity
    }

    public class TriggerCondition
    {
        public TriggerConditionType Type { get; set; }

        public List<TriggerCondition> Require_Trigger_Conditions { get; set; } //TODO: Check these other triggers to see if we can activate
        public List<TriggerCondition> Disable_If_Trigger_Conditions { get; set; } //TODO: Check these other triggers to see if we CANT activate

        public Shake_Event ShakeEvent { get; set; }
        public Proximity_Event ProximityEvent { get; set; }

        public TriggerCondition(TriggerConditionType type)
        {
            Type = type;
            Require_Trigger_Conditions = new List<TriggerCondition>();
            Disable_If_Trigger_Conditions = new List<TriggerCondition>();
            ShakeEvent = new Shake_Event();
            ProximityEvent = new Proximity_Event();
        }

        public bool CheckTrigger()
        {
            bool triggered = false;

            switch (Type)
            {
                case TriggerConditionType.Shake_Vertical:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidShake()) triggered = true;
                        }
                    }
                    break;
                case TriggerConditionType.Stab:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidStab()) triggered = true;
                        }
                    }
                    break;
                case TriggerConditionType.Twist:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidTwist()) triggered = true;
                        }
                    }
                    break;
                case TriggerConditionType.Slash:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidSlash()) triggered = true;
                        }
                    }
                    break;
                case TriggerConditionType.Swipe:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidSwipe()) triggered = true;
                        }
                    }
                    break;
                case TriggerConditionType.Proximity:
                    if (ProximityEvent != null)
                    {
                        bool anyProxEventTriggered = false;

                        foreach (TrackedObject objA in ProximityEvent.Device_Group_A)
                        {
                            foreach (TrackedObject objB in ProximityEvent.Device_Group_B)
                            {

                            }
                        }

                        if (anyProxEventTriggered)
                        {
                            if (!ProximityEvent.Invert)
                            {
                                triggered = true;
                            }
                        }
                        else if (ProximityEvent.Invert)
                        {
                            triggered = true;
                        }
                    }
                    break;
            }

            return triggered;
        }
     }

    public class ChangeAmount
    {
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float Duration { get; set; }
        public float SensitivityScale { get; set; }
        public ChangeAmount(float _min = 0.0f, float _max = 0.0f, float _dur = 0.33f, float _sens = 1.0f)
        {
            MinValue = _min;
            MaxValue = _max;
            Duration = _dur;
            SensitivityScale = _sens;
        }
    }

    public class Shake_Event
    {
        public List<TrackedObject> Trigger_For_Objects { get; set; } //Which object(s) can trigger this condition?

        public ChangeAmount Trigger_When { get; set; } //TODO: Use this to modify the values of the shake event(s) of a given TrackedObject

        public Shake_Event()
        {
            Trigger_For_Objects = new List<TrackedObject>();
            Trigger_When = new ChangeAmount();
        }
    }

    public class Proximity_Event
    {
        public List<TrackedObject> Device_Group_A { get; set; }
        public List<TrackedObject> Device_Group_B { get; set; }

        public ChangeAmount Trigger_When { get; set; } //Trigger when any Device A is within range of Device B by Change_Amount

        public bool Invert { get; set; } //Trigger when this is NOT within range instead

        public Proximity_Event(bool _inv = false)
        {
            Device_Group_A = new List<TrackedObject>();
            Device_Group_B = new List<TrackedObject>();
            Trigger_When = new ChangeAmount();
            Invert = _inv;
        }
    }
}
