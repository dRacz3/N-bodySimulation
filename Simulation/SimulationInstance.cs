using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        Stopwatch sw;
        int frameCount = 0;
        Object mementoLock = new object();

        public SimulationInstance(double time, double timestep, int bodyCount, CalculationMode mode)
        {  
            dt = timestep;
            _bodyCount = bodyCount;
            // TODO : update calculation mode to be changeable
            _solver = new NBodySolver(WorldProperties.CanvasWidth, WorldProperties.CanvasHeight, bodyCount, timestep, mode);
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
  //              Trace.WriteLine("Doing iteration..." + count + "frameCount is :" + frameCount);
            }
            sw.Stop();
    //        Console.WriteLine("Elapsed={0}", sw.Elapsed);
        }

        public void StopSimulation()
        {

        }

        private void DoOneIteration()
        {
            _solver.CalculateNextPositions();
 //           Trace.WriteLine("Adding memento");
            resultSet.AddMemento(_solver.GetCurrentMemento());

        }

        public SimulationMemento GetNextFrame()
        {
 //           Trace.WriteLine("Giving back iteration :" + resultSet.currentDisplayIndex);
            resultSet.currentDisplayIndex++;
            return resultSet.GetNextMemento();
        }

        public TimeSpan GetSimulationTime()
        {
            return sw.Elapsed;
        }
    }
}
