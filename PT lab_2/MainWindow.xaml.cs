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
using System.Windows.Forms;
using MenuItem = System.Windows.Controls.MenuItem;
using System.Reflection.PortableExecutable;

namespace PT_lab_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open(object sender, RoutedEventArgs arg)
        {
            var FolderBrowser = new FolderBrowserDialog();
            
            if (FolderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo directory = new DirectoryInfo(FolderBrowser.SelectedPath);
                MakeTreeView(directory);
            }
        }

        private void MakeTreeView(DirectoryInfo directory)
        {
            treeView.Items.Clear();
            treeView.Items.Add(AddItems(directory));
        }


        private TreeViewItem AddItems(DirectoryInfo directory)
        {
            var root = MakeFolder(directory);

            FileInfo[] files = directory.GetFiles();
            DirectoryInfo[] directories = directory.GetDirectories();

            foreach (FileInfo file in files)
            {
                root.Items.Add(MakeFile(file));
            }
            foreach (DirectoryInfo subDirectory in directories)
            {
                root.Items.Add(AddItems(subDirectory));
            }
            return root;
        }

        private TreeViewItem MakeFile(FileInfo directory)
        {
            var item = new TreeViewItem
            {
                Header = directory.Name,
                Tag = directory.FullName
            };
            item.ContextMenu = new System.Windows.Controls.ContextMenu();
            var openButton = new MenuItem { Header = "Open" };
            var deleteButton = new MenuItem { Header = "Delete" };
            item.ContextMenu.Items.Add(openButton);
            item.ContextMenu.Items.Add(deleteButton);

            openButton.Click += new RoutedEventHandler(OpenButtonClick);
            deleteButton.Click += new RoutedEventHandler(DeleteButtonClick);
            item.Selected += new RoutedEventHandler(CheckStatus);
            return item;
        }

        private TreeViewItem MakeFolder(DirectoryInfo directory)
        {
            var item = new TreeViewItem
            {
                Header = directory.Name,
                Tag = directory.FullName
            };

            item.ContextMenu = new System.Windows.Controls.ContextMenu();
            var createButton = new MenuItem { Header = "Create" };
            var deleteButton = new MenuItem { Header = "Delete" };
            item.ContextMenu.Items.Add(createButton);
            item.ContextMenu.Items.Add(deleteButton);

            createButton.Click += new RoutedEventHandler(CreateButtonClick);
            deleteButton.Click += new RoutedEventHandler(DeleteButtonClick);
            item.Selected += new RoutedEventHandler(CheckStatus);
            return item;
        }

        private void OpenButtonClick(object sender, RoutedEventArgs arg)
        {
            TreeViewItem selectedFile = (TreeViewItem)treeView.SelectedItem;
            string path = (string)selectedFile.Tag;

            string content = File.ReadAllText(path);
            scrollViewer.Content = new TextBlock() { Text = content };
        }

        private void CreateButtonClick(object sender, RoutedEventArgs arg)
        {
            TreeViewItem selectedFolder = (TreeViewItem)treeView.SelectedItem;
            string path = (string)selectedFolder.Tag;

            Dialog creator = new Dialog(path);
            creator.ShowDialog();

            if (!creator.ifCreatedFile())
            {
                return;
            }

            string createdPath = creator.GetPath();
            string type = creator.GetType();

            if (type == "file")
            {
                FileInfo file = new FileInfo(createdPath);
                selectedFolder.Items.Add(MakeFile(file));
            }
            else if (type == "folder")
            {
                DirectoryInfo directory = new DirectoryInfo(createdPath);
                selectedFolder.Items.Add(MakeFolder(directory));
            }
        }

        private void DeleteButtonClick(object sender, RoutedEventArgs arg)
        {
            TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;
            string path = (string)selectedItem.Tag;
            FileAttributes attributes = File.GetAttributes(path);
            File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);

            if (attributes == FileAttributes.Directory)
            {
                directoryDelete(path);
            }
            else
            {
                File.Delete(path);
            }

            if (selectedItem == (TreeViewItem)treeView.Items[0])
            {
                treeView.Items.Clear();
            }
            else
            {
                TreeViewItem parent = (TreeViewItem)selectedItem.Parent;
                parent.Items.Remove(selectedItem);
            }
        }

        private void directoryDelete(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            DirectoryInfo[] directories = directory.GetDirectories();
            FileInfo[] files = directory.GetFiles();

            foreach (var subDirectory in directories)
            {
                directoryDelete(subDirectory.FullName);
            }
            foreach (var file in files)
            {
                FileAttributes attributes = File.GetAttributes(file.FullName);
                File.SetAttributes(file.FullName, attributes & ~FileAttributes.ReadOnly);
                File.Delete(file.FullName);
            }
            Directory.Delete(path);

        }

        private void CheckStatus(object sender, RoutedEventArgs arg)
        { 
            TreeViewItem selectedItem = (TreeViewItem)treeView.SelectedItem;
            string path = (string)selectedItem.Tag;

            FileAttributes attributes = File.GetAttributes(path);
            Status.Text = "RAHS: ";
            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                Status.Text += 'r';
            }
            else
            {
                Status.Text += '-';
            }
            if ((attributes & FileAttributes.Archive) == FileAttributes.Archive)
            {
                Status.Text += 'a';
            }
            else
            {
                Status.Text += '-';
            }
            if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                Status.Text += 'h';
            }
            else
            {
                Status.Text += '-';
            }
            if ((attributes & FileAttributes.System) == FileAttributes.System)
            {
                Status.Text += 's';
            }
            else
            {
                Status.Text += '-';
            }

        }

        private void Exit(object sender, RoutedEventArgs arg)
        {
            Close();
        }
    }
}
