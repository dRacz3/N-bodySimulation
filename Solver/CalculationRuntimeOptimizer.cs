using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace nbody
{
    internal enum CalculationMode
    {
        ParallelForeach = 1,
        TaskBased = 0
    }

    internal class CalculationRuntimeOptimizer
    {
        private readonly CalculationMode operationMode = 0;
        private readonly SolverData solverData;


        public CalculationRuntimeOptimizer(CalculationMode operationMode, SolverData solverData)
        {
            this.operationMode = operationMode;
            this.solverData = solverData;
        }

        public void Calculate(List<Body> bodies, QuadTreeNode rootNode)
        {
            switch (operationMode)
            {
                case CalculationMode.ParallelForeach:
                    CalculateViaParallelForEach(bodies, rootNode);
                    break;
                case CalculationMode.TaskBased:
                    CalculateViaTasks(bodies, rootNode);
                    break;
                default:
                    CalculateViaParallelForEach(bodies, rootNode);
                    break;
            }
        }

        private void CalculateViaParallelForEach(List<Body> bodies, QuadTreeNode rootNode)
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

        private void CalculateViaTasks(List<Body> bodies, QuadTreeNode rootNode)
        {
            int numberOfTasks = 8;

            List<Task> tasks = new List<Task>();
            // Split Body list to sublists..
            List<Body>[] subListsOfBodies = new List<Body>[numberOfTasks];
            for (int i = 0; i < numberOfTasks; i++)
                subListsOfBodies[i] = new List<Body>();

            // We start splitting...
            int counter = 0;
            int arrayIndex = 0;
            while (counter < bodies.Count)
            {
                subListsOfBodies[arrayIndex].Add(bodies[counter]);
                counter++;
                arrayIndex++;
                if (arrayIndex == numberOfTasks)
                    arrayIndex = 0;
            }

            // Process each sublist in sequenctial form 
            for (int i = 0; i < numberOfTasks; i++)
            {
                int j = i;
                tasks.Add(new Task(() =>
                {
                    foreach (Body body in subListsOfBodies[j])
                    {
                        rootNode.CalculateNetForceOnBody(body);
                        UpdateBodyPosition(body);
                    }
                }));
            }

            foreach (Task t in tasks)
                t.Start();

            // Must wait for all... if the calculation takes longer than the next rendering cycle, it will ruin the whole simulation.
            Task.WaitAll(tasks.ToArray());
        }

        // Update Body positions based on the current active forces
        private void UpdateBodyPosition(Body body)
        {
            body.Acceleration.X = body.ActingForce.X / body.Mass;
            body.Acceleration.Y = body.ActingForce.Y / body.Mass;
            body.Velocity.X += body.Acceleration.X * solverData.CycleTime;
            body.Velocity.Y += body.Acceleration.Y * solverData.CycleTime;

            double absVel = Math.Sqrt(body.Velocity.X * body.Velocity.X + body.Velocity.Y * body.Velocity.Y);
            if (absVel > WorldProperties.MaxVelocity)
            {
                double velocityOverride = absVel / WorldProperties.MaxVelocity;
                body.Velocity.X = body.Velocity.X / velocityOverride;
                body.Velocity.Y = body.Velocity.Y / velocityOverride;
            }


            Point newPos = new Point(body.Position.X + body.Velocity.X * solverData.CycleTime,
                body.Position.Y + body.Velocity.Y * solverData.CycleTime);
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