using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace nbody
{
    /*
     * This is a List of classes that act as properties in the simulation to keep 
     * the naming scheme and code readability.
    */

    class Force
    {
        public Force(Force force)
        {
            X = force.X;
            Y = force.Y;
        }

        public Force(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }
    class Velocity
    {
        public Velocity(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }

    class Position
    {
        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }

    class Acceleration
    {
        public Acceleration(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

    }


    /*This is a facade of the previous properties  */
    class Body
    {
        public Body()
        {
            Size = 0;
            Mass = 0;
            Position= new Point(0,0);
            Velocity = new Velocity(0, 0);
            acceleration = new Acceleration(0, 0);
            ActingForce = new Force(0, 0);
            ForceT1m = new Force(0, 0);
            ID = ++bodyCounter;
            solidColor = Brushes.White;
        }

        static int bodyCounter;
        private int ID;
        

        // This property is used to decide the colour when drawing the body
        public SolidColorBrush solidColor { get; set; }
        // This property decides the radius of the ellipse on the canvas
        public double Size { get; set; }
        // Mass [kg]
        public double Mass { get; set; }
        // Actual position in the Canvas
        public Point Position { get; set; }
        // Actual velocity
        public Velocity Velocity { get; set; }
        // Actual acceleration
        public Acceleration acceleration { get; set; }
        // Actual acting force
        public Force ActingForce { get; set; }

        // Forces from the previous iteration, used for plotting only now.
        public Force ForceT1m { get; set; }
     }
}
