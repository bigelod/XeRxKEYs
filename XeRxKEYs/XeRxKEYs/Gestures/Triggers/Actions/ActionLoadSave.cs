using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace XeRxKEYs.Gestures.Triggers.Actions
{
    public static class ActionLoadSave
    {
        public static void SaveProfiles(List<TriggerAction> actionProfiles)
        {
            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Actions");

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
                    if (profile != null)
                    {
                        profile.PrepareSendableInputs();
                        actionProfiles.Add(profile);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + filePath + ": " + ex.Message, "XeRxKEYs - Error loading Action Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
