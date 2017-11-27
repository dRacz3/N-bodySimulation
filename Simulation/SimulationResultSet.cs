using System.Collections.Generic;

namespace nbody
{
    internal class SimulationResultSet
    {
        private readonly object MementoLock = new object();
        private readonly Queue<SimulationMemento> mementoQueue;


        public SimulationResultSet()
        {
            currentDisplayIndex = 0;
            mementoQueue = new Queue<SimulationMemento>();
        }

        public int currentDisplayIndex { get; set; }


        public SimulationMemento GetNextMemento()
        {
            if (mementoQueue.Count == 1)
                lock (MementoLock)
                {
                    return mementoQueue.Peek();
                }
            if (mementoQueue.Count != 0)
                lock (MementoLock)
                {
                    return mementoQueue.Dequeue();
                }
            return new SimulationMemento();
        }

        public void AddMemento(SimulationMemento memento)
        {
            lock (MementoLock)
            {
                mementoQueue.Enqueue(memento);
            }
        }
    }
}