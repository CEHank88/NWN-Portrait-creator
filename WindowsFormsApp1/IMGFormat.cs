using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
#pragma warning disable CS0169 // El campo 'Form1._folderPath' nunca se usa
        private readonly string _folderPath;
#pragma warning restore CS0169 // El campo 'Form1._folderPath' nunca se usa


        public Form1()
        {
            InitializeComponent();

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
                            DialogResult result = MessageBox.Show("Se han convertido los retratos correctamente! Haga click en aceptar para abrir la ubicación donde se encuentran los portraits convertidos. \n \nNota: Los archivos se sobreescribirán automáticamente si ya existe uno con el mismo nombre.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (result == DialogResult.OK)
                            {
                                System.Diagnostics.Process.Start(outputFolder);
                            }
                        }
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }

}