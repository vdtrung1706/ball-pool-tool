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
using System.Windows.Shapes;
using pool_tool.Class;

namespace pool_tool.Modules {
   /// <summary>
   /// Interaction logic for SettingWindow.xaml
   /// </summary>
   public partial class SettingWindow : Window {
      readonly TableConfig oldTable = new TableConfig();
      readonly TableConfig tableConfig;
      public SettingWindow(TableConfig oldTable) {
         InitializeComponent();

         this.oldTable = oldTable;
         tableConfig = new TableConfig() {
            height = oldTable.height,
            width = oldTable.width,
            top = oldTable.top,
            left = oldTable.left,
            ballSize = oldTable.ballSize
         };
      }

      public delegate void TableChangeDelegate(TableConfig newTable, string mess = "");
      public event TableChangeDelegate SizeChange;

      private void Window_Loaded(object sender, RoutedEventArgs e) {
         tbBall.Text = tableConfig.ballSize.ToString();
      }

      private void tbBall_TextChanged(object sender, TextChangedEventArgs e) {
         double ball;
         var check = double.TryParse(tbBall.Text, out ball);
         tableConfig.ballSize = check == true ? ball : 0;
         SizeChange?.Invoke(tableConfig);
      }

      private void btnSave_Click(object sender, RoutedEventArgs e) {
         SizeChange?.Invoke(tableConfig, "save");
         Close();
      }

      private void btnCancel_Click(object sender, RoutedEventArgs e) {
         SizeChange?.Invoke(oldTable, "cancel");
         Close();
      }
   }
}
