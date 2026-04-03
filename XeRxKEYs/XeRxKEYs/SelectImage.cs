using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XeRxKEYs
{
    public partial class SelectImage : Form
    {
        private static ImageList _globalImageList;

        List<string> imageNames = new List<string>();

        private string selectedIconName;

        private Action<string> returnAct;

        private CancellationTokenSource _cancellationTokenSource;

        public SelectImage()
        {
            InitializeComponent();
        }

        private void SelectImage_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancellationTokenSource?.Cancel();

            if (returnAct != null)
            {
                returnAct.Invoke(selectedIconName);
            }
        }

        public void SendIconTo(Action<string> act)
        {
            returnAct = act;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            selectedIconName = "";
            Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (lvwIcons.SelectedItems.Count > 0)
            {
                selectedIconName = imageNames[lvwIcons.SelectedItems[0].Index];
            }
            Close();
        }

        private void lvwIcons_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSelect.Enabled = true;
        }

        private async void SelectImage_Load(object sender, EventArgs e)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            selectedIconName = "";
            FormClosing += SelectImage_FormClosing;

            string imageDirectory = Path.Combine(Application.StartupPath, "Images");

            ImageList largeImageList = new ImageList();
            largeImageList.ImageSize = new Size(64, 64);
            largeImageList.ColorDepth = ColorDepth.Depth32Bit;

            try
            {
                Image no_icon = Image.FromFile(Path.Combine(imageDirectory, "0.png"));
                largeImageList.Images.Add(no_icon);
            }
            catch
            {

            }

            lvwIcons.LargeImageList = largeImageList;
            lvwIcons.View = View.LargeIcon;

            if (_globalImageList == null)
            {
                _globalImageList = new ImageList();
                _globalImageList.ImageSize = new Size(64, 64);
                _globalImageList.ColorDepth = ColorDepth.Depth32Bit;
            }

            string[] imagePaths = Directory.GetFiles(imageDirectory, "*.png");
                        
            if (imagePaths.Length > 0 && _globalImageList.Images.Count == imagePaths.Length - 1)
            {
                lvwIcons.LargeImageList = _globalImageList;
            }
            else
            {
                //List isn't accurate anymore, reload
                _globalImageList.Images.Clear();
            }

            lvwIcons.BeginUpdate();
            try
            {
                foreach (string path in imagePaths)
                {
                    if (Path.GetFileName(path) == "0.png") continue;

                    int imgIndex = 0;
                    if (_globalImageList.Images.Count == imagePaths.Length - 1)
                    {
                        imgIndex = imageNames.Count();
                    }

                    imageNames.Add(Path.GetFileName(path));
                    ListViewItem item = new ListViewItem("", imgIndex);
                    item.Tag = path;
                    lvwIcons.Items.Add(item);
                }
            }
            finally
            {
                lvwIcons.EndUpdate();
            }

            if (_globalImageList.Images.Count == 0)
            {
                try
                {
                    await LoadImagesAsync(_cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {

                }
            }
        }

        private async Task LoadImagesAsync(CancellationToken token)
        {
            foreach (ListViewItem item in lvwIcons.Items)
            {
                string path = item.Tag.ToString();

                try
                {
                    Image img = await Task.Run(() => LoadImageFromFile(path));

                    if (token.IsCancellationRequested)
                    {
                        _globalImageList.Images.Clear();
                        return;
                    }

                    if (img != null)
                    {
                        _globalImageList.Images.Add(img);
                        lvwIcons.LargeImageList.Images.Add(img);
                        item.ImageIndex = lvwIcons.LargeImageList.Images.Count - 1;
                    }
                }
                catch (Exception ex)
                {
                    item.Text = Path.GetFileNameWithoutExtension(path);
                    item.ImageIndex = 0;
                }
            }

            lvwIcons.Update();
        }

        private Image LoadImageFromFile(string path)
        {
            if (!File.Exists(path)) return null;

            byte[] bytes = File.ReadAllBytes(path);
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }
    }
}
