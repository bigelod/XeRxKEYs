using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XeRxKEYs.Gestures.Triggers;
using XeRxKEYs.Gestures.Triggers.Actions;

namespace XeRxKEYs.Gestures.MotionGestures
{
    public class MotionGesture
    {
        public SerializableJSONDataType Type = SerializableJSONDataType.MOTIONGESTURE;
        public string Name { get; set; }

        public string Image { get; set; }
        public string Description { get; set; }

        public float Sensitivity { get; set; } //TODO: Support passing sensitivity to the TriggerConditions

        public float Cooldown { get; set; }

        public List<TriggerCondition> TriggerConditions { get; set; }

        public List<TriggerAction> TriggerActions { get; set; }

        public bool TriggerOnAnyCondition { get; set; }

        private float cooldownTimer = 0f;

        public MotionGesture(string _name, string _desc = "", string _img = "", bool _triggerAny = false)
        {
            Name = _name;
            Description = _desc;
            Image = _img;
            Sensitivity = 1.0f;
            Cooldown = 0.0f;

            cooldownTimer = 0f;

            TriggerConditions = new List<TriggerCondition>();
            TriggerActions = new List<TriggerAction>();
            TriggerOnAnyCondition = _triggerAny;
        }

        public void OnLoad(ref IXRModule xrModuleInstance)
        {
            foreach (TriggerCondition condition in TriggerConditions)
            {
                condition.OnLoad(ref xrModuleInstance);
            }

            foreach (TriggerAction action in TriggerActions)
            {
                action.PrepareSendableInputs();
            }
        }

        public void OnSave()
        {
            foreach (TriggerCondition condition in TriggerConditions)
            {
                condition.OnSave();
            }

            foreach (TriggerAction action in TriggerActions)
            {
                action.PrepareSerializableCombos();
            }
        }

        public List<SendableInput> GatherInputs()
        {
            List<SendableInput> ans = new List<SendableInput>();

            if (CheckConditions())
            {
                foreach (TriggerAction action in TriggerActions)
                {
                    foreach (SendableInputCombo combo in action.InputCombos)
                    {
                        ans.AddRange(combo.ComboInputs);
                    }
                }
            }

            return ans;
        }

        private bool CheckConditions()
        {
            if (TriggerConditions.Count <= 0 || cooldownTimer > 0) return false;

            bool anyFound = false;

            foreach (TriggerCondition condition in TriggerConditions)
            {
                if (condition.CheckTrigger())
                {
                    anyFound = true;
                    if (TriggerOnAnyCondition)
                    {
                        cooldownTimer = Cooldown;
                        return true;
                    }
                }
                else
                {
                    if (!TriggerOnAnyCondition) return false;
                }
            }

            if (anyFound) cooldownTimer = Cooldown;

            return anyFound;
        }

        public void UpdateCooldown(float msPassed)
        {
            cooldownTimer -= msPassed / 1000;

            if (cooldownTimer <= 0) cooldownTimer = 0;
        }

        public void ClearCooldown()
        {
            cooldownTimer = 0f;
        }
    }
}
