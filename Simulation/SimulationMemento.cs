using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody
{
    class SimulationMemento
    {
        public int BodyCount { get; }

        public SimulationMemento()
        {
            BodyList = new List<Body>();
        }

        // x , y, size Times number of bodies
        public List<Body> BodyList { get; set; }


        public void AddBody(Body body)
        {
                BodyList.Add(body);
        }


    }

}
