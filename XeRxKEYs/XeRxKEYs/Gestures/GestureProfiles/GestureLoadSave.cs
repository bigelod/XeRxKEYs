using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.Security.Cryptography;
using XeRxKEYs.Gestures.MotionGestures;

namespace XeRxKEYs.Gestures.GestureProfiles
{
    public static class GestureLoadSave
    {
        public static void SaveProfiles(List<GestureProfile> gestureProfiles, string overridePath = "")
        {
            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Profiles");

            if (overridePath != "")
            {
                profilesFolderPath = overridePath;
            }

            if (!Directory.Exists(profilesFolderPath))
            {
                Directory.CreateDirectory(profilesFolderPath);
            }

            JsonSerializerSettings options = new JsonSerializerSettings { Formatting = Formatting.Indented };
            foreach (GestureProfile profile in gestureProfiles)
            {
                profile.OnSave();

                if (string.IsNullOrWhiteSpace(profile.Name)) continue;

                string filePath = Path.Combine(profilesFolderPath, profile.Name + ".json");

                string jsonString = JsonConvert.SerializeObject(profile, options);

                File.WriteAllText(filePath, jsonString);
            }
        }

        public static void LoadProfiles(ref List<GestureProfile> gestureProfiles, ref IXRModule xrModuleInstance)
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
                    GestureProfile profile = JsonConvert.DeserializeObject<GestureProfile>(jsonString);
                    if (profile != null && profile.Type == SerializableJSONDataType.GESTUREPROFILE)
                    {
                        profile.OnLoad(ref xrModuleInstance);
                        gestureProfiles.Add(profile);
                    }
                    else
                    {
                        MessageBox.Show("Error reading file " + filePath + ": File does not appear to be an Gesture Profile!", "XeRxKEYs - Error Loading Gesture Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + filePath + ": " + ex.Message, "XeRxKEYs - Error loading Gesture Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static bool LoadProfile(ref List<GestureProfile> gestureProfiles, ref IXRModule xrModuleInstance, string newProfile)
        {
            if (File.Exists(newProfile))
            {
                string jsonString = File.ReadAllText(newProfile);
                try
                {
                    GestureProfile profile = JsonConvert.DeserializeObject<GestureProfile>(jsonString);
                    if (profile != null && profile.Type == SerializableJSONDataType.GESTUREPROFILE)
                    {
                        bool addNew = true;

                        foreach (GestureProfile existingProfile in gestureProfiles)
                        {
                            if (existingProfile.Name == profile.Name)
                            {
                                DialogResult result = MessageBox.Show("A profile named '" + existingProfile.Name + "' already exists, overwrite?", "XeRxKEYs - Gesture Profile Exists!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                                addNew = false;

                                if (result == DialogResult.OK)
                                {
                                    existingProfile.Description = profile.Description;
                                    existingProfile.Image = profile.Image;

                                    existingProfile.Gestures.Clear();
                                    foreach (MotionGesture gesture in profile.Gestures)
                                    {
                                        existingProfile.Gestures.Add(gesture);
                                    }

                                    existingProfile.OnLoad(ref xrModuleInstance);

                                    return true;
                                }

                                break;
                            }
                        }

                        if (addNew)
                        {
                            profile.OnLoad(ref xrModuleInstance);
                            gestureProfiles.Add(profile);

                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error reading file " + newProfile + ": File does not appear to be an Gesture Profile!", "XeRxKEYs - Error Loading Gesture Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + newProfile + ": " + ex.Message, "XeRxKEYs - Error loading Gesture Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            return false;
        }
    }
}
