using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace nbody
{
    // Calculator class to separate responsibility
    class ForceCalculator
    {

        // Calculate force between 2 bodies
        static public Force CalculateForce(Body b1, Body b2)
        {
            double distance = CalculatorUtils.CalculateDistance(b1.Position, b2.Position);
            if (distance == 0 || distance < (b1.Size + b2.Size))
            {
                return new Force(0, 0);
            }
            double F = (b1.Mass * b2.Mass * WorldProperties.G) / (distance * distance);
            double ang = CalculatorUtils.CalculateAngle(b1.Position, b2.Position);
            double fx = F * Math.Cos(ang);
            double fy = F * Math.Sin(ang);
            return new Force(fx, fy);
        }

        // Overloaded function to calculate force between a body and a Centroid representation of an cluster
        static public Force CalculateForce(Centroid c1, Body b2)
        {
            Point p = new Point(c1.X, c1.Y);
            double distance = CalculatorUtils.CalculateDistance(p, b2.Position);
            if (distance == 0 || distance < b2.Size)
            {
                return new Force(0, 0);
            }
            double F = (c1.Mass * b2.Mass * WorldProperties.G) / (distance * distance);
            double ang = CalculatorUtils.CalculateAngle(p, b2.Position);
            double fx = F * Math.Cos(ang);
            double fy = F * Math.Sin(ang);
            return new Force(-fx, -fy); // TODO -> Why the "-"?
        }

    }


}
