﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace nbody
{
    // Representation of a cluster's main properties
    internal class Centroid
    {
        public Centroid(double x, double y, double mass)
        {
            X = x;
            Y = y;
            Mass = mass;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Mass { get; set; }
    }

    // This class acts as a node in the BarnesHut algorithm.
    internal class QuadTreeNode
    {
        // List of bodies inside..
        public List<Body> Bodies = new List<Body>();

        // Checking the number of elements under the node (counting children as well)
        public int BodyCount;

        // First body is stored separately.. if it's not null, every other body will be put into subtree
        private Body firstBody;

        // Subnodes..
        public QuadTreeNode[] subNodes;

        // Default constructor , initializez everything to zero
        public QuadTreeNode()
        {
            CenterOfMass = new Centroid(0, 0, 0);
            BoundingBox = new BoundingBox(0, 0, 0, 0);
        }

        // Constructor to ease setting the BoundingBox for the Node
        public QuadTreeNode(BoundingBox box)
        {
            CenterOfMass = new Centroid(0, 0, 0);
            BoundingBox = box;
        }

        // Property containing the position information of the Node box
        public BoundingBox BoundingBox { get; set; }

        // Simple representation of the node for calculations to hide cluster below
        public Centroid CenterOfMass { get; set; }

        // Returns the array of subnodes
        public QuadTreeNode[] GetSubNodes()
        {
            return subNodes;
        }


        // Add body to Node, if there are more than 1 bodies under the node, they will be placed into subtrees
        public void AddBody(Body body)
        {
            double totalX = CenterOfMass.X * CenterOfMass.Mass + body.Position.X * body.Mass;
            double totalY = CenterOfMass.Y * CenterOfMass.Mass + body.Position.Y * body.Mass;
            double totalMass = CenterOfMass.Mass + body.Mass;
            CenterOfMass = new Centroid(totalX / totalMass, totalY / totalMass, totalMass);

            CenterOfMassSanityCheck();

            BodyCount++;
            if (BodyCount == 1)
            {
                firstBody = body;
                AddToSubTree(firstBody);
            }
            else
            {
                AddToSubTree(body);
                if (BodyCount == 2)
                    AddToSubTree(firstBody);
            }
        }

        // Helper function to signal error if an impossible CenterOfMass Position is calculated..
        private void CenterOfMassSanityCheck()
        {
            if (CenterOfMass.X > WorldProperties.CanvasWidth)
                Trace.WriteLine("Error! Center of mass X:" + CenterOfMass.X);
            if (CenterOfMass.Y > WorldProperties.CanvasWidth)
                Trace.WriteLine("Error Center of mass Y:" + CenterOfMass.Y);
            if (CenterOfMass.Y < 0)
                Trace.WriteLine("Error Center of mass Y:" + CenterOfMass.Y);
            if (CenterOfMass.X < 0)
                Trace.WriteLine("Error Center of mass Y:" + CenterOfMass.X);
        }

        // Pass a body to a subtree. Creating the subtrees if they do not exist,
        public void AddToSubTree(Body body)
        {
            double subtreeWidth = Math.Abs(BoundingBox.Xmax - BoundingBox.Xmin);
            double MinimumWidth = WorldProperties.MinimumBoundingBoxWidth;
            // Don't create subtrees if it violates the width limit.
            if (subtreeWidth < MinimumWidth)
                return;

            if (subNodes == null) // If subNode does not exist... Create it!
            {
                subNodes = new QuadTreeNode[4];
                // Create boxes & assign them to the new nodes
                BoundingBox[] subBoxes = BoundingBox.Split(BoundingBox);
                for (int i = 0; i < 4; i++)
                    subNodes[i] = new QuadTreeNode(subBoxes[i]);
            }
            else
            {
                foreach (QuadTreeNode subnode in subNodes)
                    if (subnode.BoundingBox.isInside(body.Position))
                        subnode.AddBody(body);
            }
        }


        // Calculate the net acting force on the given body
        public void CalculateNetForceOnBody(Body body)
        {
            double distance = CalculatorUtils.CalculateDistance(
                body.Position,
                new Point(CenterOfMass.X, CenterOfMass.Y));
            double Width = BoundingBox.Xmax - BoundingBox.Xmin;
            double Height = BoundingBox.Ymax - BoundingBox.Ymin;
            // There's only one body in the node      || s/d < omega -> the Node is far away enough to treat it as a single object
            if (BodyCount == 1 && body != firstBody || Width * Height / (distance * distance) <
                WorldProperties.Tolerance * WorldProperties.Tolerance)
            {
                Force ActingForce = ForceCalculator.CalculateForce(CenterOfMass, body);
                body.ActingForce.X += ActingForce.X;
                body.ActingForce.Y += ActingForce.Y;
            }
            // Previous conditions not met. We must go deeper..
            else if (subNodes != null)
            {
                Parallel.ForEach(subNodes, subtree =>
                {
                    if (subtree != null)
                        subtree.CalculateNetForceOnBody(body);
                });
            }
        }
    }
}