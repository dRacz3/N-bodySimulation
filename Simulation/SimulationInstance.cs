using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody
{
    class SimulationInstance
    {
        NBodySolver _solver;
        double dt = 10; // ms;
        int _bodyCount;
        SimulationResultSet resultSet;

        public SimulationInstance(double time, double timestep, int bodyCount)
        {
            dt = timestep;
            _bodyCount = bodyCount;
            _solver = new NBodySolver(WorldProperties.CanvasWidth, WorldProperties.CanvasHeight, bodyCount, timestep);
            int frameCount = Convert.ToInt32(time / timestep);
            resultSet = new SimulationResultSet(frameCount);
        }

        public void ResetSimulation()
        {

        }

        public void StartSimulation()
        {

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
            return resultSet.GetNextMemento();
        }

    }
}
