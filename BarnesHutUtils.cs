using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody
{
    class BoundingBox
    {

        public List<Body> bodyList;
        public double X_min { get; set; }
        public double Y_min { get; set; }
        public double X_max { get; set; }
        public double Y_max { get; set; }
        public BoundingBox(double minx, double miny, double maxx, double maxy)
        {
            bodyList = new List<Body>();
            this.X_min = minx;
            this.Y_min = miny;
            this.X_max = maxx;
            this.Y_max = maxy;
        }

        // Check if The Body is inside the boundingBox's limits
        public bool isInBox(Body body)
        {
            if ((body.Position.X <= X_max)
                && (body.Position.X > X_min)
                && (body.Position.Y > Y_min)
                && (body.Position.Y <= Y_max))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
