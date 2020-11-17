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
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using MaterialDesignThemes.Wpf;

namespace pool_tool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private TableConfig tableConfig = new TableConfig();

        private string btnCurPressed = "";

        private void updateDirectLine() {
            // Remove some lines add when using draw line
            Helper.cleanCanvasChild(cvTable, 6, cvTable.Children.Count - 6);

            if (btnCurPressed == "" || !ellipse1.IsEnabled || !ellipse2.IsEnabled || btnCurPressed == btnLongLine.Name) {
                return;
            }

            var e1Pos = Helper.getEllipsePos(ellipse1, cvTable);
            var e2Pos = Helper.getEllipsePos(ellipse2, cvTable);


            line1.X1 = e1Pos.X;
            line1.Y1 = e1Pos.Y;

            line2.X2 = e2Pos.X;
            line2.Y2 = e2Pos.Y;

            Point crossPos;
            if (btnCurPressed == btnDownArrow.Name) {
                crossPos = Helper.intersectionPos(e1Pos, new Point(e2Pos.X, cvTable.ActualHeight),
                                                  e2Pos, new Point(e1Pos.X, cvTable.ActualHeight));
                line1.Y2 = line2.Y1 = cvTable.ActualHeight;
                line1.X2 = line2.X1 = crossPos.X;
            }
            else if (btnCurPressed == btnUpArrow.Name) {
                crossPos = Helper.intersectionPos(e1Pos, new Point(e2Pos.X, 0),
                                                  e2Pos, new Point(e1Pos.X, 0));
                line1.Y2 = line2.Y1 = 0;
                line1.X2 = line2.X1 = crossPos.X;
            }
            else if (btnCurPressed == btnLeftArrow.Name) {
                crossPos = Helper.intersectionPos(e1Pos, new Point(0, e2Pos.Y),
                                                  e2Pos, new Point(0, e1Pos.Y));
                line1.X2 = line2.X1 = 0;
                line1.Y2 = line2.Y1 = crossPos.Y;
            }
            else if (btnCurPressed == btnRightArrow.Name) {
                crossPos = Helper.intersectionPos(e1Pos, new Point(cvTable.ActualWidth, e2Pos.Y),
                                                  e2Pos, new Point(cvTable.ActualWidth, e1Pos.Y));
                line1.X2 = line2.X1 = cvTable.ActualWidth;
                line1.Y2 = line2.Y1 = crossPos.Y;
            }

        }


        private void writeSettingFile(Point menuPos, string message = "") {
            using (StreamWriter writer = new StreamWriter("./Data/table-config.txt")) {
                writer.WriteLine($"height_{tableConfig.height}");
                writer.WriteLine($"width_{tableConfig.width}");
                writer.WriteLine($"top_{tableConfig.top}");
                writer.WriteLine($"left_{tableConfig.left}");
                writer.WriteLine($"right_{tableConfig.right}");
                writer.WriteLine($"ball_{tableConfig.ballSize}");
                writer.WriteLine($"menuTop_{menuPos.Y}");
                writer.WriteLine($"menuLeft_{menuPos.X}");
                if (message.Length > 0) {
                    MessageBox.Show("Saved!", "Table Config", MessageBoxButton.OK);
                }
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

                switch (strValue[0]) {
                    case "height":
                        tableConfig.height = value;
                        break;
                    case "width":
                        tableConfig.width = value;
                        break;
                    case "top":
                        tableConfig.top = value;
                        break;
                    case "left":
                        tableConfig.left = value;
                        break;
                    case "right":
                        tableConfig.right = value;
                        break;
                    case "bottom":
                        tableConfig.bottom = value;
                        break;
                    case "ball":
                        tableConfig.ballSize = value;
                        break;
                    case "menuTop":
                        tableConfig.menuTop = value;
                        break;
                    default:
                        tableConfig.menuLeft = value;
                        break;
                }
            }
            brdTable.DataContext = tableConfig;
            brdTable.Margin = new Thickness(tableConfig.left, tableConfig.top, tableConfig.right, tableConfig.bottom);
            ellipse1.DataContext = ellipse2.DataContext = tableConfig;

            Canvas.SetLeft(brdMenu, tableConfig.menuLeft);
            Canvas.SetTop(brdMenu, tableConfig.menuTop);
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

            brdTable.BorderThickness = new Thickness(1);

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

            var menuPos = brdMenu.TransformToAncestor(cvBody).Transform(new Point(0, 0));


            if (settingScreen.ShowDialog() == true) {
                writeSettingFile(menuPos, "Saved");
            }
            else {
                tableConfig = oldTable;
                brdTable.Height = tableConfig.height;
                brdTable.Width = tableConfig.width;
                brdTable.Margin = new Thickness(tableConfig.left, tableConfig.top, tableConfig.right, tableConfig.bottom);
            }
            brdTable.BorderThickness = new Thickness(0);
        }


        private void btnBalls_Click(object sender, RoutedEventArgs e) {
            if (!ellipse1.IsEnabled && !ellipse2.IsEnabled) {
                Helper.endableEllipse(ellipse1);
                Helper.endableEllipse(ellipse2);

                // set position of two balls
                Canvas.SetLeft(ellipse1, (brdTable.ActualWidth / 2 - ellipse1.ActualWidth));
                Canvas.SetTop(ellipse1, brdTable.ActualHeight / 2 - ellipse1.ActualHeight / 2);

                Canvas.SetLeft(ellipse2, (brdTable.ActualWidth / 2) + 1);
                Canvas.SetTop(ellipse2, brdTable.ActualHeight / 2 - ellipse2.ActualHeight / 2);
            }
            else {
                Helper.disableEllipse(ellipse1);
                Helper.disableEllipse(ellipse2);
            }
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);
            Helper.cleanCanvasChild(cvTable, 6, cvTable.Children.Count - 6);
            btnCurPressed = "";
        }

        private void btnDownArrow_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = btnDownArrow.Name;
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);
            updateDirectLine();
        }

        // Clear
        private void btnClear_Click(object sender, RoutedEventArgs e) {
            Helper.disableEllipse(ellipse1);
            Helper.disableEllipse(ellipse2);

            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);

            Helper.cleanCanvasChild(cvTable, 6, cvTable.Children.Count - 6);
            Helper.disablePath(pathLines);
            Helper.disableEllipse(ellipseMove);
            btnCurPressed = "";
        }


        private void btnUpArrow_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = btnUpArrow.Name;
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);
            updateDirectLine();
        }

        private void btnRightArrow_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = btnRightArrow.Name;
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);
            updateDirectLine();
        }

        private void btnLeftArrow_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = btnLeftArrow.Name;
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);
            updateDirectLine();
        }
        private void btnLine_Click(object sender, RoutedEventArgs e) {
            btnClear_Click(sender, e);
            btnCurPressed = oneLine.Name;
        }

        #endregion

        #region TWO BALLS

        private void ellipse1_MouseDown(object sender, MouseButtonEventArgs e) {
            var ellipse = sender as Ellipse;
            if (e.LeftButton == MouseButtonState.Pressed && ellipse != null)
                ellipse.CaptureMouse();
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
            var pos = e.GetPosition(cvTable);
            Canvas.SetLeft(ellipse, pos.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, pos.Y - ellipse.Height / 2);
            updateDirectLine();
        }
        #endregion


        private void brdTable_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && btnCurPressed == oneLine.Name) {
                var curPos = e.GetPosition(cvTable);
                line1.X2 = curPos.X;
                line1.Y2 = curPos.Y;
            }
        }

        private void brdTable_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && btnCurPressed == oneLine.Name) {
                var curPos = e.GetPosition(cvTable);
                line1.X1 = curPos.X;
                line1.Y1 = curPos.Y;
            }
        }

        private void brdTable_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (btnCurPressed == oneLine.Name) {
                var line = new Line() {
                    Stroke = new SolidColorBrush(Colors.Black),
                    Opacity = 0.7,
                    StrokeThickness = 2,
                    X1 = line1.X1,
                    Y1 = line1.Y1,
                    X2 = line1.X2,
                    Y2 = line1.Y2
                };
                cvTable.Children.Add(line);
            }
        }

        private void btnMove_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                btnMove.CaptureMouse();
            }
        }

        private void btnMove_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (btnMove.IsMouseCaptured) {
                btnMove.ReleaseMouseCapture();
                var menuPos = brdMenu.TransformToAncestor(cvBody).Transform(new Point(0, 0));
                writeSettingFile(menuPos);
            }
        }

        private void btnMove_PreviewMouseMove(object sender, MouseEventArgs e) {
            var pos = Mouse.GetPosition(cvBody);
            if (btnMove.IsMouseCaptured) {
                Canvas.SetLeft(brdMenu, pos.X - btnMove.Width / 2);
                Canvas.SetTop(brdMenu, pos.Y - btnMove.Width / 2);
            }
        }

        private void ellipseMove_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                ellipseMove.CaptureMouse();
            }
        }

        private void ellipseMove_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (ellipseMove.IsMouseCaptured) {
                ellipseMove.ReleaseMouseCapture();
            }
        }

        private void ellipseMove_MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && btnCurPressed == btnLongLine.Name) {

                var mousePos = Mouse.GetPosition(cvTable);

                Canvas.SetLeft(ellipseMove, mousePos.X - 5);
                Canvas.SetTop(ellipseMove, mousePos.Y - 5);

                var ballPos = Helper.getEllipsePos(ellipse1, cvTable);

                CurPoint.Text = mousePos.ToString();

                var listPos = new List<Point>();
                listPos.Add(ballPos);

                var firstPos = ballPos;
                if (mousePos.X > 0 && mousePos.Y < 0 && mousePos.X < cvTable.ActualWidth) {
                    var secondPos = new Point(mousePos.X, 0);
                    listPos.Add(secondPos);
                    listPos = Helper.getListPos(listPos, firstPos, secondPos, 4, cvTable);

                }

                // table left
                else if (mousePos.X < 0 && mousePos.Y > 0 && mousePos.Y < cvTable.ActualHeight) {
                    var secondPos = new Point(0, mousePos.Y);
                    listPos.Add(secondPos);
                    listPos = Helper.getListPos(listPos, firstPos, secondPos, 4, cvTable);
                }

                // table bottom
                else if (mousePos.X > 0 && mousePos.Y > cvTable.ActualHeight && mousePos.X < cvTable.ActualWidth) {
                    var secondPos = new Point(mousePos.X, cvTable.ActualHeight);
                    listPos.Add(secondPos);
                    listPos = Helper.getListPos(listPos, firstPos, secondPos, 4, cvTable);
                }

                // table right
                else if (mousePos.X > cvTable.ActualWidth && mousePos.Y <= cvTable.ActualHeight && mousePos.Y > 0) {
                    var secondPos = new Point(cvTable.ActualWidth, mousePos.Y);
                    listPos.Add(secondPos);
                    listPos = Helper.getListPos(listPos, firstPos, secondPos, 4, cvTable);
                }
                Helper.enablePath(pathLines, listPos);
            }
        }

        private void btnLongLine_Click(object sender, RoutedEventArgs e) {
            btnClear_Click(sender, e);
            btnCurPressed = btnLongLine.Name;
            Helper.endableEllipse(ellipse1);
            Helper.endableEllipse(ellipseMove);
        }
    }

}
