using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XeRxKEYs.Gestures.GestureProfiles;
using XeRxKEYs.Gestures.Triggers;
using XeRxKEYs.Gestures.Triggers.Actions;

namespace XeRxKEYs.Gestures.MotionGestures
{
    public static class MotionLoadSave
    {
        public static void SaveProfiles(List<MotionGesture> motionProfiles, string overridePath = "")
        {
            string exePath = Application.StartupPath;
            string profilesFolderPath = Path.Combine(exePath, "Motions");

            if (overridePath != "")
            {
                profilesFolderPath = overridePath;
            }

            if (!Directory.Exists(profilesFolderPath))
            {
                Directory.CreateDirectory(profilesFolderPath);
            }

            JsonSerializerSettings options = new JsonSerializerSettings { Formatting = Formatting.Indented };
            foreach (MotionGesture profile in motionProfiles)
            {
                profile.OnSave();

                if (string.IsNullOrWhiteSpace(profile.Name)) continue;

                if (overridePath != "" && profile.Image != "")
                {
                    //Also export the images from library
                    string imgPath = Path.Combine(exePath, "Images", profile.Image);
                    string copyToDir = Path.Combine(profilesFolderPath, "Images");
                    string copyToPath = imgPath.Replace(exePath, profilesFolderPath);

                    if (!Directory.Exists(copyToDir))
                    {
                        Directory.CreateDirectory(copyToDir);
                    }

                    if (File.Exists(imgPath))
                    {
                        try
                        {
                            File.Copy(imgPath, copyToPath);
                        }
                        catch
                        {

                        }
                    }
                }

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
                    if (profile != null && profile.Type == SerializableJSONDataType.MOTIONGESTURE)
                    {
                        profile.OriginalFileName = filePath;
                        profile.OnLoad(ref xrModuleInstance);
                        motionProfiles.Add(profile);
                    }
                    else
                    {
                        MessageBox.Show("Error reading file " + filePath + ": File does not appear to be a Motion Gesture!", "XeRxKEYs - Error Loading Motion Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + filePath + ": " + ex.Message, "XeRxKEYs - Error loading Motion Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        public static bool LoadProfile(ref List<MotionGesture> motionProfiles, ref IXRModule xrModuleInstance, string newProfile)
        {
            if (File.Exists(newProfile))
            {
                //Check for images and copy them if we don't have them already as well
                string profileDir = Path.GetDirectoryName(newProfile);
                string checkImgDir = Path.Combine(profileDir, "Images");
                string libraryImgDir = Path.Combine(Application.StartupPath, "Images");

                if (Directory.Exists(checkImgDir))
                {
                    IEnumerable<string> pngFiles = Directory.EnumerateFiles(checkImgDir, "*.png", SearchOption.TopDirectoryOnly);

                    foreach (string currentFile in pngFiles)
                    {
                        string newFile = currentFile.Replace(checkImgDir, libraryImgDir);

                        try
                        {
                            File.Copy(currentFile, newFile);
                        }
                        catch
                        {

                        }
                    }
                }

                string jsonString = File.ReadAllText(newProfile);
                try
                {
                    MotionGesture profile = JsonConvert.DeserializeObject<MotionGesture>(jsonString);
                    if (profile != null && profile.Type == SerializableJSONDataType.MOTIONGESTURE)
                    {
                        bool addNew = true;

                        foreach (MotionGesture existingProfile in motionProfiles)
                        {
                            if (existingProfile.Name == profile.Name)
                            {
                                DialogResult result = MessageBox.Show("A gesture named '" + existingProfile.Name + "' already exists, overwrite?", "XeRxKEYs - Motion Gesture Exists!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);

                                addNew = false;

                                if (result == DialogResult.OK)
                                {
                                    existingProfile.Description = profile.Description;
                                    existingProfile.Image = profile.Image;
                                    existingProfile.Sensitivity = profile.Sensitivity;
                                    existingProfile.Cooldown = profile.Cooldown;
                                    existingProfile.TriggerOnAnyCondition = profile.TriggerOnAnyCondition;

                                    existingProfile.TriggerConditions.Clear();
                                    foreach (TriggerCondition condition in profile.TriggerConditions)
                                    {
                                        existingProfile.TriggerConditions.Add(condition);
                                    }

                                    existingProfile.TriggerActions.Clear();
                                    foreach (TriggerAction action in profile.TriggerActions)
                                    {
                                        existingProfile.TriggerActions.Add(action);
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
                            motionProfiles.Add(profile);

                            return true;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error reading file " + newProfile + ": File does not appear to be a Motion Gesture!", "XeRxKEYs - Error Loading Motion Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show("Error reading file " + newProfile + ": " + ex.Message, "XeRxKEYs - Error loading Motion Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            return false;
        }
    }
}
