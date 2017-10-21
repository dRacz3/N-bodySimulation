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


namespace nbody
{

    public partial class MainWindow : Window
    {

        NBodySolver _solver;
        DispatcherTimer _timer;
        double dt = 10; // ms;
        int _bodyCount;
        SimulationInstance simulation;


        public MainWindow()
        {
            InitializeComponent();
            _bodyCount = 50;
            WorldProperties.G = 9.81;
            WorldProperties.Tolerance = 0.5;
            WorldProperties.MinimumBoundingBoxWidth = 5;
        }   

        private void ResetSimulation()
        {
            MainCanvas.Children.Clear();
            
            double X_max = MainCanvas.ActualWidth;
            double Y_max = MainCanvas.ActualHeight;


        }

        private void PlotBodiesToCanvas(List<Body> points)
        {
            MainCanvas.Children.Clear();
            Ellipse[] ellipsePoints = new Ellipse[points.Count];

            for (int j = 0; j < points.Count; j++)
            {
                ellipsePoints[j] = new Ellipse() { Width = points[j].Size, Height = points[j].Size, Fill = points[j].solidColor };
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
            foreach(Body body in bodies)
            {
                Line myLine = new Line();

                myLine.Stroke = System.Windows.Media.Brushes.Green;
                myLine.X1 = body.Position.X;
                myLine.X2 = body.Position.X + body.ForceT1m.X/100 ;  
                myLine.Y1 = body.Position.Y;
                myLine.Y2 = body.Position.Y + body.ForceT1m.Y/100;
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
                BoundingBox box = node.boundingBox;
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

        private void renderNextFrame(object sender, EventArgs e)
        {
            _solver.CalculateNextPositions();
            PlotBodiesToCanvas(_solver.getBodies());

            if((bool)chckbx_Grid.IsChecked)
            {
                PlotBoundingBox(_solver.rootNode);
            }
            if((bool)chckbx_Force.IsChecked)
            {
                PlotForceVector(_solver.getBodies());
            }
        }

        /************************************
         * UI EVENTS
         ************************************/

        // Stop timer (if started) + Reset simulation
        private void btn_randomize_Click(object sender, RoutedEventArgs e)
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            ResetSimulation();
        }

        // Create Solver + Start callback timer 
        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            WorldProperties.CanvasWidth = MainCanvas.ActualWidth;
            WorldProperties.CanvasHeight = MainCanvas.ActualHeight;

            simulation = new SimulationInstance(30, dt, _bodyCount);
            _solver = new NBodySolver(MainCanvas.ActualWidth, MainCanvas.ActualHeight, _bodyCount, dt);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(dt);
            _timer.Tick += renderNextFrame;
            _timer.Start();
            btn_stop.IsEnabled = true;
            txtbox_bodycount.IsEnabled = true;
            txtbox_gravity.IsEnabled = true;
            btn_update.IsEnabled = true;
        }

        // Stop / Continue timer
        private void btn_stop_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
                btn_stop.Content = "Continue";
            }
            else
            {
                _timer.Start();
            }
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
            ResetSimulation();
            if (_solver != null)
            {
                _solver.ChangeNumberOfBodies(_bodyCount);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            WorldProperties.Tolerance = e.NewValue;

            if (label_Tolerance!=null)
            {
                String value = String.Format("Tolerance:{0}", WorldProperties.Tolerance);
                label_Tolerance.Content = value;
            }
        }
    }
}
