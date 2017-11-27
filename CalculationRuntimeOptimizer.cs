using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace nbody
{
    class CalculationRuntimeOptimizer
    {
        int operationMode = 0;
        SolverData solverData;

        public CalculationRuntimeOptimizer(int operationMode, SolverData solverData)
        {
            this.operationMode = operationMode;
            this.solverData = solverData;
        }

        public void CalculateViaParallelForEach(List<Body> bodies, QuadTreeNode rootNode)
        {
            Parallel.ForEach(bodies, body =>
            {
                if (body != null)
                {
                    rootNode.CalculateNetForceOnBody(body);
                    UpdateBodyPosition(body);
                }
            });
        }

        public void CalculateViaTasks(List<Body> bodies, QuadTreeNode rootNode)
        {

        }

        // Update Body positions based on the current active forces
        private void UpdateBodyPosition(Body body)
        {
            body.acceleration.X = body.ActingForce.X / body.Mass;
            body.acceleration.Y = body.ActingForce.Y / body.Mass;
            body.Velocity.X += body.acceleration.X * solverData.CycleTime;
            body.Velocity.Y += body.acceleration.Y * solverData.CycleTime;

            Point newPos = new Point(body.Position.X + body.Velocity.X * solverData.CycleTime, body.Position.Y + body.Velocity.Y * solverData.CycleTime);
            newPos = WrapPositionBetweenBoundaries(body, newPos);
            body.Position = newPos;
            double fx = body.ActingForce.X;
            double fy = body.ActingForce.Y;
            body.ForceT1m = new Force(fx, fy);
            body.ActingForce = new Force(0, 0);
        }

        // Just a sanity check, to not allow the Bodies to leave the canvas.
        // Sets velocity to zero in the critical direction.
        private Point WrapPositionBetweenBoundaries(Body body, Point newPos)
        {
            if (newPos.X < 0)
            {
                newPos.X = 0;
                body.Velocity.X = 0;
            }
            if (newPos.X > solverData.MaxWidth)
            {
                newPos.X = solverData.MaxWidth;
                body.Velocity.X = 0;
            }

            if (newPos.Y < 0)
            {
                newPos.Y = 0;
                body.Velocity.Y = 0;
            }
            if (newPos.Y > solverData.MaxHeight)
            {
                newPos.Y = solverData.MaxHeight;
                body.Velocity.Y = 0;
            }

            return newPos;
        }



    }
}
