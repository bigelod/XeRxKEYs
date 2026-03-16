using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XeRxKEYs.Gestures.Triggers.Actions
{
    public class TriggerAction
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public float WaitTime { get; set; }

        public List<SerializableSendableCombo> SerializableInputCombos { get; set; }

        [IgnoreDataMember]
        public List<SendableInputCombo> InputCombos { get; set; }

        public TriggerAction(string _name, string _desc = "", float _wait = 0.0f)
        {
            Name = _name;
            Description = _desc;
            WaitTime = _wait;

            SerializableInputCombos = new List<SerializableSendableCombo>();
            InputCombos = new List<SendableInputCombo>();
        }
    }
}
