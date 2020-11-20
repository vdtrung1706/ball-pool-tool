﻿using System;
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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.CompilerServices;
using System.Net.NetworkInformation;

namespace pool_tool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private MenuConfig menuConfig = new MenuConfig();

        private TableConfig tableConfig = new TableConfig();

        private string btnCurPressed = "";


        private Ellipse ellipsePressed;

        private int featurePressed = 1;

        private void updateDirectLine() {


            Helper.disablePath(pathLines);

            if (btnCurPressed == "" || !ellipse1.IsEnabled || !ellipse2.IsEnabled || btnCurPressed == btnLongLine.Name) {
                return;
            }

            var e1Pos = Helper.getEllipsePos(ellipse1, cvTable);
            var e2Pos = Helper.getEllipsePos(ellipse2, cvTable);

            if (Math.Round(e1Pos.X) == Math.Round(e2Pos.X) || Math.Round(e1Pos.Y) == Math.Round(e2Pos.Y)) {
                return;
            }

            line1.X1 = e1Pos.X;
            line1.Y1 = e1Pos.Y;

            line2.X2 = e2Pos.X;
            line2.Y2 = e2Pos.Y;


            //line for trick shot
            line3.X1 = e1Pos.X;
            line3.Y1 = e1Pos.Y;

            Point crossPos;
            if (btnCurPressed == btnDownArrow.Name) {
                crossPos = Helper.intersectionPos(e1Pos, new Point(e2Pos.X, cvTable.ActualHeight),
                                                  e2Pos, new Point(e1Pos.X, cvTable.ActualHeight));
                line2.X1 = crossPos.X;
                line2.Y1 = cvTable.ActualHeight;
                
                // for trick shot
                line3.X2 = crossPos.X + (crossPos.X - e1Pos.X);
                line3.Y2 = cvTable.ActualHeight;

                crossPos = Helper.intersectionPos(new Point(0, cvTable.ActualHeight + 20), new Point(cvTable.ActualWidth, cvTable.ActualHeight + 20),
                                                        new Point(crossPos.X, cvTable.ActualHeight), new Point(e1Pos.X, e1Pos.Y));
                line1.X2 = crossPos.X;
                line1.Y2 = crossPos.Y;
            }
            else if (btnCurPressed == btnUpArrow.Name) {
                crossPos = Helper.intersectionPos(e1Pos, new Point(e2Pos.X, 0),
                                                  e2Pos, new Point(e1Pos.X, 0));
                line2.Y1 = 0;
                line2.X1 = crossPos.X;

                // for trick shot
                line3.X2 = crossPos.X + (crossPos.X - e1Pos.X);
                line3.Y2 = 0;

                crossPos = Helper.intersectionPos(new Point(0, -20), new Point(cvTable.ActualWidth, -20),
                                                        new Point(crossPos.X, 0), new Point(e1Pos.X, e1Pos.Y));
                line1.X2 = crossPos.X;
                line1.Y2 = crossPos.Y;

            }
            else if (btnCurPressed == btnLeftArrow.Name) {
                crossPos = Helper.intersectionPos(e1Pos, new Point(0, e2Pos.Y),
                                                  e2Pos, new Point(0, e1Pos.Y));
                line2.X1 = 0;
                line2.Y1 = crossPos.Y;

                // for trick shot
                line3.X2 = 0;
                line3.Y2 = crossPos.Y + (crossPos.Y - e1Pos.Y); ;

                crossPos = Helper.intersectionPos(new Point(-20, 0), new Point(-20, cvTable.ActualHeight),
                                                        new Point(0, crossPos.Y), new Point(e1Pos.X, e1Pos.Y));
                line1.X2 = crossPos.X;
                line1.Y2 = crossPos.Y;
            }
            else if (btnCurPressed == btnRightArrow.Name) {
                crossPos = Helper.intersectionPos(e1Pos, new Point(cvTable.ActualWidth, e2Pos.Y),
                                                  e2Pos, new Point(cvTable.ActualWidth, e1Pos.Y));
                line2.X1 = cvTable.ActualWidth;
                line2.Y1 = crossPos.Y;


                // for trick shot
                line3.X2 = cvTable.ActualWidth;
                line3.Y2 = crossPos.Y + (crossPos.Y - e1Pos.Y); ;


                crossPos = Helper.intersectionPos(new Point(cvTable.ActualWidth + 20, 0), new Point(cvTable.ActualWidth + 20, cvTable.ActualHeight),
                                                        new Point(cvTable.ActualWidth, crossPos.Y), new Point(e1Pos.X, e1Pos.Y));
                line1.X2 = crossPos.X;
                line1.Y2 = crossPos.Y;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            #region READ FILE SETTING
            var line = "";
            using (var tableFile = new StreamReader("./Data/table-config.txt")) {
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
                        case "ball":
                            tableConfig.ballSize = value;
                            break;
                    }
                }
            }
            using (var menuFile = new StreamReader("./Data/menu-config.txt")) {
                while ((line = menuFile.ReadLine()) != null) {
                    var strValue = line.Split('_');
                    double value = 0;
                    double.TryParse(strValue[1], out value);

                    switch (strValue[0]) {
                        case "top":
                            menuConfig.top = value;
                            break;
                        case "left":
                            menuConfig.left = value;
                            break;
                    }
                }
            }


            brdTable.DataContext = tableConfig;
            brdTable.Margin = new Thickness(tableConfig.left, tableConfig.top, 0, 0);
            brdTable.Padding = new Thickness(tableConfig.ballSize / 2 - 2);
            ellipse1.DataContext = ellipse2.DataContext = tableConfig;

            Canvas.SetLeft(brdMenu, menuConfig.left);
            Canvas.SetTop(brdMenu, menuConfig.top);

            ellipsePressed = ellipse1;

            #endregion
        }

        #region MENU

        private void SettingScreen_SizeChange(TableConfig newTable, string mess = "") {

            brdTable.DataContext = newTable;
            ellipse1.DataContext = ellipse2.DataContext = newTable;

            if (mess == "save") {
                var brdTablePos = brdTable.TransformToAncestor(cvBody).Transform(new Point(0, 0));

                tableConfig.ballSize = newTable.ballSize;
                tableConfig.width = brdTable.ActualWidth;
                tableConfig.height = brdTable.ActualHeight;
                tableConfig.top = brdTablePos.Y;
                tableConfig.left = brdTablePos.X;


                writeSettingFile(tableConfig);
                brdTable.BorderThickness = new Thickness(0);
                Helper.disableEllipse(cornerBot);
                Helper.disableEllipse(cornerTop);
                Helper.disableEllipse(ellipse1);
                btnCurPressed = "";
            }
            else if (mess != "") {
                brdTable.BorderThickness = new Thickness(0);
                Helper.disableEllipse(cornerBot);
                Helper.disableEllipse(cornerTop);
                Helper.disableEllipse(ellipse1);
                btnCurPressed = "";
            }
            brdTable.Padding = new Thickness(newTable.ballSize / 2 - 2);
        }

        #endregion


        #region MENU BUTTON


        private void writeSettingFile(TableConfig tableConfig) {
            using (StreamWriter writer = new StreamWriter("./Data/table-config.txt")) {
                writer.WriteLine($"height_{tableConfig.height}");
                writer.WriteLine($"width_{tableConfig.width}");
                writer.WriteLine($"top_{tableConfig.top}");
                writer.WriteLine($"left_{tableConfig.left}");
                writer.WriteLine($"ball_{tableConfig.ballSize}");
            }
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e) {
            if (btnCurPressed == btnSetting.Name) {
                return;
            }

            btnClear_Click(sender, e);

            btnCurPressed = btnSetting.Name;

            brdTable.BorderThickness = new Thickness(1);

            Helper.endableEllipse(cornerBot);
            Helper.endableEllipse(cornerTop);
            Helper.endableEllipse(ellipse1);

            var brdTablePos = brdTable.TransformToAncestor(cvBody).Transform(new Point(0, 0));
            Canvas.SetLeft(cornerTop, brdTablePos.X);
            Canvas.SetTop(cornerTop, brdTablePos.Y);

            Canvas.SetLeft(cornerBot, brdTablePos.X + brdTable.ActualWidth);
            Canvas.SetTop(cornerBot, brdTablePos.Y + brdTable.ActualHeight);

            var settingScreen = new SettingWindow(tableConfig);
            settingScreen.SizeChange += SettingScreen_SizeChange;
            settingScreen.Show();

        }


        private void btnBalls_Click(object sender, RoutedEventArgs e) {
            if (!ellipse2.IsEnabled) {
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
            Helper.disableEllipse(ellipseMove);
            Helper.disablePath(pathLines);
            Helper.cleanCanvasChild(cvTable, 7, cvTable.Children.Count - 7);
            btnCurPressed = "";
        }

        private void btnDownArrow_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = btnDownArrow.Name;
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);
            Helper.endableEllipse(ellipse1);
            Helper.endableEllipse(ellipse2);
            updateDirectLine();
        }

        // Clear
        private void btnClear_Click(object sender, RoutedEventArgs e) {
            Helper.disableEllipse(ellipse1);
            Helper.disableEllipse(ellipse2);

            Helper.resetLinePos(line);
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);

            Helper.cleanCanvasChild(cvTable, 7, cvTable.Children.Count - 7);
            Helper.disablePath(pathLines);
            Helper.disableEllipse(ellipseMove);
            btnCurPressed = "";
        }


        private void btnUpArrow_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = btnUpArrow.Name;
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);
            Helper.endableEllipse(ellipse1);
            Helper.endableEllipse(ellipse2);
            updateDirectLine();
        }

        private void btnRightArrow_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = btnRightArrow.Name;
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);

            Helper.endableEllipse(ellipse1);
            Helper.endableEllipse(ellipse2);
            updateDirectLine();
        }

        private void btnLeftArrow_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = btnLeftArrow.Name;
            Helper.resetLinePos(line1);
            Helper.resetLinePos(line2);

            Helper.endableEllipse(ellipse1);
            Helper.endableEllipse(ellipse2);
            updateDirectLine();
        }
        private void btnLine_Click(object sender, RoutedEventArgs e) {
            btnCurPressed = oneLine.Name;
        }

        #endregion

        #region TWO BALLS

        private void ellipse1_MouseDown(object sender, MouseButtonEventArgs e) {
            var ellipse = sender as Ellipse;
            //ellipseCurPressed = ellipse.Name;
            ellipsePressed = ellipse;
            if (e.LeftButton == MouseButtonState.Pressed && ellipse != null) {
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
            var pos = e.GetPosition(cvTable);
            Canvas.SetLeft(ellipse, pos.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, pos.Y - ellipse.Height / 2);
            updateDirectLine();
        }
        #endregion


        private void brdTable_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && btnCurPressed == oneLine.Name) {
                var curPos = e.GetPosition(cvTable);
                line.X2 = curPos.X;
                line.Y2 = curPos.Y;
            }
        }

        private void brdTable_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && btnCurPressed == oneLine.Name) {
                var curPos = e.GetPosition(cvTable);
                line.X1 = curPos.X;
                line.Y1 = curPos.Y;
            }
        }

        private void brdTable_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (btnCurPressed == oneLine.Name) {
                var newLine = new Line() {
                    Stroke = new SolidColorBrush(Colors.Black),
                    Opacity = 0.6,
                    StrokeThickness = 3,
                    X1 = line.X1,
                    Y1 = line.Y1,
                    X2 = line.X2,
                    Y2 = line.Y2
                };

                cvTable.Children.Add(newLine);
                Helper.resetLinePos(line);
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
                menuConfig.top = menuPos.Y;
                menuConfig.left = menuPos.X;

                using (StreamWriter writer = new StreamWriter("./Data/menu-config.txt")) {
                    writer.WriteLine($"top_{menuConfig.top}");
                    writer.WriteLine($"left_{menuConfig.left}");
                }
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


                var listPos = new List<Point>();

                listPos.Add(Helper.roundPos(ballPos, 7));

                var secondPos = new Point();
                var firstPos = ballPos;


                if (mousePos.X > 0 && mousePos.Y < 0 && mousePos.X < cvTable.ActualWidth) {
                    secondPos = new Point(mousePos.X, 0);
                }

                // table left
                else if (mousePos.X < 0 && mousePos.Y > 0 && mousePos.Y < cvTable.ActualHeight) {
                    secondPos = new Point(0, mousePos.Y);
                }

                // table bottom
                else if (mousePos.X > 0 && mousePos.Y > cvTable.ActualHeight && mousePos.X < cvTable.ActualWidth) {
                    secondPos = new Point(mousePos.X, cvTable.ActualHeight);
                }

                // table right
                else if (mousePos.X > cvTable.ActualWidth && mousePos.Y <= cvTable.ActualHeight && mousePos.Y > 0) {
                    secondPos = new Point(cvTable.ActualWidth, mousePos.Y);
                }
                listPos.Add(Helper.roundPos(secondPos, 7));

                listPos = Helper.getListPos(listPos, firstPos, secondPos, 4, cvTable, ballPos);
                Helper.enablePath(pathLines, listPos);
            }
        }

        private void btnLongLine_Click(object sender, RoutedEventArgs e) {
            btnClear_Click(sender, e);
            btnCurPressed = btnLongLine.Name;

            Canvas.SetLeft(ellipse1, cvTable.ActualWidth / 2);
            Canvas.SetTop(ellipse1, cvTable.ActualHeight / 2);

            Canvas.SetLeft(ellipseMove, cvTable.ActualWidth / 2);

            Helper.endableEllipse(ellipse1);
            Helper.endableEllipse(ellipseMove);
        }




        private void cornerTop_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                cornerTop.CaptureMouse();
            }
        }

        private void cornerTop_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (cornerTop.IsMouseCaptured) {
                cornerTop.ReleaseMouseCapture();
            }
        }

        private void cornerTop_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                var pos = e.GetPosition(cvBody);

                var cornerBotPos = cornerBot.TransformToAncestor(cvBody).Transform(new Point(0, 0));

                var newBorder = Helper.getBorder(pos, cornerBotPos);
                if (newBorder.Height > 0) {
                    brdTable.Height = newBorder.Height;
                    brdTable.Width = newBorder.Width;
                    brdTable.Margin = newBorder.Margin;
                }
                Canvas.SetLeft(cornerTop, pos.X);
                Canvas.SetTop(cornerTop, pos.Y);
            }
        }

        private void cornerBot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                cornerBot.CaptureMouse();
            }
        }

        private void cornerBot_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (cornerBot.IsMouseCaptured) {
                cornerBot.ReleaseMouseCapture();
            }
        }

        private void cornerBot_PreviewMouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                var pos = e.GetPosition(cvBody);

                var cornerTopPos = cornerTop.TransformToAncestor(cvBody).Transform(new Point(0, 0));

                var newBorder = Helper.getBorder(cornerTopPos, pos);
                if (newBorder.Height > 0) {
                    brdTable.Height = newBorder.Height;
                    brdTable.Width = newBorder.Width;
                    brdTable.Margin = newBorder.Margin;
                }
                Canvas.SetLeft(cornerBot, pos.X);
                Canvas.SetTop(cornerBot, pos.Y);
            }
        }

        private void mainWindow_KeyDown(object sender, KeyEventArgs e) {

            var ePos = ellipsePressed.TransformToAncestor(cvTable).Transform(new Point(0, 0));
            switch (e.Key) {
                case Key.A:
                    Canvas.SetLeft(ellipsePressed, ePos.X - 0.7);
                    break;
                case Key.W:
                    Canvas.SetTop(ellipsePressed, ePos.Y - 0.7);
                    break;
                case Key.D:
                    Canvas.SetLeft(ellipsePressed, ePos.X + 0.7);
                    break;
                case Key.S:
                    Canvas.SetTop(ellipsePressed, ePos.Y + 0.7);
                    break;
                case Key.C:
                    btnClear_Click(sender, e);
                    break;
                case Key.B:
                    btnBalls_Click(sender, e);
                    break;
                case Key.F1:
                    btnSetting_Click(sender, e);
                    break;
                case Key.T:
                    btnLongLine_Click(sender, e);
                    break;
                case Key.L:
                    btnLine_Click(sender, e);
                    break;
                case Key.Tab:
                    if (featurePressed == 5) {
                        featurePressed = 1;
                    }
                    if (featurePressed == 1) {
                        btnLeftArrow_Click(sender, e);

                    }
                    else if (featurePressed == 2) {
                        btnUpArrow_Click(sender, e);

                    }
                    else if (featurePressed == 3) {
                        btnRightArrow_Click(sender, e);
                    }
                    else {
                        btnDownArrow_Click(sender, e);

                    }
                    featurePressed++;
                    break;
            }
        }
    }

}
