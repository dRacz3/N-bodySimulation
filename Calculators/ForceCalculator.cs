using System;
using System.Windows;

namespace nbody
{
    // Calculator class to separate responsibility
    internal class ForceCalculator
    {
        // Calculate force between 2 bodies
        public static Force CalculateForce(Body b1, Body b2)
        {
            double distance = CalculatorUtils.CalculateDistance(b1.Position, b2.Position);
            Body bigger = b1.Size > b2.Size ? b1 : b2;
            if (distance == 0 || distance <= bigger.Size)
                return new Force(0, 0);
            double F = b1.Mass * b2.Mass * WorldProperties.G / (distance * distance);
            double ang = CalculatorUtils.CalculateAngle(b1.Position, b2.Position);
            double fx = F * Math.Cos(ang);
            double fy = F * Math.Sin(ang);
            return new Force(fx, fy);
        }

        // Overloaded function to calculate force between a body and a Centroid representation of an cluster
        public static Force CalculateForce(Centroid c1, Body b2)
        {
            Point p = new Point(c1.X, c1.Y);
            double distance = CalculatorUtils.CalculateDistance(p, b2.Position);
            if (distance == 0 || distance < b2.Size)
                return new Force(0, 0);
            double F = c1.Mass * b2.Mass * WorldProperties.G / (distance * distance);
            double ang = CalculatorUtils.CalculateAngle(p, b2.Position);
            double fx = F * Math.Cos(ang);
            double fy = F * Math.Sin(ang);
            return new Force(-fx, -fy); 
        }
    }
}