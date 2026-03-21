using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XeRxKEYs.Gestures.Triggers;
using XeRxKEYs.Gestures.Triggers.Actions;

namespace XeRxKEYs.Gestures.MotionGestures
{
    public class MotionGesture
    {
        public string Name { get; set; }

        public string Image { get; set; }
        public string Description { get; set; }

        public float Sensitivity { get; set; }

        public float Cooldown { get; set; }

        public List<TriggerCondition> TriggerConditions { get; set; }

        public List<TriggerAction> TriggerActions { get; set; }

        public MotionGesture(string _name, string _desc = "", string _img = "")
        {
            Name = _name;
            Description = _desc;
            Image = _img;
            Sensitivity = 1.0f;
            Cooldown = 0.0f;

            TriggerConditions = new List<TriggerCondition>();
            TriggerActions = new List<TriggerAction>();
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
    }
}
