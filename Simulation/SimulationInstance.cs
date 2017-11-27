using System;
using System.Diagnostics;

namespace nbody
{
    internal class SimulationInstance
    {
        private int _bodyCount;
        private readonly NBodySolver _solver;
        private double dt = 10; // ms;
        private readonly int frameCount;
        private object mementoLock = new object();
        private readonly SimulationResultSet resultSet;
        private Stopwatch sw;

        public SimulationInstance(double time, double timestep, int bodyCount, CalculationMode mode)
        {
            dt = timestep;
            _bodyCount = bodyCount;
            // TODO : update calculation mode to be changeable
            _solver = new NBodySolver(WorldProperties.CanvasWidth, WorldProperties.CanvasHeight, bodyCount, timestep,
                mode);
            frameCount = Convert.ToInt32(time * 1000 / timestep);
            resultSet = new SimulationResultSet();
        }

        public void ResetSimulation()
        {
        }

        public void StartSimulation()
        {
            sw = new Stopwatch();
            sw.Start();
            int count = 0;
            for (int i = 0; i < frameCount; i++)
            {
                DoOneIteration();
                count++;
            }
            sw.Stop();
        }

        public void StopSimulation()
        {
        }

        private void DoOneIteration()
        {
            _solver.CalculateNextPositions();
            resultSet.AddMemento(_solver.GetCurrentMemento());
        }

        public SimulationMemento GetNextFrame()
        {
            resultSet.currentDisplayIndex++;
            return resultSet.GetNextMemento();
        }

        public TimeSpan GetSimulationTime()
        {
            return sw.Elapsed;
        }
    }
}