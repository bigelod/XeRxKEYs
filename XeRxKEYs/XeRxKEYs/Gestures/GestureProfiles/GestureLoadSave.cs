using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace XeRxKEYs.Gestures.GestureProfiles
{
    public static class GestureLoadSave
    {
        public static void SaveProfiles(List<GestureProfile> gestureProfiles)
        {
            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Profiles");

            if (!Directory.Exists(profilesFolderPath))
            {
                Directory.CreateDirectory(profilesFolderPath);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            foreach (GestureProfile profile in gestureProfiles)
            {
                if (string.IsNullOrWhiteSpace(profile.Name)) continue;

                string filePath = Path.Combine(profilesFolderPath, profile.Name + ".json");

                string jsonString = JsonSerializer.Serialize(profile, options);

                File.WriteAllText(filePath, jsonString);
            }
        }

        public static void LoadProfiles(ref List<GestureProfile> gestureProfiles)
        {
            gestureProfiles = new List<GestureProfile>();

            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Profiles");

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
                    GestureProfile profile = JsonSerializer.Deserialize<GestureProfile>(jsonString);
                    if (profile != null)
                    {
                        gestureProfiles.Add(profile);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + filePath + ": " + ex.Message, "XeRxKEYs - Error loading Gesture Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
