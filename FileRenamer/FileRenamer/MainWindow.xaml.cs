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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FileRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataObject dataObject;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DataObject();
            dataObject = DataContext as DataObject;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            ListBox_ImageData.SelectionChanged -= ListBox_ImageData_SelectionChanged;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                dataObject.FileList.Clear();
                dataObject.PropList.Clear();

                bool firstFile = true;
                foreach (string filePath in openFileDialog.FileNames)
                {
                    File newFile = new File(filePath);
                    dataObject.FileList.Add(newFile);

                    // Load image properties
                    firstFile = LoadImageProperties(filePath, firstFile);
                }
            }
            ListBox_ImageData.SelectionChanged += ListBox_ImageData_SelectionChanged;
        }

        private bool LoadImageProperties(string filePath, bool firstFile)
        {
            using (Bitmap image = new Bitmap(filePath))
            {
                if (firstFile)
                {
                    foreach (PropertyItem pi in image.PropertyItems)
                    {
                        Prop p = new Prop(pi);
                        dataObject.PropList.Add(p);
                    }
                    firstFile = false;
                }
                else
                {
                    List<Prop> tempPropList = new List<Prop>(dataObject.PropList);
                    foreach (Prop p in tempPropList)
                    {
                        if (dataObject.PropList.Count == 0) break;
                        if (!image.PropertyIdList.Contains(p.Id)) dataObject.PropList.Remove(p);
                    }
                }
            }
            
            return firstFile;
        }

        private System.Drawing.Image LoadImage(string filePath)
        {
            System.Drawing.Image image;
            using (Bitmap bm = new Bitmap(filePath))
            {
                image = bm;
            }
            return image;
        }

        private void ListBox_ImageData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox_ImageData.SelectionChanged -= ListBox_ImageData_SelectionChanged;
            // signature of a prop (id, name and type, not including value and len)
            Prop prop = dataObject.PropList.Single(x => x.Name == ((Prop)ListBox_ImageData.SelectedValue).Name);
            foreach (File file in dataObject.FileList)
            {
                PropertyItem propItem;
                using (Bitmap bm = new Bitmap(file.FilePath))
                {
                    propItem = bm.GetPropertyItem(prop.Id);
                }
                
                // Actual prop of the current image file (id, name, type, value and len)
                byte[] propData = propItem.Value;
                int propDataLength = propItem.Len;

                string result = "";
                int num_items, item_size;
                string newFileName = "";

                switch (prop.Type)
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

                file.NewFileName = newFileName + file.FileExtension;
            }

            ListBox_ImageData.SelectionChanged += ListBox_ImageData_SelectionChanged;

        }

        private void Button_Rename_Click(object sender, RoutedEventArgs e)
        {
            List<string> ErrorMessages = new List<string>();
            foreach (File f in dataObject.FileList)
            {
                try
                {
                    System.IO.File.Move(f.FilePath, f.FileDirectory + @"\" + f.NewFileName);
                }
                catch (Exception ex)
                {
                    ErrorMessages.Add(ex.Message);
                }
                
            }

            if (ErrorMessages.Count == 0)
            {
                MessageBox.Show("Files successfully renamed");
                dataObject.FileList.Clear();
                ListBox_ImageData.SelectionChanged -= ListBox_ImageData_SelectionChanged;
                dataObject.PropList.Clear();
                ListBox_ImageData.SelectionChanged += ListBox_ImageData_SelectionChanged;
            } else
            {
                string error = "";
                foreach(string errorMessage in ErrorMessages)
                {
                    error += errorMessage + Environment.NewLine;
                }
                MessageBox.Show(error);
            }
        }

        private void Button_RemoveFile_Click(object sender, RoutedEventArgs e)
        {
            List<File> selectedFiles = new List<File>();
            foreach (File f in ListBox_Files.SelectedItems)
            {
                selectedFiles.Add(f);
            }

            foreach (File f in selectedFiles)
            {
                dataObject.FileList.Remove(f);
            }

            ListBox_ImageData.SelectionChanged -= ListBox_ImageData_SelectionChanged;
            dataObject.PropList.Clear();
            bool firstFile = true;
            foreach (File f in dataObject.FileList)
            {
                // Load image properties
                firstFile = LoadImageProperties(f.FilePath, firstFile);
            }
            ListBox_ImageData.SelectionChanged += ListBox_ImageData_SelectionChanged;
        }
    }
}
