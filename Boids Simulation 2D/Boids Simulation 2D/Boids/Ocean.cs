using System;
using System.Collections.Generic;

namespace Project.Boids
{
    public delegate void OnOceanUpdated(Boid[] boids, List<Obstacle> obstacles);

    public class Ocean
    {
        public OnOceanUpdated OnOceanUpdated;

        Boid[] _boids = null;
        List<Obstacle> _obstacles = null;

        Random _randomGenerator;

        protected double MaxWidth;
        protected double MaxHeight;

        public Ocean(int nbBoids, double maxWidth, double maxHeight)
        {
            MaxWidth = maxWidth;
            MaxHeight = maxHeight;
            _randomGenerator = new Random();
            _boids = new Boid[nbBoids];
            _obstacles = new List<Obstacle>();

            for (int i = 0; i < nbBoids; i++)
            {
                float alea = (float)_randomGenerator.NextDouble();
                _boids[i] = new Boid
                                    (
                                    alea * MaxWidth, 
                                    alea * MaxHeight,
                                    alea * 2f * Math.PI
                                    );
            }
        }

        public void AddObstacle(double posX, double posY, float radius)
        {
            _obstacles.Add(new Obstacle(posX, posY, radius));
        }

        public void UpdateObstacles()
        {
            for (int i = 0; i < _obstacles.Count; i++)
            {
                _obstacles[i].Update();
            }
            _obstacles.RemoveAll(o => o.IsDead());
        }

        public void UpdateBoids()
        {
            for (int i = 0; i < _boids.Length; i++)
            {
                _boids[i].Update(_boids, _obstacles, MaxWidth, MaxHeight);
            }
        }

        public void UpdateEnvironment()
        {
            UpdateObstacles();
            UpdateBoids();
            OnOceanUpdated?.Invoke(_boids, _obstacles);

        }
    }
}