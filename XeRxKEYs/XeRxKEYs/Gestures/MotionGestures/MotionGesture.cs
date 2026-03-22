using System;
using System.Collections.Generic;
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
        public string Name { get; set; }

        public string Image { get; set; }
        public string Description { get; set; }

        public float Sensitivity { get; set; } //TODO: Support passing sensitivity to the TriggerConditions

        public float Cooldown { get; set; } //TODO: Support a cooldown so this can't trigger back-to-back if so

        public List<TriggerCondition> TriggerConditions { get; set; }

        public List<TriggerAction> TriggerActions { get; set; }

        public bool TriggerOnAnyCondition { get; set; } //Trigger if any condition is true?

        public MotionGesture(string _name, string _desc = "", string _img = "", bool _triggerAny = false)
        {
            Name = _name;
            Description = _desc;
            Image = _img;
            Sensitivity = 1.0f;
            Cooldown = 0.0f;

            TriggerConditions = new List<TriggerCondition>();
            TriggerActions = new List<TriggerAction>();
            TriggerOnAnyCondition = _triggerAny;
        }

        public void OnLoad()
        {
            foreach (TriggerAction action in TriggerActions)
            {
                action.PrepareSendableInputs();
            }
        }

        public void OnSave()
        {
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
            if (TriggerConditions.Count <= 0) return false;

            bool anyFound = false;

            foreach (TriggerCondition condition in TriggerConditions)
            {
                if (!condition.CheckTrigger())
                {
                    anyFound = true;
                    if (!TriggerOnAnyCondition) return false;
                }
            }

            return anyFound;
        }
    }
}
