using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Project.Boids;

namespace Boids_Simulation_2D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Ocean _ocean;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        public void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            OceanCanvas.MouseDown += OceanCanvas_MouseDown;

            _ocean = new Ocean(250, OceanCanvas.ActualWidth, OceanCanvas.ActualHeight);
            _ocean.OnOceanUpdated += OnOceanUpdated;

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            dispatcherTimer.Start();
        }

        private void OnOceanUpdated(Boid[] boids, List<Obstacle> obstacles)
        {
            OceanCanvas.Children.Clear();

            foreach (Boid boid in boids)
            {
                DrawBoid(boid);
            }

            foreach (Obstacle obstacle in obstacles)
            {
                DrawObstacle(obstacle);
            }

            OceanCanvas.UpdateLayout();

        }

        private void OceanCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _ocean.AddObstacle(e.GetPosition(OceanCanvas).X, e.GetPosition(OceanCanvas).Y, 10);
        }

        private void DispatcherTimer_Tick(object? sender, EventArgs e)
        {
            _ocean.UpdateEnvironment();
        }


        private void DrawBoid(Boid boid)
        {
            Line body = new();
            body.Stroke = Brushes.Black;
            body.X1 = boid.PosX;
            body.Y1 = boid.PosY;
            body.X2 = boid.PosX - 10 * boid.SpeedX;
            body.Y2 = boid.PosY - 10 * boid.SpeedY;
            OceanCanvas.Children.Add(body);
        }

        private void DrawObstacle(Obstacle obstacle)
        {
            Ellipse circle = new();
            circle.Stroke = Brushes.Black;
            circle.Width = circle.Height = 2 * obstacle.Radius;
            circle.Margin = new Thickness(obstacle.PosX - obstacle.Radius, obstacle.PosY - obstacle.Radius, 0, 0);
            OceanCanvas.Children.Add(circle);
        }
    }
}
