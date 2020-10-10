using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace homerworker
{
    /// <summary>
    /// Window1.xaml 的互動邏輯
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void pathBtn_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            System.Windows.Forms.DialogResult dl = folderBrowserDialog1.ShowDialog();
            if (System.Windows.Forms.DialogResult.OK==dl)
            {
                homerworker.Properties.Settings.Default.directoryPath = folderBrowserDialog1.SelectedPath;
                homerworker.Properties.Settings.Default.Save();
            }


           
        }
    }
}
