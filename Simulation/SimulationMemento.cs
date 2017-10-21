using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody
{
    // Lightweight data storage for a bodyProperty
    class BodyProperty
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Size { get; set; }
    }

    class SimulationMemento
    {
        int AdditionIndex = 0;
        int maxBodyNum = 0;
        int maxBBoxNum = 0;

        public SimulationMemento(int bodyNum, int bboxNum)
        {
            maxBodyNum = bodyNum;
            maxBBoxNum = bboxNum;
            BodyProperties = new BodyProperty[bodyNum];
            BoundingBoxProperties = new double[bboxNum, bboxNum, bboxNum, bboxNum, bboxNum];
        }

        // x , y, size Times number of bodies
        BodyProperty[] BodyProperties;
        // xs, xe, ys, ye
        double[,,,,] BoundingBoxProperties;

        public void AddBody(Body body)
        {
            if (AdditionIndex >= maxBodyNum)
            {
                return;
            }
            else
            {
                BodyProperty tmp = new BodyProperty();
                tmp.X = body.Position.X;
                tmp.Y = body.Position.Y;
                tmp.Size = body.Size;
                BodyProperties[AdditionIndex] = tmp;
            }
        }

        public BodyProperty[] GetBodyProperties()
        {
            return BodyProperties;
        }


    }

}
