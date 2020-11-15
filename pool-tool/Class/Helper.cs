using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace pool_tool.Class {
    public class Helper {
        public static Point pointsIntersection(Point a, Point b, Point c, Point d) {
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
    }
}
