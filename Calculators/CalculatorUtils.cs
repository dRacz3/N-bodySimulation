using System;
using System.Windows;

namespace nbody
{
    // Basic calculation functionality that are independent.
    internal class CalculatorUtils
    {
        public static double CalculateDistance(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double CalculateAngle(Point p1, Point p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;

            return Math.Atan2(dy, dx);
        }
    }
}