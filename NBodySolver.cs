using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;

namespace nbody
{

    class NBodySolver
    {
        List<Body> _bodies;
        double _maxWidth;
        double _maxHeight;
        double _dt;
        int _bodyCount;
        public QuadTreeNode rootNode { get; set; }



        // Default constructor
        public NBodySolver(double maxWidth, double maxHeight, int numberOfBodies, double cycleTime)
        {
            this._bodies = new List<Body>();
            this._maxWidth = maxWidth;
            this._maxHeight = maxHeight;
            this._bodyCount = numberOfBodies;
            this._dt = cycleTime / 1000;
            GenerateBodies();
        }

        // Facade function to call calculations
        public void CalculateNextPositions()
        {
            BuildQuadTree();
            CalculateForceAndNextPosition();
        }

        // Function used to Calculate the Net Acting force & next position on each body
        private void CalculateForceAndNextPosition()
        {
            Parallel.ForEach(_bodies, body =>
            {
                if (body != null)
                {
                    rootNode.CalculateNetForceOnBody(body);
                    UpdateBodyPosition(body);
                }
            });
        }

        // Update Body positions based on the current active forces
        private void UpdateBodyPosition(Body body)
        {
            body.acceleration.X = body.ActingForce.X / body.Mass;
            body.acceleration.Y = body.ActingForce.Y / body.Mass;
            body.Velocity.X += body.acceleration.X * _dt;
            body.Velocity.Y += body.acceleration.Y * _dt;

            Point newPos = new Point(body.Position.X + body.Velocity.X * _dt, body.Position.Y + body.Velocity.Y * _dt);
            newPos = WrapPositionBetweenBoundaries(body, newPos);
            body.Position = newPos;
            double fx = body.ActingForce.X;
            double fy = body.ActingForce.Y;
            body.ForceT1m = new Force(fx, fy);
            body.ActingForce = new Force(0, 0);
        }

        // Just a sanity check, to not allow the Bodies to leave the canvas.
        // Sets velocity to zero in the critical direction.
        private Point WrapPositionBetweenBoundaries(Body body, Point newPos)
        {
            if (newPos.X < 0)
            {
                newPos.X = 0;
                body.Velocity.X = 0;
            }
            if (newPos.X > _maxWidth)
            {
                newPos.X = _maxWidth;
                body.Velocity.X = 0;
            }

            if (newPos.Y < 0)
            {
                newPos.Y = 0;
                body.Velocity.Y = 0;
            }
            if (newPos.Y > _maxHeight)
            {
                newPos.Y = _maxHeight;
                body.Velocity.Y = 0;
            }

            return newPos;
        }

        // Build the quadtre..
        private void BuildQuadTree()
        {
            BoundingBox rootBox = new BoundingBox(_maxWidth, 0, _maxHeight, 0);
            rootNode = new QuadTreeNode();
            rootNode.boundingBox = rootBox;
            foreach (Body body in _bodies)
            {
                rootNode.AddBody(body);
            }
        }

        // Replacable function. Generates a set of bodies based on random numbers.
        private void GenerateBodies()
        {
            Random rnd = new Random();
            double canvasWidth = _maxWidth;
            double canvasHeight = _maxHeight;

            Body Sun = new Body();
            Sun.Position = new Point(canvasWidth / 3, canvasHeight / 2);
            Sun.Mass = 10000;
            Sun.Size = 50;
            Sun.solidColor = Brushes.Yellow;
            //_bodies.Add(Sun);

            for (int j = 0; j < _bodyCount; j++)
            {
                Body body = new Body();
                body.Position = new Point(rnd.Next(1, (int)canvasWidth), rnd.Next(1, (int)canvasHeight));
                body.Mass = (rnd.Next(10, 500)) + 1;
                body.Size = body.Mass / 40;
                _bodies.Add(body);
            }

        }

        // Return the current list of living bodies inside the simulation
        public List<Body> getBodies()
        {
            return _bodies;
        }

        // Override the desired number of bodies in the simulation
        public void ChangeNumberOfBodies(int newNumber)
        {
            _bodies.Clear();
            _bodyCount = newNumber;
            GenerateBodies();
        }

        public SimulationMemento GetCurrentMemento()
        {
            SimulationMemento memento = new SimulationMemento(_bodies.Count, 0);
            foreach (Body body in _bodies) // TODO : Maybe use parallel here?
            {
                memento.AddBody(body);
            }
            /*TODO : same thing for bounding boxes */
            return memento;
        }


    }
}
