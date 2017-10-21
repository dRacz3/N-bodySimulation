using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody
{
    class SimulationResultSet
    {
        SimulationMemento[] mementoList;
        int currentDisplayIndex = 0;
        int lastAddedIndex = 0;
        int maxFrames = 0;

        public SimulationResultSet(int numberOfFrames)
        {
            maxFrames = numberOfFrames;
            mementoList = new SimulationMemento[numberOfFrames];
        }


        public SimulationMemento GetNextMemento()
        {
            SimulationMemento tmp;

            if (currentDisplayIndex < maxFrames || mementoList[currentDisplayIndex + 1] == null)
            {
                tmp = mementoList[currentDisplayIndex];
            }
            else
            {
                tmp = mementoList[maxFrames];
            }
            currentDisplayIndex++;
            return tmp;
        }

        public void AddMemento(SimulationMemento memento)
        {
            mementoList[lastAddedIndex] = memento;
            lastAddedIndex++;
        }

    }
}
