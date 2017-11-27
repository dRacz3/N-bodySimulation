using System.Windows;

namespace nbody
{
    internal class BoundingBox
    {
        // IDs are based on this counter
        private static int Counter;

        // Constructor that initializez the coordinates
        public BoundingBox(double xmax, double xmin, double ymax, double ymin)
        {
            Xmax = xmax;
            Xmin = xmin;
            Ymax = ymax;
            Ymin = ymin;

            Counter++;
        }

        // Read only properties
        public double Xmax { get; }

        public double Xmin { get; }
        public double Ymax { get; }
        public double Ymin { get; }

        public string ID { get; set; }

        // Check if the position is between the Boundaries specified in the properties
        public bool isInside(Position position)
        {
            if (position.X > Xmin &&
                position.X <= Xmax &&
                position.Y > Ymin &&
                position.Y <= Ymax)
                return true;
            return false;
        }

        public bool isInside(Point position)
        {
            if (position.X > Xmin &&
                position.X <= Xmax &&
                position.Y > Ymin &&
                position.Y <= Ymax)
                return true;
            return false;
        }

        public BoundingBox[] Split(BoundingBox boundingBox)
        {
            BoundingBox[] splittedBoxes = new BoundingBox[4];
            double xs = boundingBox.Xmin;
            double xe = boundingBox.Xmax;
            double ys = boundingBox.Ymin;
            double ye = boundingBox.Ymax;

            double h = ye - ys;
            double w = xe - xs;


            BoundingBox NorthWest = new BoundingBox(xs + w / 2, xs, ys + h / 2, ys);
            BoundingBox NorthEast = new BoundingBox(xe, xs + w / 2, ys + h / 2, ys);
            BoundingBox SouthWest = new BoundingBox(xs + w / 2, xs, ye, ys + h / 2);
            BoundingBox SouthEast = new BoundingBox(xe, xs + w / 2, ye, ys + h / 2);
            // TODO Check if coordinates are alright..

            NorthEast.ID = "NE";
            NorthWest.ID = "NW";
            SouthWest.ID = "SW";
            SouthEast.ID = "SE";

            splittedBoxes[0] = NorthWest;
            splittedBoxes[1] = NorthEast;
            splittedBoxes[2] = SouthWest;
            splittedBoxes[3] = SouthEast;

            return splittedBoxes;
        }
    }
}