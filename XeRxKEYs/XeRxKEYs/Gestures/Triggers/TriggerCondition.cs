using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XeRxKEYs.Gestures.Triggers
{
    public enum TriggerConditionType
    {
        Shake_Vertical,
        Shake_Horizontal,
        Stab,
        Twist,
        Slash,
        Swipe,
        Proximity
    }

    public class TriggerCondition
    {
        public TriggerConditionType Type { get; set; }

        public List<TriggerCondition> Require_Trigger_Conditions = new List<TriggerCondition>();
        public List<TriggerCondition> Disable_If_Trigger_Conditions = new List<TriggerCondition>();
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

        public ChangeAmount Trigger_When { get; set; } //Trigger when shaken by this amount

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
