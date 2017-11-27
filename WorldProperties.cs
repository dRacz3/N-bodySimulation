namespace nbody
{
    /* This is a static class Containing Simulation properties shared between Parts of the system*/
    internal class WorldProperties
    {
        // Gravity
        public static double G { set; get; }

        public static double CanvasWidth { get; set; }

        public static double CanvasHeight { get; set; }

        // Tolerance value to decide to use for the BarnesHut s/d < theta comparison
        public static double Tolerance { get; set; }

        // Size of the smallest QuadTree box allowed
        public static double MinimumBoundingBoxWidth { get; set; }

        // Maximum allowed velocity for a body
        public static double MaxVelocity { get; set; }
    }
}