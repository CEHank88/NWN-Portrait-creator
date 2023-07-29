using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        private string _folderPath;
        private FileSystemWatcher _fileWatcher;

        public Form1()
        {
            InitializeComponent();
            ActualizarListBox();
            _fileWatcher = new FileSystemWatcher
            {
                Path = _folderPath,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = "*.tga"
            };
            _fileWatcher.Created += new FileSystemEventHandler(OnFileCreated);
            _fileWatcher.EnableRaisingEvents = true;
            this.Load += Form1_Load;

        }
        /*private void Form1_Load(object sender, EventArgs e)
        {
            string fileName = "config.ini";
            if (!File.Exists(fileName))
            {
                // Show folder browser dialog to select folder
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Select the folder";
                    dialog.ShowNewFolderButton = true;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        // create the config file
                        File.WriteAllText(fileName, $"FolderPath={dialog.SelectedPath}");
                    }
                    else
                    {
                        Close();
                        return;
                    }
                }
            }
            else
            {
                // read the config file
                string[] lines = File.ReadAllLines(fileName);
                if (lines.Length >= 1)
                {
                    string folderPathLine = lines[0];
                    if (folderPathLine.StartsWith("FolderPath="))
                    {
                        _folderPath = folderPathLine.Substring("FolderPath=".Length);
                    }
                    else
                    {
                        MessageBox.Show("Invalid config file");
                        Close();
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Invalid config file");
                    Close();
                    return;
                }
            }


        }*/
        private void Form1_Load(object sender, EventArgs e)
        {
            string fileName = "config.ini";
            if (!File.Exists(fileName))
            {
                // Show folder browser dialog to select "portraits" folder
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "Select the 'portraits' folder";
                    dialog.ShowNewFolderButton = true;
                    while (true)
                    {
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            // check if the selected folder is named "portraits"
                            if (Path.GetFileName(dialog.SelectedPath) != "portraits")
                            {
                                MessageBox.Show("Please select a folder named 'portraits'", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                System.Media.SystemSounds.Exclamation.Play();
                            }
                            else
                            {
                                // create the config file
                                File.WriteAllText(fileName, $"FolderPath={dialog.SelectedPath}");
                                break;
                            }
                        }
                        else
                        {
                            Close();
                            return;
                        }
                    }
                }
            }
            else
            {
                // read the config file
                string[] lines = File.ReadAllLines(fileName);
                if (lines.Length >= 1)
                {
                    string folderPathLine = lines[0];
                    if (folderPathLine.StartsWith("FolderPath="))
                    {
                        string folderPath = folderPathLine.Substring("FolderPath=".Length);

                        // check if the folder is named "portraits"
                        if (Path.GetFileName(folderPath) != "portraits")
                        {
                            // Show folder browser dialog to select "portraits" folder
                            using (var dialog = new FolderBrowserDialog())
                            {
                                dialog.Description = "Select the 'portraits' folder";
                                dialog.ShowNewFolderButton = true;
                                while (true)
                                {
                                    if (dialog.ShowDialog() == DialogResult.OK)
                                    {
                                        // check if the selected folder is named "portraits"
                                        if (Path.GetFileName(dialog.SelectedPath) != "portraits")
                                        {
                                            MessageBox.Show("Please select a folder named 'portraits'", "Error",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error,
                                                MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                                            System.Media.SystemSounds.Exclamation.Play();
                                        }
                                        else
                                        {

                                            File.WriteAllText(fileName, $"FolderPath={dialog.SelectedPath}");
                                        }
                                    }
                                }                             }
                        }
                    }
                }
            }
        }



                public static void ProcessImage(string inputFilePath, string outputFolder, string newName)
        {
            int[,] dimensions = { { 256, 400, 'H' }, { 128, 200, 'L' }, { 64, 100, 'M' }, { 32, 50, 'S' }, { 16, 25, 'T' } };


            using (var originalImage = new Bitmap(inputFilePath))
            {

                for (int i = 0; i < dimensions.GetLength(0); i++)
                {

                    using (var resizedImage = originalImage.GetThumbnailImage(dimensions[i, 0], dimensions[i, 1], null, IntPtr.Zero))
                    {

                        string fileName = newName;
                        string fileExtension = ".tga";
                        string resizedFileName = $"{fileName}{(char)dimensions[i, 2]}{fileExtension}";

                        string resizedFilePath = Path.Combine(outputFolder, resizedFileName);


                        resizedImage.Save(resizedFilePath);


                        using (var img = SixLabors.ImageSharp.Image.Load<Rgb24>(resizedFilePath))
                        {
                            int width = img.Width;
                            int height = img.Height;
                            int blackHeight = (int)(height * 1.28);
                            Image<Rgb24> blackImg = new Image<Rgb24>(width, blackHeight);
                            blackImg.Mutate(x => x.BackgroundColor(SixLabors.ImageSharp.Color.Black));
                            blackImg.Mutate(x => x.DrawImage(img, SixLabors.ImageSharp.Point.Empty, 1));
                            blackImg.Save(resizedFilePath);
                        }

                        using (var image = SixLabors.ImageSharp.Image.Load(resizedFilePath))
                        {
                            var encoder = new TgaEncoder
                            {
                                BitsPerPixel = TgaBitsPerPixel.Pixel24,
                                Compression = TgaCompression.None
                            };
                            string outputFilePath = Path.Combine(outputFolder, $"{fileName}{(char)dimensions[i, 2]}.tga");

                            image.Save(outputFilePath, encoder);
                        }
                    }
                }
            }
        }

            private void UpdatePortraits()
            {
                string fileName = "config.ini";
                if (System.IO.File.Exists(fileName))
                {
                    string[] lines = System.IO.File.ReadAllLines(fileName);
                    if (lines.Length >= 2)
                    {
                        string folderPathLine = lines[1];
                        if (folderPathLine.StartsWith("FolderPath="))
                        {
                            string folderPath = folderPathLine.Substring("FolderPath=".Length);
                        // create the S3 client
                        var s3Config = new AmazonS3Config { ServiceURL = "https://s3.eu-west-3.amazonaws.com" };
                        var credentials = new BasicAWSCredentials("AKIAR2IUMCOGS3OQZVXC", "hl/mzqSrT3IX8i3gJOOFMv63FsVytQl2LfJ6GEW5");
                        using (var s3Client = new AmazonS3Client(credentials, s3Config))
                        {
                                // the bucket name
                                string bucketName = "portraitspdb";
                                ListObjectsRequest listObjectsRequest = new ListObjectsRequest
                                {
                                    BucketName = bucketName
                                };
                                ListObjectsResponse response;
                                do
                                {
                                    response = s3Client.ListObjects(listObjectsRequest);
                                    foreach (S3Object obj in response.S3Objects)
                                    {
                                        var key = obj.Key;
                                        // check if the file exists in the local folder
                                        var localFilePath = Path.Combine(folderPath, key);
                                        if (!File.Exists(localFilePath))
                                        {
                                            // download the file
                                            var request = new GetObjectRequest
                                            {
                                                BucketName = bucketName,
                                                Key = key
                                            };
                                            using (var getResponse = s3Client.GetObject(request))
                                            using (var responseStream = getResponse.ResponseStream)
                                            using (var fileStream = File.Create(localFilePath))
                                            {
                                                responseStream.CopyTo(fileStream);
                                            }
                                        }
                                    }
                                    listObjectsRequest.Marker = response.NextMarker;
                                } while (response.IsTruncated);
                            }
                        }
                    }
                }
            }
        private bool _eventRunning = false;
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (_eventRunning) return;
            _eventRunning = true;
            listBox1.Invoke(new Action(ActualizarListBox));
            _eventRunning = false;
        }

        public void ActualizarListBox()
        {
            listBox1.Items.Clear();
            string fileName = "config.ini";
            if (System.IO.File.Exists(fileName))
            {
                string[] lines = System.IO.File.ReadAllLines(fileName);
                if (lines.Length >= 2)
                {
                    string folderPathLine = lines[1];
                    if (folderPathLine.StartsWith("FolderPath="))
                    {
                        _folderPath = folderPathLine.Substring("FolderPath=".Length);
                        _fileWatcher = new FileSystemWatcher(_folderPath);
                        _fileWatcher.Changed += OnChanged;
                        _fileWatcher.Created += OnChanged;
                        _fileWatcher.Deleted += OnChanged;
                        _fileWatcher.Renamed += OnChanged;
                        _fileWatcher.EnableRaisingEvents = true;

                    }
                }
            }
            if (_folderPath == null)
            {
                _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Neverwinter Nights\portraits";
            }
            listBox1.Items.Clear();
            DirectoryInfo di = new DirectoryInfo(_folderPath);
            var tgaFiles = di.GetFiles().Where(f => f.Extension == ".tga" && f.Name.EndsWith("H.tga"));
            foreach (var item in tgaFiles)
            {
                var name = Path.GetFileNameWithoutExtension(item.FullName);
                if (name.EndsWith("H"))
                {
                    name = name.Remove(name.Length - 1);
                    if (!listBox1.Items.Contains(name))
                    {
                        listBox1.Items.Add(name);
                    }
                }
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            ActualizarListBox();
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }


        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                string selectedFile = (string)listBox1.SelectedItem;
                string filePath = Path.Combine(_folderPath, selectedFile + "H.tga");

                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        image.Save(ms, SixLabors.ImageSharp.Formats.Png.PngFormat.Instance);
                        ms.Position = 0;
                        System.Drawing.Image img = System.Drawing.Image.FromStream(ms);
                        pictureBox1.Image = img;
                    }
                }
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Formatos soportados: JPG, JPEG, PNG, ICO, BMP y GIF|*.jpg;*.jpeg;*.png;*.ico;*.bmp;*.gif";
                openFileDialog.Title = "Selecciona una imagen";

                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                    {
                        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                        {
                            string outputFolder = folderBrowserDialog.SelectedPath;
                            foreach (string inputFilePath in openFileDialog.FileNames)
                            {
                                string newName = Path.GetFileNameWithoutExtension(inputFilePath);
                                ProcessImage(inputFilePath, outputFolder, newName);
                            }
                            DialogResult result = MessageBox.Show("Se han convertido los retratos correctamente!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (result == DialogResult.OK)
                            {
                                System.Diagnostics.Process.Start(outputFolder);
                            }
                        }
                    }
                }
            }
            ActualizarListBox();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                string selectedFile = (string)listBox1.SelectedItem;
                string selectedFileNoExtension = Path.GetFileNameWithoutExtension(selectedFile);
                string[] relatedFiles = new string[] { $"{selectedFileNoExtension}H.tga", $"{selectedFileNoExtension}L.tga", $"{selectedFileNoExtension}M.tga", $"{selectedFileNoExtension}S.tga", $"{selectedFileNoExtension}T.tga" };
                foreach (string file in relatedFiles)
                {
                    string filePath = Path.Combine(_folderPath, file);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            UpdatePortraits();
        }
    }
}