using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XeRxKEYs.Gestures.Triggers.Actions
{
    [JsonObject(MemberSerialization.OptOut)]
    public class TriggerAction
    {
        public SerializableJSONDataType Type = SerializableJSONDataType.TRIGGERACTION;
        public string Name { get; set; }

        public string Description { get; set; }

        public float WaitTime { get; set; } //TODO: Use this to add delay between key combo sends

        public List<SerializableSendableCombo> SerializableInputCombos { get; set; }

        [JsonIgnore]
        public List<SendableInputCombo> InputCombos { get; set; }

        [JsonIgnore]
        public string OriginalFileName { get; set; }

        private float waitTimer = 0f;

        public TriggerAction(string _name, string _desc = "", float _wait = 0.0f)
        {
            Name = _name;
            Description = _desc;
            WaitTime = _wait;

            SerializableInputCombos = new List<SerializableSendableCombo>();
            InputCombos = new List<SendableInputCombo>();

            waitTimer = 0f;
            OriginalFileName = "";
        }

        public void PrepareSendableInputs()
        {
            InputCombos.Clear();

            foreach (SerializableSendableCombo serializableSendableCombo in SerializableInputCombos)
            {
                SendableInputCombo sendableCombo = new SendableInputCombo();

                foreach (string serializableInput in serializableSendableCombo.ComboInputs)
                {
                    foreach (SendableInput input in InputHelper.AllSendableInputs)
                    {
                        if (input.Name == serializableInput)
                        {
                            sendableCombo.ComboInputs.Add(input);
                            break;
                        }
                    }
                }

                InputCombos.Add(sendableCombo);
            }
        }

        public void PrepareSerializableCombos()
        {
            SerializableInputCombos.Clear();

            foreach (SendableInputCombo combo in InputCombos)
            {
                SerializableInputCombos.Add(SendableInputCombo.ToSerializable(combo));
            }
        }

        public void UpdateWaitTimer(float msPassed)
        {
            waitTimer -= msPassed / 1000;

            if (waitTimer <= 0) waitTimer = 0;
        }

        public void ClearWaitTimer()
        {
            waitTimer = 0f;
        }
    }
}
