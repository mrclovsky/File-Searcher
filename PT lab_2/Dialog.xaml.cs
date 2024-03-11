using System;
using System.Collections.Generic;
using System.IO;
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
using System.Text.RegularExpressions;

namespace PT_lab_2
{
    /// <summary>
    /// Logika interakcji dla klasy Dialog.xaml
    /// </summary>
    public partial class Dialog : Window
    {
        private string name;
        private string path;
        private string type;
        private bool created;
        public Dialog(string path)
        {
            InitializeComponent();
            this.path = path;
            this.type = "";
            created = false;
        }

        public string GetPath()
        {
            return path;
        }

        public bool ifCreatedFile()
        {
            return created;
        }

        public string GetType()
        {
            return type;
        }

        public void OkButtonClick(object sender, RoutedEventArgs arg)
        {
            bool isFile = (bool)fileButton.IsChecked;
            bool isDirectory = (bool)directoryButton.IsChecked;
            FileAttributes attributes = FileAttributes.Normal;
            bool isReadOnly = (bool)readOnlyCheckBox.IsChecked;
            bool isArchive = (bool)archiveCheckBox.IsChecked;
            bool isHidden = (bool)hiddenCheckBox.IsChecked;
            bool isSystem = (bool)systemCheckBox.IsChecked;

            name = fileName.Text;
            path = path + "\\" + name;
            if (isFile)
            {
                type = "file";
            }
            else
            {
                type = "folder";
            }


            if (!isFile && !isDirectory)
            {
                System.Windows.MessageBox.Show("Specify [file/directory]", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (isFile && !Regex.IsMatch(fileName.Text, "^[a-zA-Z0-9_~-]{1,8}\\.(txt|php|html)$"))
            {
                System.Windows.MessageBox.Show("Invalid name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if (isReadOnly)
                {
                    attributes |= FileAttributes.ReadOnly;
                }
                if (isArchive)
                {
                    attributes |= FileAttributes.Archive;
                }
                if (isHidden)
                {
                    attributes |= FileAttributes.Hidden;
                }
                if (isSystem)
                {
                    attributes |= FileAttributes.System;
                }

                if (isFile)
                {
                    File.Create(path);
                }
                else if (isDirectory)
                {
                    Directory.CreateDirectory(path);
                }
                File.SetAttributes(path, attributes);
                created = true;
                Close();
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs arg)
        {
            Close();
        }

    }
}
