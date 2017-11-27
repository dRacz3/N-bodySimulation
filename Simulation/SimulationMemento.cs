using System.Collections.Generic;

namespace nbody
{
    internal class SimulationMemento
    {
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