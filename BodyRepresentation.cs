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
    #region UTILS
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

#endregion

    /*This is a facade of the previous properties  */
    class Body
    {
        public Body()
        {
            Size = 0;
            Mass = 0;
            Position= new Point(0,0);
            Velocity = new Velocity(0, 0);
            Acceleration = new Acceleration(0, 0);
            ActingForce = new Force(0, 0);
            ForceT1m = new Force(0, 0);
            ID = ++bodyCounter;
            SolidColor = Brushes.White;
        }

        static int bodyCounter;
        private int ID;
        
        public Body Copy()
        {
            Body tmp = new Body();
            tmp.Size = this.Size;
            tmp.Position = new Point(this.Position.X, this.Position.Y);
            tmp.SolidColor = this.SolidColor;
            return tmp;
        }

        // This property is used to decide the colour when drawing the body
        public SolidColorBrush SolidColor { get; set; }
        // This property decides the radius of the ellipse on the canvas
        public double Size { get; set; }
        // Mass [kg]
        public double Mass { get; set; }
        // Actual position in the Canvas
        public Point Position { get; set; }
        // Actual velocity
        public Velocity Velocity { get; set; }
        // Actual acceleration
        public Acceleration Acceleration { get; set; }
        // Actual acting force
        public Force ActingForce { get; set; }

        // Forces from the previous iteration, used for plotting only now.
        public Force ForceT1m { get; set; }
     }
}
