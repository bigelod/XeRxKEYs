using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XeRxKEYs.Gestures.MotionGestures;

namespace XeRxKEYs.Gestures.GestureProfiles
{
    public class GestureProfile
    {
        public string Name { get; set; }

        public List<MotionGesture> Gestures { get; set; }

        public GestureProfile(string _name) {
            Name = _name;
            Gestures = new List<MotionGesture>();
        }

        public void OnLoad()
        {
            foreach (MotionGesture gesture in Gestures)
            {
                gesture.OnLoad();
            }
        }

        public void OnSave()
        {
            foreach (MotionGesture gesture in Gestures)
            {
                gesture.OnSave();
            }
        }
    }
}
