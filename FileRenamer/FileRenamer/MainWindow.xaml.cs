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
using System.Drawing.Imaging;

namespace FileRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, string> fileName_filePath;
        public List<Prop> currentPropList;
        public struct Prop
        {
            public int Id;
            public string Name;
            public Utilities.ExifPropertyDataTypes Type;

            public Prop(int id, string name, Utilities.ExifPropertyDataTypes type)
            {
                Id = id;
                Name = name;
                Type = type;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            fileName_filePath = new Dictionary<string, string>();
            //populateListBoxImageData();

            //retrieveEXIFDate();
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
            string poo = image.PropertyItems.ToString();
            MessageBox.Show(poo);

            //var propertyFound = false;
            //foreach (var prop in image.PropertyItems)
            //{
            //    if (prop.Id == 0x0112) propertyFound = true;
            //}
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                bool firstFile = true;
                currentPropList = new List<Prop>();

                foreach (string filePath in openFileDialog.FileNames)
                {
                    // get list of file names 
                    string fileName = System.IO.Path.GetFileName(filePath);
                    ListBox_Files.Items.Add(fileName);
                    fileName_filePath.Add(fileName,filePath);

                    // 
                    System.Drawing.Image image = System.Drawing.Image.FromFile(filePath);
                    if (firstFile)
                    {
                        foreach (PropertyItem pi in image.PropertyItems)
                        {
                            string propName = Enum.GetName(typeof(Utilities.ExifPropertyTypes), pi.Id);
                            Utilities.ExifPropertyDataTypes propType = (Utilities.ExifPropertyDataTypes)pi.Type;
                            Prop p = new Prop(pi.Id, propName, propType);
                            currentPropList.Add(p);
                        }
                        firstFile = false;
                    }
                    else
                    {
                        List<Prop> tempPropList = new List<Prop>();
                        currentPropList.All(x =>
                        {
                            if (tempPropList.Count == image.PropertyIdList.Length) return false;
                            if (image.PropertyIdList.Contains(x.Id)) tempPropList.Add(x);
                            return true;
                        });
                        currentPropList = new List<Prop>(tempPropList);
                    }

                    ListBox_Preview.Items.Add(fileName);
                }

                ListBox_ImageData.ItemsSource = currentPropList.Select(x => x.Name);

            }

        }

        private void ListBox_ImageData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Prop p = currentPropList.Find(x => x.Name == ListBox_ImageData.SelectedValue.ToString());
            ListBox_Preview.Items.Clear();
            foreach (string fileName in ListBox_Files.Items)
            {
                System.Drawing.Image image = System.Drawing.Image.FromFile(fileName_filePath[fileName]);
                PropertyItem propItem = image.GetPropertyItem(p.Id);

                byte[] propData = propItem.Value;
                int propDataLength = propItem.Len;

                string result = "";
                int num_items, item_size;
                string newFileName = "";
                // rename fileName
                switch (p.Type)
                {
                    case Utilities.ExifPropertyDataTypes.ByteArray:
                    case Utilities.ExifPropertyDataTypes.UByteArray:
                        newFileName =
                            BitConverter.ToString(propData);
                        break;

                    case Utilities.ExifPropertyDataTypes.String:
                        newFileName = Encoding.UTF8.GetString(
                            propData, 0, propDataLength - 1);
                        break;

                    case Utilities.ExifPropertyDataTypes.UShortArray:
                        result = "";
                        item_size = 2;
                        num_items = propDataLength / item_size;
                        for (int i = 0; i < num_items; i++)
                        {
                            ushort value = BitConverter.ToUInt16(
                                propData, i * item_size);
                            result += ", " + value.ToString();
                        }
                        if (result.Length > 0) result = result.Substring(2);
                        newFileName = "[" + result + "]";
                        break;

                    case Utilities.ExifPropertyDataTypes.ULongArray:
                        result = "";
                        item_size = 4;
                        num_items = propDataLength / item_size;
                        for (int i = 0; i < num_items; i++)
                        {
                            uint value = BitConverter.ToUInt32(
                                propData, i * item_size);
                            result += ", " + value.ToString();
                        }
                        if (result.Length > 0) result = result.Substring(2);
                        newFileName = "[" + result + "]";
                        break;

                    case Utilities.ExifPropertyDataTypes.ULongFractionArray:
                        result = "";
                        item_size = 8;
                        num_items = propDataLength / item_size;
                        for (int i = 0; i < num_items; i++)
                        {
                            uint numerator = BitConverter.ToUInt32(
                                propData, i * item_size);
                            uint denominator = BitConverter.ToUInt32(
                                propData,
                                i * item_size + item_size / 2);
                            result += ", " + numerator.ToString() +
                                "/" + denominator.ToString();
                        }
                        if (result.Length > 0) result = result.Substring(2);
                        newFileName = "[" + result + "]";
                        break;

                    case Utilities.ExifPropertyDataTypes.LongArray:
                        result = "";
                        item_size = 4;
                        num_items = propDataLength / item_size;
                        for (int i = 0; i < num_items; i++)
                        {
                            int value = BitConverter.ToInt32(
                                propData, i * item_size);
                            result += ", " + value.ToString();
                        }
                        if (result.Length > 0) result = result.Substring(2);
                        newFileName = "[" + result + "]";
                        break;

                    case Utilities.ExifPropertyDataTypes.LongFractionArray:
                        result = "";
                        item_size = 8;
                        num_items = propDataLength / item_size;
                        for (int i = 0; i < num_items; i++)
                        {
                            int numerator = BitConverter.ToInt32(
                                propData, i * item_size);
                            int denominator = BitConverter.ToInt32(
                                propData,
                                i * item_size + item_size / 2);
                            result += ", " + numerator.ToString() +
                                "/" + denominator.ToString();
                        }
                        if (result.Length > 0) result = result.Substring(2);
                        newFileName = "[" + result + "]";
                        break;
                }

                ListBox_Preview.Items.Add(newFileName);
            }
        }

        private void Button_Rename_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
