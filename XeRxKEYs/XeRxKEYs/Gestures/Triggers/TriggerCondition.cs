using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

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

        public List<TriggerCondition> Require_Trigger_Conditions { get; set; }
        public List<TriggerCondition> Disable_If_Trigger_Conditions { get; set; }


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

        public void OnLoad(ref IXRModule xrModuleInstance)
        {
            switch (Type)
            {
                case TriggerConditionType.Shake_Vertical:
                case TriggerConditionType.Stab:
                case TriggerConditionType.Twist:
                case TriggerConditionType.Slash:
                case TriggerConditionType.Swipe:
                    if (ShakeEvent != null)
                    {
                        ShakeEvent.PrepareTrackedObjects(ref xrModuleInstance);
                    }
                    ProximityEvent = new Proximity_Event();
                    break;
                case TriggerConditionType.Proximity:
                    if (ProximityEvent != null)
                    {
                        ProximityEvent.PrepareTrackedObjects(ref xrModuleInstance);
                    }
                    ShakeEvent = new Shake_Event();
                    break;
            }

            foreach (TriggerCondition condition in Require_Trigger_Conditions)
            {
                condition.OnLoad(ref xrModuleInstance);
            }

            foreach (TriggerCondition condition in Disable_If_Trigger_Conditions)
            {
                condition.OnLoad(ref xrModuleInstance);
            }
        }

        public void OnSave()
        {
            switch (Type)
            {
                case TriggerConditionType.Shake_Vertical:
                case TriggerConditionType.Stab:
                case TriggerConditionType.Twist:
                case TriggerConditionType.Slash:
                case TriggerConditionType.Swipe:
                    if (ShakeEvent != null)
                    {
                        ShakeEvent.PrepareSerializableTrackedObjects();
                    }
                    ProximityEvent = null;
                    break;
                case TriggerConditionType.Proximity:
                    if (ProximityEvent != null)
                    {
                        ProximityEvent.PrepareSerializableTrackedObjects();
                    }
                    ShakeEvent = null;
                    break;
            }

            foreach (TriggerCondition condition in Require_Trigger_Conditions)
            {
                condition.OnSave();
            }

            foreach (TriggerCondition condition in Disable_If_Trigger_Conditions)
            {
                condition.OnSave();
            }
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

                            if (triggered) break;
                        }
                    }
                    break;
                case TriggerConditionType.Stab:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidStab()) triggered = true;

                            if (triggered) break;
                        }
                    }
                    break;
                case TriggerConditionType.Twist:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidTwist()) triggered = true;

                            if (triggered) break;
                        }
                    }
                    break;
                case TriggerConditionType.Slash:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidSlash()) triggered = true;

                            if (triggered) break;
                        }
                    }
                    break;
                case TriggerConditionType.Swipe:
                    if (ShakeEvent != null)
                    {
                        foreach (TrackedObject obj in ShakeEvent.Trigger_For_Objects)
                        {
                            if (obj.DidSwipe()) triggered = true;

                            if (triggered) break;
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
                                float distance = Vector3.Distance(objA.Position, objB.Position);

                                if (distance >= ProximityEvent.Trigger_When.MinValue || ProximityEvent.Trigger_When.MinValue <= 0)
                                {
                                    if (distance <= ProximityEvent.Trigger_When.MaxValue)
                                    {
                                        anyProxEventTriggered = true;
                                        break;
                                    }
                                }
                            }

                            if (anyProxEventTriggered) break;
                        }

                        if (ProximityEvent.Invert)
                        {
                            if (anyProxEventTriggered)
                            {
                                ProximityEvent.ClearEvent();
                            }
                            else
                            {
                                triggered = ProximityEvent.CanTrigger();
                            }
                        }
                        else
                        {
                            if (anyProxEventTriggered)
                            {
                                triggered = ProximityEvent.CanTrigger();
                            }
                            else
                            {
                                ProximityEvent.ClearEvent();
                            }
                        }
                    }
                    break;
            }

            if (triggered)
            {
                foreach (TriggerCondition requiredConditions in Require_Trigger_Conditions)
                {
                    if (!requiredConditions.CheckTrigger())
                    {
                        return false;
                    }
                }

                foreach(TriggerCondition disableConditions in Disable_If_Trigger_Conditions)
                {
                    if (disableConditions.CheckTrigger())
                    {
                        return false;
                    }
                }
            }

            return triggered;
        }
    }

    public class ChangeAmount
    {
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float Duration { get; set; } //Unused in a proximity event (for now?)
        public float SensitivityScale { get; set; } //Unused in a proximity event
        public ChangeAmount(float _min = 0.0f, float _max = 0.0f, float _dur = 0.33f, float _sens = 1.0f)
        {
            MinValue = _min;
            MaxValue = _max;
            Duration = _dur;
            SensitivityScale = _sens;
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class Shake_Event
    {
        public List<SerializableTrackedObject> SerializableTriggerForObjects { get; set; }

        [JsonIgnore]
        public List<TrackedObject> Trigger_For_Objects { get; set; } //Which object(s) can trigger this condition?

        public ChangeAmount Trigger_When { get; set; } //TODO: Use this to modify the values of the shake event(s) of a given TrackedObject

        public Shake_Event()
        {
            Trigger_For_Objects = new List<TrackedObject>();
            Trigger_When = new ChangeAmount();

            SerializableTriggerForObjects = new List<SerializableTrackedObject>();
        }

        public void PrepareTrackedObjects(ref IXRModule xrModuleInstance)
        {
            Trigger_For_Objects.Clear();

            if (xrModuleInstance != null)
            {
                foreach (SerializableTrackedObject serializedObj in SerializableTriggerForObjects)
                {
                    TrackedObject obj = xrModuleInstance.GetTrackedObject(serializedObj);

                    if (obj != null)
                    {
                        Trigger_For_Objects.Add(obj);
                    }
                }
            }
        }

        public void PrepareSerializableTrackedObjects()
        {
            SerializableTriggerForObjects.Clear();

            foreach (TrackedObject obj in Trigger_For_Objects)
            {
                SerializableTriggerForObjects.Add(new SerializableTrackedObject(obj));
            }
        }
    }

    [JsonObject(MemberSerialization.OptOut)]
    public class Proximity_Event
    {
        public List<SerializableTrackedObject> SerializableDeviceGroupA { get; set; }
        public List<SerializableTrackedObject> SerializableDeviceGroupB { get; set; }

        [JsonIgnore]
        public List<TrackedObject> Device_Group_A { get; set; }
        [JsonIgnore]
        public List<TrackedObject> Device_Group_B { get; set; }

        public ChangeAmount Trigger_When { get; set; } //Trigger when any Device A is within range of Device B by Change_Amount

        public bool Invert { get; set; } //Trigger when this is NOT within range instead

        public bool TriggerOnce { get; set; } //Trigger once while true?

        private bool triggered = false;

        public Proximity_Event(bool _inv = false, bool _triggeronce = false)
        {
            Device_Group_A = new List<TrackedObject>();
            Device_Group_B = new List<TrackedObject>();
            Trigger_When = new ChangeAmount();
            Invert = _inv;
            TriggerOnce = _triggeronce;

            SerializableDeviceGroupA = new List<SerializableTrackedObject>();
            SerializableDeviceGroupB = new List<SerializableTrackedObject>();
        }

        public void PrepareTrackedObjects(ref IXRModule xrModuleInstance)
        {
            Device_Group_A.Clear();
            Device_Group_B.Clear();

            if (xrModuleInstance != null)
            {
                foreach (SerializableTrackedObject serializedObj in SerializableDeviceGroupA)
                {
                    TrackedObject obj = xrModuleInstance.GetTrackedObject(serializedObj);

                    if (obj != null)
                    {
                        Device_Group_A.Add(obj);
                    }
                }

                foreach (SerializableTrackedObject serializedObj in SerializableDeviceGroupB)
                {
                    TrackedObject obj = xrModuleInstance.GetTrackedObject(serializedObj);

                    if (obj != null)
                    {
                        Device_Group_B.Add(obj);
                    }
                }
            }
        }

        public void PrepareSerializableTrackedObjects()
        {
            SerializableDeviceGroupA.Clear();
            SerializableDeviceGroupB.Clear();

            foreach (TrackedObject obj in Device_Group_A)
            {
                SerializableDeviceGroupA.Add(new SerializableTrackedObject(obj));
            }

            foreach (TrackedObject obj in Device_Group_B)
            {
                SerializableDeviceGroupB.Add(new SerializableTrackedObject(obj));
            }
        }

        public bool CanTrigger()
        {
            if (triggered && TriggerOnce) return false;

            triggered = true;

            return true;
        }

        public void ClearEvent()
        {
            triggered = false;
        }
    }
}
