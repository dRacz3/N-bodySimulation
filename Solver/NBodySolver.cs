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
    class SolverData
    {
        public SolverData(double maxWidth, double maxHeight, int numberOfBodies, double cycleTime)
        {
            BodyCount = numberOfBodies;
            MaxHeight = maxHeight;
            MaxWidth = maxWidth;
            CycleTime = cycleTime;
        }

        public double MaxWidth { get;  }
        public double MaxHeight { get;  }
        public double CycleTime { get; }
        public int BodyCount { get; set; }
    }


    class NBodySolver
    {
        List<Body> _bodies;
        SolverData solverData;
        CalculationRuntimeOptimizer runtimeOptimizer;

        public QuadTreeNode RootNode { get; set; }



        // Default constructor
        public NBodySolver(double maxWidth, double maxHeight, int numberOfBodies, double cycleTime, CalculationMode calculationMode)
        {
            solverData = new SolverData(maxWidth,maxHeight,numberOfBodies,cycleTime / 1000);
            this._bodies = new List<Body>();
            GenerateBodies();
            runtimeOptimizer = new CalculationRuntimeOptimizer(calculationMode, solverData);
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
            runtimeOptimizer.Calculate(_bodies, RootNode);
        }

  

        // Build the quadtre..
        private void BuildQuadTree()
        {
            BoundingBox rootBox = new BoundingBox(solverData.MaxWidth, 0, solverData.MaxHeight, 0);
            RootNode = new QuadTreeNode();
            RootNode.BoundingBox = rootBox;
            foreach (Body body in _bodies)
            {
                RootNode.AddBody(body);
            }
        }

        // Replacable function. Generates a set of bodies based on random numbers.
        private void GenerateBodies()
        {
            Random rnd = new Random();
            double canvasWidth = solverData.MaxWidth;
            double canvasHeight = solverData.MaxHeight;

            Body Sun = new Body();
            Sun.Position = new Point(canvasWidth / 3, canvasHeight / 2);
            Sun.Mass = 10000;
            Sun.Size = 50;
            Sun.SolidColor = Brushes.Yellow;
            _bodies.Add(Sun);

            for (int j = 0; j < solverData.BodyCount; j++)
            {
                Body body = new Body();
                body.Position = new Point(rnd.Next(1, (int)canvasWidth), rnd.Next(1, (int)canvasHeight));
                body.Mass = (rnd.Next(10, 500)) + 1;
                body.Size = body.Mass / 40;
                _bodies.Add(body);
            }

        }

        // Return the current list of living bodies inside the simulation
        public List<Body> GetBodies()
        {
            return _bodies;
        }

        // Override the desired number of bodies in the simulation
        public void ChangeNumberOfBodies(int newNumber)
        {
            _bodies.Clear();
            solverData.BodyCount = newNumber;
            GenerateBodies();
        }

        public SimulationMemento GetCurrentMemento()
        {
            SimulationMemento memento = new SimulationMemento();
            foreach (Body body in _bodies) // TODO : Maybe use parallel here?
            {
                memento.AddBody(body.Copy());
            }
            return memento;
        }


    }
}
