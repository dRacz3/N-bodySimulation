using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nbody
{

    // TODO: IMPLEMENT IT
    // Collision checker
    class CollisionCalculator
    {
        static public int CheckForCollision(Body b1, List<Body> bodyList)
        {
            for(int i = 0;i < bodyList.Count; i++)
            {
                if(CheckForCollision(b1, bodyList[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        static public bool CheckForCollision(Body b1, Body b2)
        {
            double distance = CalculatorUtils.CalculateDistance(b1.Position, b2.Position);
            if (distance < (b1.Size + b2.Size))
            {
                return true;
            }
            return false;
        }

        static public Body CalculateMomentumBetweenCollidingObjects(Body b1, Body b2)
        {
            return new Body();
        }
    }
}
