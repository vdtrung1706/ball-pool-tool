using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pool_tool.Class {
    public class Helper {
        public static Point intersectionPos(Point a, Point b, Point c, Point d) {
            // Line AB represented as a1x + b1y = c1  
            double a1 = b.Y - a.Y;
            double b1 = a.X - b.X;
            double c1 = a1 * (a.X) + b1 * (a.Y);

            // Line CD represented as a2x + b2y = c2  
            double a2 = d.Y - c.Y;
            double b2 = c.X - d.X;
            double c2 = a2 * (c.X) + b2 * (c.Y);

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0) {
                // The lines are parallel. This is simplified  
                // by returning a pair of FLT_MAX  
                return new Point(double.MaxValue, double.MaxValue);
            }
            else {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                return new Point(x, y);
            }
        }

        public static Point nextPos(Point firstPos, Point secondPos, Canvas table, int lineCount = 0) {

            var nextPos = new Point(0, 0);

            var corTopLeftPos = new Point(0, 0);
            var corTopRightPos = new Point(table.ActualWidth, 0);
            var corBotLeftPos = new Point(0, table.ActualHeight);
            var corBotRightPos = new Point(table.ActualWidth, table.ActualHeight);

            // table top
            if (secondPos.X >= 0 && secondPos.X <= table.ActualWidth && secondPos.Y == 0) {
                var crossBotPos = intersectionPos(firstPos, secondPos, corBotLeftPos, corBotRightPos);
                var symPos = new Point(crossBotPos.X - 2 * (crossBotPos.X - secondPos.X), table.ActualHeight);

                if (symPos.X < 0) {
                    nextPos = intersectionPos(secondPos, symPos, corTopLeftPos, corBotLeftPos);
                }
                else if (symPos.X > table.ActualWidth) {
                    nextPos = intersectionPos(secondPos, symPos, corTopRightPos, corBotRightPos);
                }
                else {
                    nextPos = symPos;
                }

                if (lineCount == 2) {
                    nextPos.X += firstPos.X > secondPos.X ? -50 : 50;
                }

                if (lineCount == 3) {
                    nextPos.X += firstPos.X > secondPos.X ? -60 : 60;
                }
                if (lineCount == 4) {
                    nextPos.X += firstPos.X > secondPos.X ? -70 : 70;
                }
            }

            // table left
            else if (secondPos.Y >= 0 && secondPos.Y <= table.ActualHeight && secondPos.X == 0) {
                var crossRightPos = intersectionPos(firstPos, secondPos, corTopRightPos, corBotRightPos);
                var symPos = new Point(table.ActualWidth, crossRightPos.Y - 2 * (crossRightPos.Y - secondPos.Y));
                if (symPos.Y < 0) {
                    nextPos = intersectionPos(secondPos, symPos, corTopLeftPos, corTopRightPos);
                }
                else if (symPos.Y > table.ActualHeight) {
                    nextPos = intersectionPos(secondPos, symPos, corBotLeftPos, corBotRightPos);
                }
                else {
                    nextPos = symPos;
                }
                if (lineCount == 2) {
                    nextPos.Y += firstPos.Y > secondPos.Y ? -50 : 50;
                }

                if (lineCount == 3) {
                    nextPos.Y += firstPos.Y > secondPos.Y ? -60 : 60;
                }

                if (lineCount == 4) {
                    nextPos.Y += firstPos.Y > secondPos.Y ? -70 : 70;
                }
            }

            // table bottom
            else if (secondPos.X >= 0 && secondPos.X <= table.ActualWidth && secondPos.Y == table.ActualHeight) {
                var crossTopPos = intersectionPos(firstPos, secondPos, corTopLeftPos, corTopRightPos);
                var symPos = new Point(crossTopPos.X - 2 * (crossTopPos.X - secondPos.X), 0);
                if (symPos.X < 0) {
                    nextPos = intersectionPos(secondPos, symPos, corTopLeftPos, corBotLeftPos);
                }
                else if (symPos.X > table.ActualWidth) {
                    nextPos = intersectionPos(secondPos, symPos, corTopRightPos, corBotRightPos);
                }
                else {
                    nextPos = symPos;
                }

            }

            // table right
            else {
                var crossLeftPos = intersectionPos(firstPos, secondPos, corTopLeftPos, corBotLeftPos);
                var symPos = new Point(0, crossLeftPos.Y - 2 * (crossLeftPos.Y - secondPos.Y));
                if (symPos.Y < 0) {
                    nextPos = intersectionPos(secondPos, symPos, corTopLeftPos, corTopRightPos);
                }
                else if (symPos.Y > table.ActualHeight) {
                    nextPos = intersectionPos(secondPos, symPos, corBotLeftPos, corBotRightPos);
                }
                else {
                    nextPos = symPos;
                }

                if (lineCount == 2) {
                    nextPos.Y += firstPos.Y > secondPos.Y ? -50 : 50;
                }

                if (lineCount == 3) {
                    nextPos.Y += firstPos.Y > secondPos.Y ? -60 : 60;
                }

                if (lineCount == 4) {
                    nextPos.Y += firstPos.Y > secondPos.Y ? -70 : 70;
                }
            }
            return nextPos;
        }

        public static void endableEllipse(Ellipse ellipse) {
            ellipse.Visibility = Visibility.Visible;
            ellipse.IsEnabled = true;
        }

        public static void disableEllipse(Ellipse ellipse) {
            ellipse.Visibility = Visibility.Hidden;
            ellipse.IsEnabled = false;
        }

        public static void cleanCanvasChild(Canvas canvas, int index, int count) => canvas.Children.RemoveRange(index, count);

        public static Point getEllipsePos(Ellipse ellipse, Canvas canvas) {
            var pos = ellipse.TransformToAncestor(canvas).Transform(new Point(0, 0));
            return new Point(pos.X + ellipse.Height / 2, pos.Y + ellipse.Width / 2);
        }

        public static void resetLinePos(Line line) => line.X1 = line.X2 = line.Y1 = line.Y2 = 0;

        public static void disablePath(Path path) {
            path.IsEnabled = false;
            path.Visibility = Visibility.Hidden;
            path.Data = Geometry.Parse("");
        }

        public static void enablePath(Path path, List<Point> listPos = null) {
            path.IsEnabled = true;
            path.Visibility = Visibility.Visible;
            if (listPos != null) {
                var pathData = "M";
                foreach (var pos in listPos) {
                    pathData += " " + pos.ToString();
                }
                path.Data = Geometry.Parse(pathData);
            }
        }

        public static Point roundPos(Point pos, int digits) {
            pos.X = Math.Round(pos.X, digits);
            pos.Y = Math.Round(pos.Y, digits);
            return pos;
        }

        public static List<Point> getListPos(List<Point> listPos, Point firstPos, Point secondPos, int lineCount, Canvas canvas) {
            for (int i = 0; i < lineCount; i++) {
                var nextPos = Helper.nextPos(firstPos, secondPos, canvas, i + 1);
                nextPos = roundPos(nextPos, 3);
                listPos.Add(nextPos);
                firstPos = secondPos;
                secondPos = nextPos;
            }
            return listPos;
        }
    }
}
