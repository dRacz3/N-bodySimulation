using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody
{
    class CenterOfMass
    {
        public Position cPosition { get; }
        public double Mass { get; }

        public CenterOfMass(double x, double y, double m)
        {
            cPosition = new Position(x, y);
            Mass = m;
        }
    }


    class Node
    {
        public List<Node> SubNodes { get; }
        public List<Body> Bodies { get; }
        public CenterOfMass CenterOfMass { get; }

        public double Size { get; }

        public Node(List<Node> subNodes, CenterOfMass centerOfMass, double size)
        {
            this.SubNodes = subNodes;
            this.CenterOfMass = centerOfMass;
            this.Size = size;
        }

        public Node(List<Node> subNodes, List<Body> bodies, CenterOfMass centerOfMass, double size)
        {
            this.Bodies = bodies;
            this.SubNodes = subNodes;
            this.CenterOfMass = centerOfMass;
            this.Size = size;
        }


    }
}
