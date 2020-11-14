using System;
using System.Collections;
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
using System.IO;
using pool_tool.Modules;
using pool_tool.Class;

namespace pool_tool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }


        public TableConfig tableConfig = new TableConfig();
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            #region READ FILE SETTING
            var line = "";
            var tableFile = new StreamReader("./Data/table-config.txt");
            while ((line = tableFile.ReadLine()) != null) {
                var strValue = line.Split('_');
                double value = 0;
                double.TryParse(strValue[1], out value);

                if (strValue[0] == "height") {
                    tableConfig.height = value;
                }
                else if (strValue[0] == "width") {
                    tableConfig.width = value;
                }
                else if (strValue[0] == "top") {
                    tableConfig.top = value;
                }
                else if (strValue[0] == "left") {
                    tableConfig.left = value;
                }
                else if (strValue[0] == "right") {
                    tableConfig.right = value;
                }
                else if (strValue[0] == "bottom") {
                    tableConfig.bottom = value;
                }
                else {
                    tableConfig.ballSize = value;
                }
            }
            brdTable.DataContext = tableConfig;
            brdTable.Margin = new Thickness(tableConfig.left, tableConfig.top, tableConfig.right, tableConfig.bottom);
            ellipse1.DataContext = ellipse2.DataContext = tableConfig;
            #endregion
        }

        #region MENU
        #endregion

        #region BUTTON SETTING
        private void btnSetting_Click(object sender, RoutedEventArgs e) {
            var oldTable = new TableConfig() {
                height = tableConfig.height,
                width = tableConfig.width,
                top = tableConfig.top,
                left = tableConfig.left,
                right = tableConfig.right,
                bottom = tableConfig.bottom,
                ballSize = tableConfig.ballSize
            };
            var settingScreen = new SettingWindow(tableConfig);
            settingScreen.SizeChange += SettingScreen_SizeChange;
            if (settingScreen.ShowDialog() == true) {
                using (StreamWriter writer = new StreamWriter("./Data/table-size.txt")) {
                    writer.WriteLine($"height_{tableConfig.height}");
                    writer.WriteLine($"width_{tableConfig.width}");
                    writer.WriteLine($"top_{tableConfig.top}");
                    writer.WriteLine($"left_{tableConfig.left}");
                    writer.WriteLine($"right_{tableConfig.right}");
                    writer.WriteLine($"ball_{tableConfig.ballSize}");
                    MessageBox.Show("Saved!", "Table Config", MessageBoxButton.OK);
                }
            }
            else {
                tableConfig = oldTable;
                brdTable.Height = tableConfig.height;
                brdTable.Width = tableConfig.width;
                brdTable.Margin = new Thickness(tableConfig.left, tableConfig.top, tableConfig.right, tableConfig.bottom);
            }
        }


        #region TWO BALLS
        
        #endregion
        private void SettingScreen_SizeChange(TableConfig newTable) {
            tableConfig = newTable;
            brdTable.DataContext = tableConfig;
            brdTable.Margin = new Thickness(tableConfig.left, tableConfig.top, tableConfig.right, tableConfig.bottom);
            ellipse1.DataContext = ellipse2.DataContext = tableConfig;
            // miss ball
        }
        #endregion
    }

}
