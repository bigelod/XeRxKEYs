using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XeRxKEYs.Gestures.MotionGestures;

namespace XeRxKEYs.Gestures.Triggers.Actions
{
    public static class ActionLoadSave
    {
        public static void SaveProfiles(List<TriggerAction> actionProfiles, string overridePath = "")
        {
            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Actions");

            if (overridePath != "")
            {
                profilesFolderPath = overridePath;
            }

            if (!Directory.Exists(profilesFolderPath))
            {
                Directory.CreateDirectory(profilesFolderPath);
            }

            JsonSerializerSettings options = new JsonSerializerSettings { Formatting = Formatting.Indented };
            foreach (TriggerAction profile in actionProfiles)
            {
                profile.PrepareSerializableCombos();

                if (string.IsNullOrWhiteSpace(profile.Name)) continue;

                string filePath = Path.Combine(profilesFolderPath, profile.Name + ".json");

                string jsonString = JsonConvert.SerializeObject(profile, options);

                File.WriteAllText(filePath, jsonString); 
            }
        }

        public static void LoadProfiles(ref List<TriggerAction> actionProfiles)
        {
            actionProfiles = new List<TriggerAction>();

            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Actions");

            if (!Directory.Exists(profilesFolderPath))
            {
                Directory.CreateDirectory(profilesFolderPath);
            }

            string[] filePaths = Directory.GetFiles(profilesFolderPath, "*.json");

            foreach (string filePath in filePaths)
            {
                string jsonString = File.ReadAllText(filePath);
                try
                {
                    TriggerAction profile = JsonConvert.DeserializeObject<TriggerAction>(jsonString);
                    if (profile != null && profile.Type == SerializableJSONDataType.TRIGGERACTION)
                    {
                        profile.PrepareSendableInputs();
                        actionProfiles.Add(profile);
                    }
                    else
                    {
                        MessageBox.Show("Error reading file " + filePath + ": File does not appear to be an Action Trigger!", "XeRxKEYs - Error Loading Action Trigger", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + filePath + ": " + ex.Message, "XeRxKEYs - Error loading Action Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static bool LoadProfile(ref List<TriggerAction> actionProfiles, string newProfile)
        {
            if (File.Exists(newProfile))
            {
                string jsonString = File.ReadAllText(newProfile);
                try
                {
                    TriggerAction profile = JsonConvert.DeserializeObject<TriggerAction>(jsonString);
                    if (profile != null && profile.Type == SerializableJSONDataType.TRIGGERACTION)
                    {
                        bool addNew = true;

                        foreach (TriggerAction existingProfile in actionProfiles)
                        {
                            if (existingProfile.Name == profile.Name)
                            {
                                DialogResult result = MessageBox.Show("An action named '" + existingProfile.Name + "' already exists, overwrite?", "XeRxKEYs - Action Profile Exists!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                                addNew = false;

                                if (result == DialogResult.OK)
                                {
                                    existingProfile.Description = profile.Description;
                                    existingProfile.WaitTime = profile.WaitTime;

                                    existingProfile.SerializableInputCombos.Clear();
                                    foreach (SerializableSendableCombo combo in profile.SerializableInputCombos)
                                    {
                                        existingProfile.SerializableInputCombos.Add(combo);
                                    }

                                    existingProfile.PrepareSendableInputs();

                                    return true;
                                }

                                break;
                            }
                        }

                        if (addNew)
                        {
                            profile.PrepareSendableInputs();
                            actionProfiles.Add(profile);

                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error reading file " + newProfile + ": File does not appear to be an Action Trigger!", "XeRxKEYs - Error Loading Action Trigger", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + newProfile + ": " + ex.Message, "XeRxKEYs - Error loading Action Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            return false;
        }
    }
}
