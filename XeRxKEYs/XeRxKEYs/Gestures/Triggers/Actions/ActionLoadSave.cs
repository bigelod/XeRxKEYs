using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

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

            var options = new JsonSerializerOptions { WriteIndented = true };
            foreach (TriggerAction profile in actionProfiles)
            {
                if (string.IsNullOrWhiteSpace(profile.Name)) continue;

                string filePath = Path.Combine(profilesFolderPath, profile.Name + ".json");

                string jsonString = JsonSerializer.Serialize(profile, options);

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
                    TriggerAction profile = JsonSerializer.Deserialize<TriggerAction>(jsonString);
                    if (profile != null)
                    {
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
