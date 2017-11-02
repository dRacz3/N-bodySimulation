using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace nbody
{

    public partial class MainWindow : Window
    {
        NBodySolver _solver;
        DispatcherTimer _timer;
        double dt = 10; // ms;
        int _bodyCount;
        SimulationInstance simulation;
        Stopwatch TotalSimulationTimeStopWatch;
        bool isCalculationModeSelected;
        int SimulationModeId = 0;


        public MainWindow()
        {
            InitializeComponent();
            _bodyCount = 100;
            WorldProperties.G = 9.81; // m/s^2
            WorldProperties.Tolerance = 0.5;  
            WorldProperties.MinimumBoundingBoxWidth = 15;
            WorldProperties.MaxVelocity = 250;
        }

#region Business Logic

        private void ClearCanvas()
        {
            MainCanvas.Children.Clear();
        }

        private void PlotBodiesToCanvas(List<Body> points)
        {
            MainCanvas.Children.Clear();
            Ellipse[] ellipsePoints = new Ellipse[points.Count];

            for (int j = 0; j < points.Count; j++)
            {
                ellipsePoints[j] = new Ellipse() { Width = points[j].Size, Height = points[j].Size, Fill = points[j].SolidColor };
                MainCanvas.Children.Add(ellipsePoints[j]);
            }

            for (int i = 0; i < points.Count; i++)
            {
                Canvas.SetLeft(ellipsePoints[i], points[i].Position.X - ellipsePoints[i].Width / 2);
                Canvas.SetTop(ellipsePoints[i], points[i].Position.Y - ellipsePoints[i].Height / 2);
            }
        }


        private void PlotForceVector(List<Body> bodies)
        {
            foreach (Body body in bodies)
            {
                Line myLine = new Line();

                myLine.Stroke = System.Windows.Media.Brushes.Green;
                myLine.X1 = body.Position.X;
                myLine.X2 = body.Position.X + body.ForceT1m.X / 100;
                myLine.Y1 = body.Position.Y;
                myLine.Y2 = body.Position.Y + body.ForceT1m.Y / 100;
                myLine.StrokeThickness = 1;

                MainCanvas.Children.Add(myLine);
            }

        }

        private void PlotBoundingBox(QuadTreeNode root)
        {
            QuadTreeNode[] sub = root.GetSubNodes();
            RecursivePrintBoundingBoxes(root);
        }

        private QuadTreeNode[] RecursivePrintBoundingBoxes(QuadTreeNode node)
        {
            if (node.BodyCount == 0)
            {
                return null;
            }
            else
            {
                BoundingBox box = node.BoundingBox;
                Rectangle rect = new Rectangle();
                rect.Stroke = new SolidColorBrush(Colors.Red);
                rect.Width = box.Xmax - box.Xmin;
                rect.Height = box.Ymax - box.Ymin;
                Canvas.SetLeft(rect, box.Xmin);
                Canvas.SetTop(rect, box.Ymin);
                MainCanvas.Children.Add(rect);

                QuadTreeNode[] subNodeArray = node.GetSubNodes();
                if (subNodeArray != null)
                {
                    foreach (QuadTreeNode subnode in subNodeArray)
                    {
                        RecursivePrintBoundingBoxes(subnode);
                    }
                }
            }
            return node.subNodes;
        }

        private void RenderNextFrame(object sender, EventArgs e)
        {
            if (isCalculationModeSelected)
            {
                if (SimulationModeId == 0)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    _solver.CalculateNextPositions();
                    PlotBodiesToCanvas(_solver.GetBodies());

                    if ((bool)chckbx_Grid.IsChecked)
                    {
                        PlotBoundingBox(_solver.RootNode);
                    }
                    if ((bool)chckbx_Force.IsChecked)
                    {
                        PlotForceVector(_solver.GetBodies());
                    }
                    sw.Stop();

                    label_SimTime.Content = sw.Elapsed.ToString();
                    label_TotalSimTime.Content = TotalSimulationTimeStopWatch.Elapsed.ToString();
                }
                else if (SimulationModeId == 1)
                {
                    PlotBodiesToCanvas(simulation.GetNextFrame().BodyList);
                    label_SimTime.Content = simulation.GetSimulationTime().ToString();
                    label_TotalSimTime.Content = TotalSimulationTimeStopWatch.Elapsed.ToString();

                }
            }
        }
#endregion

#region Events

        /************************************
         * UI EVENTS
         ************************************/

        // Create Solver + Start callback timer 
        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            WorldProperties.CanvasWidth = MainCanvas.ActualWidth;
            WorldProperties.CanvasHeight = MainCanvas.ActualHeight;
            _solver = new NBodySolver(MainCanvas.ActualWidth, MainCanvas.ActualHeight, _bodyCount, dt, CalculationMode.ParallelForeach);
            TotalSimulationTimeStopWatch = new Stopwatch();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(dt);
            _timer.Tick += RenderNextFrame;
            _timer.Start();

            txtbox_bodycount.IsEnabled = true;
            txtbox_gravity.IsEnabled = true;
            btn_update.IsEnabled = true;

            btn_SwitchToFt.IsEnabled = true;
            btn_SwtichToRt.IsEnabled = true;
            String value = String.Format("Tolerance: {0}", WorldProperties.Tolerance);
            label_Tolerance.Content = value;
        }


        // Textbox input functions for changing body count
        private void CheckIfOnlyNumberWasEntered(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _bodyCount = int.Parse(txtbox_bodycount.Text);
            WorldProperties.G = (double.Parse(txtbox_gravity.Text));
            ClearCanvas();
            if (_solver != null)
            {
                _solver.ChangeNumberOfBodies(_bodyCount);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WorldProperties.Tolerance = e.NewValue;

            if (label_Tolerance != null)
            {
                String value = String.Format("Tolerance:{0}", WorldProperties.Tolerance);
                label_Tolerance.Content = value;
            }
        }

        private void Button_Swtich_To_FixedTime(object sender, RoutedEventArgs e)
        {
            // Creating a simulation instance..
            label_SimMode.Content = "Fixed Time";
            SimulationModeId = 1;
            int selectedMode = fixMode_Selector.SelectedIndex;
            Trace.WriteLine("Selected mode is: " + selectedMode);
            simulation = new SimulationInstance(double.Parse(txtbox_SimTime.Text), dt, _bodyCount, (CalculationMode) selectedMode);

            // Launching a task for it, that will update the sim time label after it is finished
            Task simulationTask = new Task(simulation.StartSimulation);
            simulationTask.Start();
            label_CalcTimeInfo.Content = "Elapsed time since simulation start";
            TotalSimulationTimeStopWatch.Reset();
            TotalSimulationTimeStopWatch.Start();
            chckbx_Grid.IsEnabled = false;
            chckbx_Grid.IsChecked = false;
            chckbx_Force.IsEnabled = false;
            chckbx_Force.IsChecked = false;
            isCalculationModeSelected = true;
        }

        private void Button_Switch_To_Realtime(object sender, RoutedEventArgs e)
        {
            SimulationModeId = 0;
            label_SimMode.Content = "RealTime mode";
            label_CalcTimeInfo.Content = "One iteration took";
            chckbx_Grid.IsEnabled = true;
            chckbx_Force.IsEnabled = true;
            TotalSimulationTimeStopWatch.Reset();
            TotalSimulationTimeStopWatch.Start();
            isCalculationModeSelected = true;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("This program has been developed by Dániel Rácz \n");
            sb.Append("This piece of software is an N-body simulator, representing an arbitrary number of bodeis in 2 Dimensional space. \n");
            sb.Append("1, Press start to initialize the environment. \n");
            sb.Append("2, Press any of the simulation mode switch button to start execution. Parameters can be changed during simulation. \n");
            sb.Append("Please note, Fixed time simulation does not support changing properties during runtime, only before starting the calculation \n");
            sb.Append("The additional feature for visualising BoundingBoxes and Forces on bodies only available on realtime simulation mode. \n");
           
            MessageBox.Show(sb.ToString());
        }
    }
#endregion
}
