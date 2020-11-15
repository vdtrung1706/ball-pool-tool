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
using System.Windows.Media.Animation;
using Microsoft.Win32;
using System.Windows.Controls.Primitives;

namespace pool_tool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private TableConfig tableConfig = new TableConfig();

        private string btnCurrentPressed = "";
        private void updateDirectLine(string btnName) {
            if (btnName == "") {
                return;
            }

            var pointE1 = ellipse1.TransformToAncestor(cvtable)
                                        .Transform(new Point(0, 0));

            var pointE2 = ellipse2.TransformToAncestor(cvtable)
                                       .Transform(new Point(0, 0));

            pointE1 = new Point(pointE1.X + ellipse1.Height / 2, pointE1.Y + ellipse1.Height / 2);
            pointE2 = new Point(pointE2.X + ellipse2.Height / 2, pointE2.Y + ellipse2.Height / 2);

            var crossPoint = new Point(0, 0);

            line1.X1 = pointE1.X;
            line1.Y1 = pointE1.Y;

            line2.X2 = pointE2.X;
            line2.Y2 = pointE2.Y;

            if (btnName == btnLine.Name) {
                return;
            }
            else if (btnName == btnDownArrow.Name) {
                crossPoint = Helper.pointsIntersection(pointE1, new Point(pointE2.X, cvtable.ActualHeight),
                    pointE2, new Point(pointE1.X, cvtable.ActualHeight));
                line1.Y2 = line2.Y1 = cvtable.ActualHeight;
                line1.X2 = line2.X1 = crossPoint.X;
            }
            else if (btnName == btnUpArrow.Name) {
                crossPoint = Helper.pointsIntersection(pointE1, new Point(pointE2.X, 0),
                                   pointE2, new Point(pointE1.X, 0));
                line1.Y2 = line2.Y1 = 0;
                line1.X2 = line2.X1= crossPoint.X;
            }
            else if (btnName == btnLeftArrow.Name) {
                crossPoint = Helper.pointsIntersection(pointE1, new Point(0, pointE2.Y),
                                   pointE2, new Point(0, pointE1.Y));
                line1.X2 = line2.X1 = 0;
                line1.Y2 = line2.Y1 = crossPoint.Y;
            }
            else if (btnName == btnRightArrow.Name) {
                crossPoint = Helper.pointsIntersection(pointE1, new Point(cvtable.ActualWidth, pointE2.Y),
                                   pointE2, new Point(cvtable.ActualWidth, pointE1.Y));
                line1.X2 = line2.X1 = cvtable.ActualWidth;
                line1.Y2 = line2.Y1 = crossPoint.Y;
            }

        }

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


        private void SettingScreen_SizeChange(TableConfig newTable) {
            tableConfig = newTable;
            brdTable.DataContext = tableConfig;
            brdTable.Margin = new Thickness(tableConfig.left, tableConfig.top, tableConfig.right, tableConfig.bottom);
            ellipse1.DataContext = ellipse2.DataContext = tableConfig;
        }

        #endregion


        #region MENU BUTTON
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
                using (StreamWriter writer = new StreamWriter("./Data/table-config.txt")) {
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


        private void btnBalls_Click(object sender, RoutedEventArgs e) {
            if (ellipse1.Visibility == Visibility.Hidden) {
                ellipse1.Visibility = ellipse2.Visibility = Visibility.Visible;
                ellipse1.IsEnabled = ellipse2.IsEnabled = true;

                // set position of two balls
                Canvas.SetLeft(ellipse1, (brdTable.ActualWidth / 2 - ellipse1.ActualWidth));
                Canvas.SetTop(ellipse1, brdTable.ActualHeight / 2 - ellipse1.ActualHeight / 2);

                Canvas.SetLeft(ellipse2, (brdTable.ActualWidth / 2) + 1);
                Canvas.SetTop(ellipse2, brdTable.ActualHeight / 2 - ellipse2.ActualHeight / 2);
            }
            else {
                ellipse1.Visibility = ellipse2.Visibility = Visibility.Hidden;
                ellipse1.IsEnabled = ellipse2.IsEnabled = false;
            }

        }

        #endregion

        #region TWO BALLS

        private void ellipse1_MouseDown(object sender, MouseButtonEventArgs e) {
            var ellipse = sender as Ellipse;
            if (ellipse != null) {
                if (e.LeftButton != MouseButtonState.Pressed)
                    return;

                ellipse.CaptureMouse();
            }
        }

        private void ellipse1_MouseUp(object sender, MouseButtonEventArgs e) {
            var ellipse = sender as Ellipse;
            if (ellipse != null) {
                if (!ellipse.IsMouseCaptured)
                    return;
                ellipse.ReleaseMouseCapture();
            }
        }


        private void ellipse1_MouseMove(object sender, MouseEventArgs e) {
            var ellipse = sender as Ellipse;
            if (ellipse != null) {
                if (e.LeftButton != MouseButtonState.Pressed || !ellipse.IsMouseCaptured)
                    return;

                MoveEllipse(e, sender as Ellipse);
            }
        }

        private void MoveEllipse(MouseEventArgs e, Ellipse ellipse) {
            var pos = e.GetPosition(cvtable);
            Canvas.SetLeft(ellipse, pos.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, pos.Y - ellipse.Height / 2);
            updateDirectLine(btnCurrentPressed);
        }
        #endregion

        private void btnDownArrow_Click(object sender, RoutedEventArgs e) {
            btnCurrentPressed = btnDownArrow.Name;
            updateDirectLine(btnCurrentPressed);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e) {
            ellipse1.Visibility = ellipse2.Visibility = Visibility.Hidden;
            ellipse1.IsEnabled = ellipse2.IsEnabled = false;

            line1.X1 = line1.Y1 = line1.X2 = line1.Y2 = line2.X2 = line2.Y2 = line2.X1 = line2.Y1 = 0;

            btnCurrentPressed = "";
        }

        private void btnUpArrow_Click(object sender, RoutedEventArgs e) {
            btnCurrentPressed = btnUpArrow.Name;
            updateDirectLine(btnCurrentPressed);
        }

        private void btnRightArrow_Click(object sender, RoutedEventArgs e) {
            btnCurrentPressed = btnRightArrow.Name;
            updateDirectLine(btnCurrentPressed);
        }

        private void btnLeftArrow_Click(object sender, RoutedEventArgs e) {
            btnCurrentPressed = btnLeftArrow.Name;
            updateDirectLine(btnCurrentPressed);
        }
    }

}
