using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XeRxKEYs.Gestures.MotionGestures
{
    public static class MotionLoadSave
    {
        public static void SaveProfiles(List<MotionGesture> motionProfiles)
        {
            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Motions");

            if (!Directory.Exists(profilesFolderPath))
            {
                Directory.CreateDirectory(profilesFolderPath);
            }

            JsonSerializerSettings options = new JsonSerializerSettings { Formatting = Formatting.Indented };
            foreach (MotionGesture profile in motionProfiles)
            {
                profile.OnSave();

                if (string.IsNullOrWhiteSpace(profile.Name)) continue;

                string filePath = Path.Combine(profilesFolderPath, profile.Name + ".json");

                string jsonString = JsonConvert.SerializeObject(profile, options);

                File.WriteAllText(filePath, jsonString);
            }
        }

        public static void LoadProfiles(ref List<MotionGesture> motionProfiles, ref IXRModule xrModuleInstance)
        {
            motionProfiles = new List<MotionGesture>();

            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Motions");

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
                    MotionGesture profile = JsonConvert.DeserializeObject<MotionGesture>(jsonString);
                    if (profile != null)
                    {
                        profile.OnLoad(ref xrModuleInstance);
                        motionProfiles.Add(profile);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + filePath + ": " + ex.Message, "XeRxKEYs - Error loading Motion Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
    }
}
