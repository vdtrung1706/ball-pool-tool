using MaterialDesignThemes.Wpf.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pool_tool.Class {
   public class Helper {

      public static Point intersectPos(Point a, Point b, Point c, Point d) {
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
         } else {
            double x = (b2 * c1 - b1 * c2) / determinant;
            double y = (a1 * c2 - a2 * c1) / determinant;
            return new Point(x, y);
         }
      }

      #region ELLIPSE
      public static void disableEllipses(Ellipse ellipse) {
         ellipse.Visibility = Visibility.Hidden;
         ellipse.IsEnabled = false;
      }

      public static void disableEllipses(Ellipse e1, Ellipse e2) {
         disableEllipses(e1);
         disableEllipses(e2);
      }

      public static void disableEllipses(Ellipse e1, Ellipse e2, Ellipse e3) {
         disableEllipses(e1, e2);
         disableEllipses(e3);
      }

      public static void enableEllipses(Ellipse ellipse) {
         ellipse.Visibility = Visibility.Visible;
         ellipse.IsEnabled = true;
      }

      public static void enableEllipses(Ellipse e1, Ellipse e2) {
         enableEllipses(e1);
         enableEllipses(e2);
      }

      public static void enableEllipses(Ellipse e1, Ellipse e2, Ellipse e3) {
         enableEllipses(e1, e2);
         enableEllipses(e3);
      }

      public static Point getEllipsePos(Ellipse ellipse, Canvas canvas) {
         var pos = ellipse.TransformToAncestor(canvas).Transform(new Point(0, 0));
         return new Point(pos.X + ellipse.Height / 2, pos.Y + ellipse.Width / 2);
      }
      #endregion


      public static void removeCanvasChild(Canvas canvas, int index, int count) => canvas.Children.RemoveRange(index, count);

      public static void disableLines(Line line) => line.X1 = line.X2 = line.Y1 = line.Y2 = 0;

      public static void disableLines(Line line, Line line1) {
         disableLines(line);
         disableLines(line1);
      }

      public static void disableLines(Line line, Line line1, Line line2) {
         disableLines(line);
         disableLines(line1, line2);
      }

      public static void disableLines(Line line, Line line1, Line line2, Line line3) {
         disableLines(line, line3);
         disableLines(line1, line2);
      }

      public static void disablePath(Path path) {
         path.IsEnabled = false;
         path.Visibility = Visibility.Hidden;
         path.Data = Geometry.Parse("");
      }

      public static void disablePath(Path path, Path path1) {
         disablePath(path);
         disablePath(path1);
      }

      public static void enablePath(Path path, List<Point> points = null) {
         path.IsEnabled = true;
         path.Visibility = Visibility.Visible;
         if (points != null) {
            var pathData = "M";
            foreach (var pos in points) {
               pathData += " " + pos.ToString();
            }
            path.Data = Geometry.Parse(pathData);
         }
      }

      public static Point roundPos(Point pos, int digits) {
         return new Point(Math.Round(pos.X, digits), Math.Round(pos.Y, digits));
      }

      public static double distance(Point pos1, Point pos2) {
         // Calculating distance 
         return Math.Sqrt(Math.Pow(pos2.X - pos1.X, 2) +
                         Math.Pow(pos2.Y - pos1.Y, 2) * 1.0);
      }

      public static Border getBorder(Point topPos, Point botPos) {
         var border = new Border();
         border.Width = distance(new Point(botPos.X, topPos.Y), topPos);
         border.Height = distance(new Point(topPos.X, botPos.Y), topPos);
         if (botPos.X > topPos.X) {
            border.Margin = new Thickness(topPos.X, topPos.Y, 0, 0);
         } else if (botPos.X < topPos.X) {
            border.Margin = new Thickness(topPos.X - border.ActualWidth, topPos.Y, 0, 0);
         }
         return border;
      }

      public static Point missingPoint(Point a, Point b, Point c) {
         return new Point(a.X + (c.X - b.X), a.Y + (c.Y - b.Y));
      }

      public static List<Point> getPoints(Point ballPos, Point curPos, Canvas table, Ellipse el) {
         List<Point> points = new List<Point>();
         Point missingPos;
         Point symPos;

         var touchPos = new Point();
         var touchPos2 = new Point();

         var topLeft = new Point(0, 0);
         var topRight = new Point(table.ActualWidth + el.Height / 2, 0);
         var botLeft = new Point(0, table.ActualHeight);
         var botRight = new Point(table.ActualWidth, table.ActualHeight);

         var secondPos = curPos;
         // top
         if (curPos.X >= 0 && curPos.X <= table.ActualWidth && curPos.Y <= 0) {
            secondPos.Y = 0;
            if (secondPos.X < ballPos.X) {
               symPos = new Point(-secondPos.X, 0);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos = intersectPos(topLeft, botLeft, symPos, missingPos);
               if (touchPos.Y > table.ActualHeight) {
                  touchPos = intersectPos(touchPos, secondPos, botLeft, botRight);
               }

               symPos = new Point(-secondPos.X * 2, 0);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos2 = intersectPos(topRight, botRight, symPos, missingPos);
               if (touchPos2.Y > table.ActualHeight) {
                  touchPos2 = intersectPos(touchPos, touchPos2, botRight, botLeft);
               }
            } else if (secondPos.X > ballPos.X) {
               symPos = new Point(table.ActualWidth + (table.ActualWidth - secondPos.X), 0);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos = intersectPos(topRight, botRight, symPos, missingPos);
               if (touchPos.Y > table.ActualHeight) {
                  touchPos = intersectPos(botLeft, botRight, secondPos, touchPos);
               }

               symPos = new Point(table.ActualWidth + (table.ActualWidth - secondPos.X) * 2, 0);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos2 = intersectPos(topLeft, botLeft, symPos, missingPos);
               if (touchPos2.Y > table.ActualHeight) {
                  touchPos2 = intersectPos(botLeft, botRight, touchPos, touchPos2);
               }
            }
         }
         // bottom
         else if (curPos.X >= 0 && curPos.X <= table.ActualWidth && curPos.Y >= table.ActualHeight) {
            secondPos.Y = table.ActualHeight;
            if (secondPos.X < ballPos.X) {
               symPos = new Point(-secondPos.X, table.ActualHeight);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos = intersectPos(botLeft, topLeft, symPos, missingPos);
               if (touchPos.Y < 0) {
                  touchPos = intersectPos(topLeft, topRight, secondPos, touchPos);
               }

               symPos = new Point(-secondPos.X * 2, table.ActualHeight);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos2 = intersectPos(topRight, botRight, symPos, missingPos);
               if (touchPos2.Y < 0) {
                  touchPos2 = intersectPos(topLeft, topRight, touchPos2, touchPos);
               }
            } else if (secondPos.X > ballPos.X) {
               symPos = new Point(table.ActualWidth + (table.ActualWidth - secondPos.X), table.ActualHeight);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos = intersectPos(botRight, topRight, symPos, missingPos);
               if (touchPos.Y < 0) {
                  touchPos = intersectPos(topLeft, topRight, secondPos, touchPos);
               }

               symPos = new Point(table.ActualWidth + (table.ActualWidth - secondPos.X) * 2, table.ActualHeight);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos2 = intersectPos(topLeft, botLeft, symPos, missingPos);
               if (touchPos2.Y < 0) {
                  touchPos2 = intersectPos(topLeft, topRight, touchPos2, touchPos);
               }
            }
         }
         // left
         else if (curPos.X <= 0 && curPos.Y >= 0 && curPos.Y <= table.ActualHeight) {
            secondPos.X = 0;
            if (secondPos.Y < ballPos.Y) {
               symPos = new Point(0, -secondPos.Y);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos = intersectPos(topLeft, topRight, symPos, missingPos);

               if (touchPos.X > table.ActualWidth) {
                  touchPos = intersectPos(touchPos, secondPos, topRight, botRight);
               }

               symPos = new Point(0, -secondPos.Y * 2);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos2 = intersectPos(botRight, botLeft, symPos, missingPos);
               if (touchPos2.X > table.ActualWidth) {
                  touchPos2 = intersectPos(touchPos, touchPos2, topRight, botRight);
               }

            } else if (secondPos.Y > ballPos.Y) {
               symPos = new Point(0, table.ActualHeight + (table.ActualHeight - secondPos.Y));
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos = intersectPos(botLeft, botRight, symPos, missingPos);
               if (touchPos.X > table.ActualWidth) {
                  touchPos = intersectPos(touchPos, secondPos, topRight, botRight);
               }

               symPos = new Point(0, table.ActualHeight + (table.ActualHeight - secondPos.Y) * 2);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos2 = intersectPos(topLeft, topRight, symPos, missingPos);
               if (touchPos2.X > table.ActualWidth) {
                  touchPos2 = intersectPos(touchPos, touchPos2, topRight, botRight);
               }
            }
         }
         // right
         else if (curPos.X >= table.ActualWidth && curPos.Y >= 0 && curPos.Y <= table.ActualHeight) {
            secondPos.X = table.ActualWidth;
            if (secondPos.Y < ballPos.Y) {
               symPos = new Point(table.ActualWidth, 0 - secondPos.Y);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos = intersectPos(topLeft, topRight, symPos, missingPos);


               if (touchPos.X < 0) {
                  touchPos = intersectPos(touchPos, secondPos, topLeft, botLeft);
               }

               symPos = new Point(table.ActualWidth, 0 - secondPos.Y * 2);
               missingPos = missingPoint(ballPos, secondPos, symPos);

               touchPos2 = intersectPos(botLeft, botRight, symPos, missingPos);


               if (touchPos2.X < 0) {
                  touchPos2 = intersectPos(touchPos, touchPos2, topLeft, botLeft);
               }

            } else if (secondPos.Y > ballPos.Y) {
               symPos = new Point(table.ActualWidth, table.ActualHeight + (table.ActualHeight - secondPos.Y));
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos = intersectPos(botLeft, botRight, symPos, missingPos);

               if (touchPos.X < 0) {
                  touchPos = intersectPos(touchPos, secondPos, topLeft, botLeft);
               }
               symPos = new Point(table.ActualWidth, table.ActualHeight + (table.ActualHeight - secondPos.Y) * 2);
               missingPos = missingPoint(ballPos, secondPos, symPos);
               touchPos2 = intersectPos(topLeft, topRight, symPos, missingPos);
               if (touchPos2.X < 0) {
                  touchPos2 = intersectPos(touchPos, touchPos2, topLeft, botLeft);
               }
            }
         }
         points.Add(ballPos);
         points.Add(secondPos);
         points.Add(touchPos);
         points.Add(touchPos2);
         return points;
      }

      public static Point nextPos(Point firstPos, Point secondPos, Canvas table, int lineCount = 0) {
         var corTopLeftPos = new Point(0, 0);
         var corTopRightPos = new Point(table.ActualWidth, 0);
         var corBotLeftPos = new Point(0, table.ActualHeight);
         var corBotRightPos = new Point(table.ActualWidth, table.ActualHeight);
         Point nextPos;
         // table top
         if (secondPos.X >= 0 && secondPos.X <= table.ActualWidth && secondPos.Y == 0) {
            var crossBotPos = intersectPos(firstPos, secondPos, corBotLeftPos, corBotRightPos);
            var symPos = new Point(crossBotPos.X - 2 * (crossBotPos.X - secondPos.X), table.ActualHeight);
            if (symPos.X < 0) {
               nextPos = intersectPos(secondPos, symPos, corTopLeftPos, corBotLeftPos);
            } else if (symPos.X > table.ActualWidth) {
               nextPos = intersectPos(secondPos, symPos, corTopRightPos, corBotRightPos);
            } else {
               nextPos = symPos;
            }
         }

         // table left
         else if (secondPos.Y >= 0 && secondPos.Y <= table.ActualHeight && secondPos.X == 0) {
            var crossRightPos = intersectPos(firstPos, secondPos, corTopRightPos, corBotRightPos);
            var symPos = new Point(table.ActualWidth, crossRightPos.Y - 2 * (crossRightPos.Y - secondPos.Y));
            if (symPos.Y < 0) {
               nextPos = intersectPos(secondPos, symPos, corTopLeftPos, corTopRightPos);
            } else if (symPos.Y > table.ActualHeight) {
               nextPos = intersectPos(secondPos, symPos, corBotLeftPos, corBotRightPos);
            } else {
               nextPos = symPos;
            }
         }

         // table bottom
         else if (secondPos.X >= 0 && secondPos.X <= table.ActualWidth && secondPos.Y == table.ActualHeight) {
            var crossTopPos = intersectPos(firstPos, secondPos, corTopLeftPos, corTopRightPos);
            var symPos = new Point(crossTopPos.X - 2 * (crossTopPos.X - secondPos.X), 0);
            if (symPos.X < 0) {
               nextPos = intersectPos(secondPos, symPos, corTopLeftPos, corBotLeftPos);
            } else if (symPos.X > table.ActualWidth) {
               nextPos = intersectPos(secondPos, symPos, corTopRightPos, corBotRightPos);
            } else {
               nextPos = symPos;
            }
         }
         // table right
         else {
            var crossLeftPos = intersectPos(firstPos, secondPos, corTopLeftPos, corBotLeftPos);
            var symPos = new Point(0, crossLeftPos.Y - 2 * (crossLeftPos.Y - secondPos.Y));
            if (symPos.Y < 0) {
               nextPos = intersectPos(secondPos, symPos, corTopLeftPos, corTopRightPos);
            } else if (symPos.Y > table.ActualHeight) {
               nextPos = intersectPos(secondPos, symPos, corBotLeftPos, corBotRightPos);
            } else {
               nextPos = symPos;
            }
         }
         return nextPos;
      }
   }
}
