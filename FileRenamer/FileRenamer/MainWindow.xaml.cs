using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Drawing;

namespace FileRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //populateListBoxImageData();

            retrieveEXIFDate();
        }

        private void populateListBoxImageData()
        {
            ListBox_ImageData.Items.Add("File Size");
            ListBox_ImageData.Items.Add("File Type");
            ListBox_ImageData.Items.Add("MIME Type");
            ListBox_ImageData.Items.Add("Image Width");
            ListBox_ImageData.Items.Add("Image Height");
            ListBox_ImageData.Items.Add("Encoding Process");
            ListBox_ImageData.Items.Add("Bits Per Sample");
            ListBox_ImageData.Items.Add("Color Components");
            ListBox_ImageData.Items.Add("X Resolution");
            ListBox_ImageData.Items.Add("Y Resolution");
            ListBox_ImageData.Items.Add("YCbCr Sub Sampling");

        }


        private void retrieveEXIFDate()
        {
            System.Drawing.Image image = System.Drawing.Image.FromFile(@"C:\Users\Lcocarell2816\Desktop\Photos\120px-10_Brown_Titlark.jpg");
            var PropertyTagGridSize = Encoding.UTF8.GetString(image.GetPropertyItem(0x5091).Value);
            ListBox_ImageData.Items.Add(PropertyTagGridSize);
            string poo =  image.PropertyItems.ToString();
            MessageBox.Show(poo);
            //var propertyFound = false;
            //foreach (var prop in image.PropertyItems)
            //{
            //    if (prop.Id == 0x0112) propertyFound = true;
            //}
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                    ListBox_Files.Items.Add(System.IO.Path.GetFileName(filename));
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        class Button
        {
            public string ButtonContent { get; set; }
            public string ButtonID { get; set; }
        }

        private void button_Deselect_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Files.UnselectAll();
        }

        private void button_SelectAll_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Files.SelectAll();
        }

        private void button_ClearFiles_Click(object sender, RoutedEventArgs e)
        {
            ListBox_Files.Items.Clear();
        }

        private void button_Basic_Click(object sender, RoutedEventArgs e)
        {
            if (button_Basic.Content.ToString() == "Basic")
            {

                button_Basic.Content = "Advanced";
            }
            else
            {
                button_Basic.Content = "Basic";
            }
        }
    }
}
