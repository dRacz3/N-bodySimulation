using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody.BarnesHut
{
    class QuadTreeParser
    {
        static int counter = 0;
        public QuadTreeNode BuildTree(BoundingBox boundingBox, List<Body> bodyList)
        {

            QuadTreeNode root = new QuadTreeNode();
            // Go through each element, check if it's inside the box, if yes add it to the node's list
            counter++;
            foreach(Body element in bodyList)
            {
                if (boundingBox.isInside(element.Position))
                {
                    root.m_cBodyList.Add(element);
                }
            }

            if(root.m_cBodyList.Count > 1) // If it contains more than one... TIME TO SPLIT IT!
            {
                List<BoundingBox> splittedBoxes = Split(boundingBox);
                foreach(BoundingBox box in splittedBoxes)
                {
                    QuadTreeNode subNodes = BuildTree(box, root.m_cBodyList);
                    root.m_cNodeList.Add(subNodes);
                }
            }

            return root;

        }

        public List<BoundingBox> Split(BoundingBox boundingBox)
        {
            List<BoundingBox> splittedBoxes = new List<BoundingBox>();
            double xs = boundingBox.Xmin;
            double xe = boundingBox.Xmax;
            double ys = boundingBox.Ymin;
            double ye = boundingBox.Ymax;

            BoundingBox NorthWest = new BoundingBox(xs + xe / 2 , xs         , ys + ye / 2, ys          );
            BoundingBox NorthEast = new BoundingBox(xs + xe     , xs + xe / 2, ys + ye / 2, ys          );
            BoundingBox SouthWest = new BoundingBox(xs + xe / 2 , xs         , ys + ye    , ys + ye / 2);
            BoundingBox SouthEast = new BoundingBox(xs + xe     , xs + xe / 2, ys + ye    , ys + ye / 2);

            splittedBoxes.Add(NorthWest);
            splittedBoxes.Add(NorthEast);
            splittedBoxes.Add(SouthWest);
            splittedBoxes.Add(SouthEast);

            return splittedBoxes;
        }


    }
}
