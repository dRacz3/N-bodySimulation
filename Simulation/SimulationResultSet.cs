using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody
{
    class SimulationResultSet
    {
        Queue<SimulationMemento> mementoQueue;
        public int currentDisplayIndex { get; set; }

        object MementoLock = new object();


        public SimulationResultSet()
        {
            currentDisplayIndex = 0;
            mementoQueue = new Queue<SimulationMemento>();
        }


        public SimulationMemento GetNextMemento()
        {
            if (mementoQueue.Count == 1)
            {
                lock (MementoLock)
                {
                    return mementoQueue.Peek();
                }
            }
            else if(mementoQueue.Count != 0)
            {
                lock (MementoLock)
                {
                    return mementoQueue.Dequeue();
                }
            }
            else
            {
                return new SimulationMemento();
            }
        }

        public void AddMemento(SimulationMemento memento)
        {
            lock(MementoLock)
            {
                mementoQueue.Enqueue(memento);
            }
        }

    }
}
