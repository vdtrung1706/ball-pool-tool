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
using System.Windows.Shapes;
using pool_tool.Class;

namespace pool_tool.Modules {
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window {
        TableConfig tableConfig;
        public SettingWindow(TableConfig curTable) {
            InitializeComponent();
            tableConfig = new TableConfig() {
                height = curTable.height,
                width = curTable.width,
                top = curTable.top,
                left = curTable.left,
                right= curTable.right,
                bottom = curTable.bottom,
                ballSize = curTable.ballSize
            };
        }

        public delegate void TableChangeDelegate (TableConfig newTable);
        public event TableChangeDelegate SizeChange;

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            tbHeight.Text = tableConfig.height.ToString();
            tbWidth.Text = tableConfig.width.ToString();
            tbTop.Text = tableConfig.top.ToString();
            tbLeft.Text = tableConfig.left.ToString();
            tbRight.Text = tableConfig.right.ToString();
            tbBottom.Text = tableConfig.bottom.ToString();
            tbBall.Text = tableConfig.ballSize.ToString();
        }

        private void tbHeight_TextChanged(object sender, TextChangedEventArgs e) {
            double height;
            var check = double.TryParse(tbHeight.Text, out height);
            tableConfig.height = check == true ? height : 0;
            SizeChange?.Invoke(tableConfig);
        }

        private void tbWidth_TextChanged(object sender, TextChangedEventArgs e) {
            double width;
            var check = double.TryParse(tbWidth.Text, out width);
            tableConfig.width = check == true ? width : 0;
            SizeChange?.Invoke(tableConfig);
        }

        private void tbTop_TextChanged(object sender, TextChangedEventArgs e) {
            double top;
            var check = double.TryParse(tbTop.Text, out top);
            tableConfig.top = check == true ? top : 0;
            SizeChange?.Invoke(tableConfig);
        }

        private void tbLeft_TextChanged(object sender, TextChangedEventArgs e) {
            double left;
            var check = double.TryParse(tbLeft.Text, out left);
            tableConfig.left = check == true ? left : 0;
            SizeChange?.Invoke(tableConfig);
        }

        private void tbRight_TextChanged(object sender, TextChangedEventArgs e) {
            double right;
            var check = double.TryParse(tbRight.Text, out right);
            tableConfig.right = check == true ? right : 0;
            SizeChange?.Invoke(tableConfig);
        }

        private void tbBottom_TextChanged(object sender, TextChangedEventArgs e) {
            double bottom;
            var check = double.TryParse(tbBottom.Text, out bottom);
            tableConfig.bottom = check == true ? bottom : 0;
            SizeChange?.Invoke(tableConfig);
        }

        private void tbBall_TextChanged(object sender, TextChangedEventArgs e) {
            double ball;
            var check = double.TryParse(tbBall.Text, out ball);
            tableConfig.ballSize = check == true ? ball : 0;
            SizeChange?.Invoke(tableConfig);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
