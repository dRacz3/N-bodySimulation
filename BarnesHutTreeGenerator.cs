using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace nbody
{


    class BarnesHutTreeGenerator
    {
        public static List<BoundingBox> m_BoundingBoxes = new List<BoundingBox>();
        double eps = 0.5;
        double distTreshold = 15;
        static int counter = 0;

        // This function is responsible for populating the tree
        public static Node PopulateTree(BoundingBox boundingBox, List<Body> bodyList, int N)
        {
            List<Node> subNodes = new List<Node>();
            double[] sizeOfBoundingBox = { Math.Abs(boundingBox.X_max - boundingBox.X_min),
                           Math.Abs(boundingBox.Y_max - boundingBox.Y_min) };
            double smallestSize = sizeOfBoundingBox.Min();
            CenterOfMass center = CalculateCenterOfMass(bodyList);

            counter++;
            BoundingBox[] boxesAndPts = SplitToQuadrants(boundingBox, bodyList);
            if (boxesAndPts[0] == null) // if it's empty ,we can return with what we have
            {
                Node node = new Node(subNodes, center, smallestSize);
                return node;
            }
            else // Else... we must go deeper :)
            {
                foreach (BoundingBox _boundingBox in boxesAndPts)
                {
                    if (_boundingBox != null)
                    {
                        Node resultNode = PopulateTree(_boundingBox, _boundingBox.bodyList, N);
                        subNodes.Add(resultNode);
                        if (resultNode.Bodies != null)
                        {
                            m_BoundingBoxes.Add(_boundingBox);
                            Trace.WriteLine("Boundingbox added...");
                        }
                    }
                }
                return new Node(subNodes, center, smallestSize);
            }
        }

        /*This function calculates the minimum bounding box, based on all elements*/
        public static BoundingBox FindBoundingBox(List<Body> bodyList)
        {
            double x_min = 0.0;
            double y_min = 0.0;
            double x_max = 0.0;
            double y_max = 0.0;
            foreach (Body body in bodyList)
            {
                x_min = Math.Min(x_min, body.Position.X);
                y_min = Math.Min(x_min, body.Position.Y);
                x_max = Math.Max(x_max, body.Position.X);
                y_max = Math.Max(x_max, body.Position.Y);
            }
            return new BoundingBox(x_min, y_min, x_max, y_max);
        }

        /*This function calculates the Center of Mass for the List of Bodies*/
        public static CenterOfMass CalculateCenterOfMass(List<Body> bodyList)
        {
            double x = 0.0;
            double y = 0.0;
            double mass = 0.0;
            foreach (Body body in bodyList)
            {
                x += body.Weight * body.Position.X;
                y += body.Weight * body.Position.Y;
                mass += body.Weight;
            }
            return new CenterOfMass(x / mass, y / mass, mass);
        }


        // This function splits the Bodies into 4 quadrants, 
        // Returns the non-empty BoundingBoxes 
        public static BoundingBox[] SplitToQuadrants(BoundingBox boundingBox, List<Body> bodyList)
        {
            int quadrants = 4;
            BoundingBox[] box = new BoundingBox[quadrants];
            double middle_x = (boundingBox.X_max + boundingBox.X_min) / 2;
            double middle_y = (boundingBox.Y_max + boundingBox.Y_min) / 2;

            // parameter reminder for BoundingBox constructor : xmin ymin xmax ymax
            box[0] = new BoundingBox(boundingBox.X_min, boundingBox.Y_min, middle_x, middle_y); // NorthWest
            box[1] = new BoundingBox(middle_x, boundingBox.Y_min, middle_x * 2, middle_y); // NorthEast
            box[2] = new BoundingBox(boundingBox.X_min, middle_y, middle_x, middle_y * 2); // SouthWest
            box[3] = new BoundingBox(middle_x, middle_y, middle_x * 2, middle_y * 2); // SouthEast


            foreach (Body body in bodyList)
            {
                for (int i = 0; i < quadrants; i++)
                {
                    if (box[i].isInBox(body))
                    {
                        box[i].bodyList.Add(body);
                        // TODO: if we added into one list.. maybe we can remove it from the other to save time
                    }
                }
            }
            // Checking if a box is empty... If contains at least 1 element, we return it.
            BoundingBox[] finalBoundingBox = new BoundingBox[quadrants];
            for (int i = 0; i < quadrants; i++)
            {
                if (box[i].bodyList.Count != 0)
                {
                    finalBoundingBox[i] = (box[i]);
                }
            }
            return finalBoundingBox;
        }
    }
}