using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{

    public partial class Form1 : Form
    {
        private string _folderPath;
        private FolderBrowserDialog folderBrowserDialog1;

        /*public static void ConvertAndResizeImage(string inputFilePath, string outputFilePath)
        {
            using (System.Drawing.Image originalImage = (System.Drawing.Image.FromFile(inputFilePath)))
            {

                using (System.Drawing.Image resizedImage = new Bitmap(originalImage, new System.Drawing.Size(256, 400)))
                {
                    using (Bitmap finalImage = new Bitmap(256, 512))
                    {
                        using (Graphics g = Graphics.FromImage(finalImage))
                        {
                            g.Clear(System.Drawing.Color.White);
                        }

                        int yOffset = 0;


                        /*using (Graphics g = Graphics.FromImage(finalImage))
                        {
                            g.DrawImage(resizedImage, new System.Drawing.Point(0, 0));
                        }
                        using (Graphics g = Graphics.FromImage(finalImage))
                        {
                            g.DrawImage(resizedImage, new System.Drawing.Point(0, 0));
                        }
                        finalImage.Save(outputFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                    }
                }
            }

            using (Image<Rgba32> finalImage = SixLabors.ImageSharp.Image.Load<Rgba32>(outputFilePath))
            {
                var tgaEncoder = new TgaEncoder();
                tgaEncoder.BitsPerPixel = TgaBitsPerPixel.Pixel24;


                finalImage.Save(outputFilePath, tgaEncoder);
            }

        }*/


        public static void ResizeImage(string filePath, string destinationFolder)
        {
            // Matriz con las dimensiones de las imágenes redimensionadas
            int[,] dimensions = { { 256, 400, 'H' }, { 128, 200, 'L' }, { 64, 100, 'M' }, { 32, 50, 'S' }, { 16, 25, 'T' } };

            // Obtener la imagen original
            using (var originalImage = new Bitmap(filePath))
            {
                // Recorrer la matriz de dimensiones
                for (int i = 0; i < dimensions.GetLength(0); i++)
                {
                    // Obtener la imagen redimensionada
                    using (var resizedImage = originalImage.GetThumbnailImage(dimensions[i, 0], dimensions[i, 1], null, IntPtr.Zero))
                    {
                        // Obtener el nombre del archivo original
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        string fileExtension = Path.GetExtension(filePath);
                        //string resizedFileName = $"{fileName}-{(char)dimensions[i, 2]}{fileExtension}";
                        string resizedFileName = $"{fileName}{(char)dimensions[i, 2]}{fileExtension}";

                        string resizedFilePath = Path.Combine(destinationFolder, resizedFileName);
                        resizedImage.Save(resizedFilePath);
                    }
                }
            }
        }
        public static void ConvertToTga(string inputFilePath, string outputFilePath)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(inputFilePath))
            {
                var encoder = new TgaEncoder();
                encoder.BitsPerPixel = TgaBitsPerPixel.Pixel24;
                encoder.Compression = TgaCompression.None;
                image.Save(outputFilePath, encoder);
            }
        }

        public static void ScaleAndDrawImage(string inputFile, string outputFile)
        {
            Image<Rgb24> img = SixLabors.ImageSharp.Image.Load<Rgb24>(inputFile);
            //img.Mutate(x => x.Resize(256, 400));
            int width = img.Width;
            int height = img.Height;
            int blackHeight = (int)(height * 1.28);
            Image<Rgb24> blackImg = new Image<Rgb24>(width, blackHeight);
            blackImg.Mutate(x => x.BackgroundColor(SixLabors.ImageSharp.Color.Black));
            blackImg.Mutate(x => x.DrawImage(img, SixLabors.ImageSharp.Point.Empty, 1));
            blackImg.Save(outputFile);
        }
        //.....................................................................................................................................
        /*public static void ProcessImage(string inputFilePath, string outputFolder)
        {
            // Matriz con las dimensiones de las imágenes redimensionadas
            int[,] dimensions = { { 256, 400, 'H' }, { 128, 200, 'L' }, { 64, 100, 'M' }, { 32, 50, 'S' }, { 16, 25, 'T' } };

            // Obtener la imagen original
            using (var originalImage = new Bitmap(inputFilePath))
            {
                // Recorrer la matriz de dimensiones
                for (int i = 0; i < dimensions.GetLength(0); i++)
                {
                    // Obtener la imagen redimensionada
                    using (var resizedImage = originalImage.GetThumbnailImage(dimensions[i, 0], dimensions[i, 1], null, IntPtr.Zero))
                    {
                        // Obtener el nombre del archivo original
                        string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                        string fileExtension = Path.GetExtension(inputFilePath);
                        //string resizedFileName = $"{fileName}-{(char)dimensions[i, 2]}{fileExtension}";
                        string resizedFileName = $"{fileName}{(char)dimensions[i, 2]}{fileExtension}";

                        string resizedFilePath = Path.Combine(outputFolder, resizedFileName);

                        // Guardar imagen redimensionada
                        resizedImage.Save(resizedFilePath);

                        // Aplicar ScaleAndDrawImage a la imagen redimensionada
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

                        // Convertir imagen redimensionada y procesada a TGA
                        using (var image = SixLabors.ImageSharp.Image.Load(resizedFilePath))
                        {
                            var encoder = new TgaEncoder();
                            encoder.BitsPerPixel = TgaBitsPerPixel.Pixel24;
                            encoder.Compression = TgaCompression.None;

                            // Obtener nombre del archivo de salida
                            string outputFileName = $"{fileName}{(char)dimensions[i, 2]}.tga";
                            string outputFilePath = Path.Combine(outputFolder, outputFileName);

                            image.Save(outputFilePath, encoder);
                        }
                    }
                }
            }
        }*/
        public static void ProcessImage(string inputFilePath, string outputFolder)
        {
            // Matriz con las dimensiones de las imágenes redimensionadas
            int[,] dimensions = { { 256, 400, 'H' }, { 128, 200, 'L' }, { 64, 100, 'M' }, { 32, 50, 'S' }, { 16, 25, 'T' } };

            // Obtener la imagen original
            using (var originalImage = new Bitmap(inputFilePath))
            {
                // Recorrer la matriz de dimensiones
                for (int i = 0; i < dimensions.GetLength(0); i++)
                {
                    // Obtener la imagen redimensionada
                    using (var resizedImage = originalImage.GetThumbnailImage(dimensions[i, 0], dimensions[i, 1], null, IntPtr.Zero))
                    {
                        // Obtener el nombre del archivo original
                        string fileName = Path.GetFileNameWithoutExtension(inputFilePath);
                        string fileExtension = Path.GetExtension(inputFilePath);
                        //string resizedFileName = $"{fileName}-{(char)dimensions[i, 2]}{fileExtension}";
                        string resizedFileName = $"{fileName}{(char)dimensions[i, 2]}{fileExtension}";

                        string resizedFilePath = Path.Combine(outputFolder, resizedFileName);

                        // Guardar imagen redimensionada
                        resizedImage.Save(resizedFilePath);

                        // Aplicar ScaleAndDrawImage a la imagen redimensionada
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

                        // Convertir imagen redimensionada y procesada a TGA
                        using (var image = SixLabors.ImageSharp.Image.Load(resizedFilePath))
                        {
                            var encoder = new TgaEncoder();
                            encoder.BitsPerPixel = TgaBitsPerPixel.Pixel24;
                            encoder.Compression = TgaCompression.None;

                            // Obtener nombre del archivo de salida
                            string outputFileName = $"{fileName}{(char)dimensions[i, 2]}.tga";
                            string outputFilePath = Path.Combine(outputFolder, outputFileName);

                            image.Save(outputFilePath, encoder);
                        }

                        // Eliminar archivo original redimensionado
                        File.Delete(resizedFilePath);
                    }
                }
            }
        }

        //.,...................................................................................................................................

        public Form1()
        {
            InitializeComponent();
            folderBrowserDialog1 = new FolderBrowserDialog();
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
                    }
                }
            }

            if (_folderPath == null)
            {
                _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Neverwinter Nights\portraits";
            }


            DirectoryInfo di = new DirectoryInfo(_folderPath);
            foreach (var item in di.GetFiles())
            {
                listBox1.Items.Add(item.Name);
            }
        
    
}

        private void button1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void ActualizarListBox()
        {
            folderBrowserDialog1 = new FolderBrowserDialog();
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
                    }
                }
            }

            if (_folderPath == null)
            {
                _folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Neverwinter Nights\portraits";
            }
            DirectoryInfo di = new DirectoryInfo(_folderPath);
            foreach (var item in di.GetFiles())
            {
                listBox1.Items.Add(item.Name);
            }
        }
        private OpenFileDialog openFileDialog1 = new OpenFileDialog();
        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos JPG|*.jpeg|Archivos PNG|*.png";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                // Obtener la ruta del archivo seleccionado
                string inputFilePath = openFileDialog1.FileName;

                // Obtener la carpeta del archivo seleccionado
                string inputFileFolder = Path.GetDirectoryName(inputFilePath);

                // Utilizar la carpeta del archivo seleccionado como ubicación de destino
                ResizeImage(inputFilePath, inputFileFolder);
            }
            else
            {
                // Mostrar un mensaje de error si no se pudo abrir el archivo
                MessageBox.Show("No se pudo abrir el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos JPG|*.jpg;*.jpeg|Archivos PNG|*.png";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string inputFilePath = openFileDialog1.FileName;
                string outputFolder = Path.GetDirectoryName(inputFilePath);
                string outputFilePath = Path.ChangeExtension(inputFilePath, ".tga");
                ConvertToTga(inputFilePath, outputFilePath);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Archivos JPG|*.jpg;*.jpeg|Archivos PNG|*.png";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;

            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                string inputFilePath = openFileDialog1.FileName;
                string outputFilePath = Path.ChangeExtension(inputFilePath, ".jpg");
                ScaleAndDrawImage(inputFilePath, inputFilePath);
            }
        }
        private static string ReadConfigFile(string key)
        {
            // Abre el archivo de configuración y lo lee línea a línea
            using (StreamReader reader = new StreamReader("config.ini"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Si la línea comienza con el valor de la clave especificada, devuelve el valor que hay después del signo igual
                    if (line.StartsWith(key + "="))
                    {
                        return line.Substring(key.Length + 1);
                    }
                }
            }

            // Si no se encontró la clave en el archivo, devuelve null
            return null;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string inputFilePath = openFileDialog.FileName;
                string outputFolder = Path.GetDirectoryName(inputFilePath);

                ProcessImage(inputFilePath, outputFolder);
            }

        }
    }
    
      
}