using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XeRxKEYs
{
    public static class ImageHelper
    {
        //TODO: Decide object type for displaying the images in UI
        public static void LoadImageToObject(object a, string imgURL)
        {
            string imgDir = Path.Combine(Application.StartupPath, "Images");

            if (!Directory.Exists(imgDir))
            {
                Directory.CreateDirectory(imgDir);
            }

            string imgFile = Path.Combine(imgDir, imgURL);

            if (File.Exists(imgFile))
            {
                //TODO: Load and display the file
            }
        }
    }
}
