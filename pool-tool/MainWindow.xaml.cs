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


        public TableSize tableSize = new TableSize();
        private void Window_Loaded(object sender, RoutedEventArgs e) {
            // LOAD TABLE FROM FILE
            var line = "";
            var tableFile = new StreamReader("./Data/table-size.txt");
            while ((line = tableFile.ReadLine()) != null) {
                var strValue = line.Split('_');
                double value = 0;
                double.TryParse(strValue[1], out value);

                if (strValue[0] == "height") {
                    tableSize.height = value;
                }
                else if (strValue[0] == "width") {
                    tableSize.width = value;
                }
                else if (strValue[0] == "top") {
                    tableSize.top = value;
                }
                else if (strValue[0] == "left") {
                    tableSize.left = value;
                }
                else if (strValue[0] == "right") {
                    tableSize.right = value;
                }
                else if (strValue[0] == "bottom") {
                    tableSize.bottom = value;
                }
                else {
                    tableSize.ball = value;
                }
            }
            cvTable.Height = tableSize.height;
            cvTable.Width = tableSize.width;
            bdTable.Margin = new Thickness(tableSize.left, tableSize.top, tableSize.right, tableSize.bottom);

        }

        #region MENU DRAG
        private object movingObject;
        private double firstXPos, firstYPos;

        private void menu_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            // In this event, we get the current mouse position on the control to use it in the MouseMove event.
            StackPanel stack = sender as StackPanel;
            Canvas canvas = stack.Parent as Canvas;

            firstXPos = e.GetPosition(stack).X;
            firstYPos = e.GetPosition(stack).Y;

            movingObject = sender;

            // Put the image currently being dragged on top of the others
            int top = Canvas.GetZIndex(stack);
            foreach (StackPanel child in canvas.Children)
                if (top < Canvas.GetZIndex(child))
                    top = Canvas.GetZIndex(child);
            Canvas.SetZIndex(stack, top + 1);
        }

        private void menu_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && sender == movingObject) {
                StackPanel stack = sender as StackPanel;
                Canvas canvas = stack.Parent as Canvas;

                double newLeft = e.GetPosition(canvas).X - firstXPos - canvas.Margin.Left;
                // newLeft inside canvas right-border?
                if (newLeft > canvas.Margin.Left + canvas.ActualWidth - stack.ActualWidth)
                    newLeft = canvas.Margin.Left + canvas.ActualWidth - stack.ActualWidth;
                // newLeft inside canvas left-border?
                else if (newLeft < canvas.Margin.Left)
                    newLeft = canvas.Margin.Left;
                stack.SetValue(Canvas.LeftProperty, newLeft);

                double newTop = e.GetPosition(canvas).Y - firstYPos - canvas.Margin.Top;
                // newTop inside canvas bottom-border?
                if (newTop > canvas.Margin.Top + canvas.ActualHeight - stack.ActualHeight)
                    newTop = canvas.Margin.Top + canvas.ActualHeight - stack.ActualHeight;
                // newTop inside canvas top-border?
                else if (newTop < canvas.Margin.Top)
                    newTop = canvas.Margin.Top;
                stack.SetValue(Canvas.TopProperty, newTop);
            }
        }

        private void menu_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            StackPanel stack = sender as StackPanel;
            Canvas canvas = stack.Parent as Canvas;

            movingObject = null;

            // Put the image currently being dragged on top of the others
            int top = Canvas.GetZIndex(stack);
            foreach (StackPanel child in canvas.Children)
                if (top > Canvas.GetZIndex(child))
                    top = Canvas.GetZIndex(child);
            Canvas.SetZIndex(stack, top + 1);
        }
        #endregion

        #region BUTTON SETTING
        private void btnSetting_Click(object sender, RoutedEventArgs e) {
            var oldTable = new TableSize() {
                height = tableSize.height,
                width = tableSize.width,
                top = tableSize.top,
                left = tableSize.left,
                right = tableSize.right,
                bottom = tableSize.bottom,
                ball = tableSize.ball
            };
            var settingScreen = new SettingWindow(tableSize);
            settingScreen.SizeChange += SettingScreen_SizeChange;
            if (settingScreen.ShowDialog() == true) {
                using (StreamWriter writer = new StreamWriter("./Data/table-size.txt")) {
                    writer.WriteLine($"height_{tableSize.height}");
                    writer.WriteLine($"width_{tableSize.width}");
                    writer.WriteLine($"top_{tableSize.top}");
                    writer.WriteLine($"left_{tableSize.left}");
                    writer.WriteLine($"right_{tableSize.right}");
                    writer.WriteLine($"ball_{tableSize.ball}");
                    MessageBox.Show("Saved!", "Table Config", MessageBoxButton.OK);
                }
            }
            else {
                tableSize = oldTable;
                cvTable.Height = tableSize.height;
                cvTable.Width = tableSize.width;
                bdTable.Margin = new Thickness(tableSize.left, tableSize.top, tableSize.right, tableSize.bottom);
            }
        }

        private void SettingScreen_SizeChange(TableSize newTable) {
            tableSize = newTable;
            cvTable.Height = newTable.height;
            cvTable.Width = newTable.width;
            bdTable.Margin = new Thickness(newTable.left, newTable.top, newTable.right, newTable.bottom);

            // miss ball
        }
        #endregion
    }

}
