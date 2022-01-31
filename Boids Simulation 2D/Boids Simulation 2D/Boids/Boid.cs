using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Project.Boids
{
    public class Boid : ObjectInWorld
    {
        protected const double STEP = 3;               //Distance parcourue à chaque itération
        protected const double DST_EVITE = 5;          //Distance d'évitement entre chaque boid
        protected const double SQR_DST_EVITE = 25;     //Distance au carrée, mise en cost pour optimiser les calculs
        protected const double DST_ALIGNE = 40;        //Distance d'alignement entre chaque boid
        protected const double SQR_DST_ALIGNE = 1600;  //Distance au carrée, mise en cost pour optimiser les calculs
        protected const double SPD_CHANGE_DIR = .3;    //Delta de changement de la vitesse si le boid rencontre un obstacle

        public double SpeedX { get; protected set; }
        public double SpeedY { get; protected set; }

        internal Boid(double posX, double posY, double dir) : base(posX, posY)
        {
            PosX = posX;
            PosY = posY;
            SpeedX = Math.Cos(dir);
            SpeedY = Math.Sin(dir);
        }

        internal void UpdatePos()
        {
            PosX += STEP * SpeedX;
            PosY += STEP * SpeedY;
        }

        /// <summary>
        /// Vérifie que deux boids soient dans leurs zones d'alignement.
        /// </summary>
        private bool Near(Boid other)
        {
            double sqrDst = SqrDistanceTo(other);
            return sqrDst < SQR_DST_ALIGNE && sqrDst > SQR_DST_EVITE;
        }

        /// <summary>
        /// Renvoie la distance avec le mur le plus proche.
        /// </summary>
        /// <param name="wallMin"></param>
        /// <param name="wallMax"></param>
        /// <returns></returns>
        internal double DistanceToWall(double wallMinX, double wallMinY, double wallMaxX, double wallMaxY)
        {
            double min = double.MaxValue;
            min = Math.Min(min, PosX - wallMinX);
            min = Math.Min(min, PosY - wallMinY);
            min = Math.Min(min, wallMaxY - PosY);
            min = Math.Min(min, wallMaxX - PosX);
            return min;
        }

        protected void NormalizeSpeed()
        {
            double speedLength = Math.Sqrt(SpeedX * SpeedX + SpeedY * SpeedY);
            SpeedX /= speedLength;
            SpeedY /= speedLength;
        }


        internal bool AvoidWalls(double wallMinX, double wallMinY, double wallMaxX, double wallMaxY)
        {
            //On s'arrête aux murs
            PosX = Math.Clamp(PosX, wallMinX, wallMaxX);
            PosY = Math.Clamp(PosY, wallMinY, wallMaxY);

            //Changer de direction
            double dst = DistanceToWall(wallMinX, wallMinY, wallMaxX, wallMaxY);
            if(dst < DST_EVITE)
            {
                if (dst == PosX - wallMinX)
                {
                    SpeedX += SPD_CHANGE_DIR;
                }
                else if (dst == PosY - wallMinY)
                {
                    SpeedY += SPD_CHANGE_DIR;
                }
                else if (dst == wallMaxX - PosX)
                {
                    SpeedX -= SPD_CHANGE_DIR;
                }
                else if (dst == wallMaxY - PosY)
                {
                    SpeedY -= SPD_CHANGE_DIR;
                }

                NormalizeSpeed();
                return true;
            }

            return false;
        }

        internal bool AvoidObstacle(List<Obstacle> obstacles)
        {
            Obstacle nearestObstacle = null;
            for (int i = 0; i < obstacles.Count; i++)
            {
                Obstacle obs = obstacles[i];
                if (SqrDistanceTo(obs) < obs.Radius * obs.Radius)
                {
                    nearestObstacle = obs;
                    break;
                }
            }

            if (nearestObstacle is not null)
            {
                double dstToObs = DistanceTo(nearestObstacle);
                double dX = (nearestObstacle.PosX - PosX) / dstToObs;
                double dY = (nearestObstacle.PosY - PosY) / dstToObs;
                SpeedX -= dX / 2f;
                SpeedY -= dY / 2f;
                NormalizeSpeed();
                return true;
            }

            return false;
        }

        internal bool AvoidBoid(Boid boid)
        {
            double dst = DistanceTo(boid);

            if (dst < DST_EVITE)
            {
                double dX = (boid.PosX - PosX) / dst;
                double dY = (boid.PosY - PosY) / dst;
                SpeedX -= dX / 4f;
                SpeedY -= dY / 4f;

                NormalizeSpeed();
                return true;
            }

            return false;
        }


        List<Boid> _boidsUsed = new List<Boid>();
        internal void ComputeAverageDir(Boid[] boids)
        {
            _boidsUsed.Clear();
            for (int i = 0; i < boids.Length; i++)
            {
                if (Near(boids[i]))
                {
                    _boidsUsed.Add(boids[i]);
                }
            }

            int nbBoids = _boidsUsed.Count;
            if(nbBoids >= 1)
            {
                double speedXTotal = 0;
                double speedYTotal = 0;

                for (int i = 0; i < nbBoids; i++)
                {
                    speedXTotal += _boidsUsed[0].SpeedX;
                    speedYTotal += _boidsUsed[0].SpeedY;
                }

                SpeedX = (speedXTotal / nbBoids + SpeedX) / 2f;
                SpeedY = (speedYTotal / nbBoids + SpeedY) / 2f;
                NormalizeSpeed();
            }
        }

        /// <summary>
        /// On cherche d'abord si on doit éviter un mur, puis un obstacle puis un boid.
        /// S'il n'y a pas eu d'évitement à faire, le boid s'aligne avec les autres.
        /// 
        /// </summary>
        internal void Update(Boid[] boids, List<Obstacle> obstacles, double terrainWidth, double terrainHeight)
        {
            if(!AvoidWalls(0, 0, terrainWidth, terrainHeight))
            {
                if (!AvoidObstacle(obstacles))
                {
                    double sqrDstMin = boids.Where(b => !b.Equals(this)).Min(x => x.SqrDistanceTo(this));

                    if (!AvoidBoid(boids.FirstOrDefault(b => SqrDistanceTo(b) == sqrDstMin)))
                    {
                        ComputeAverageDir(boids);
                    }
                }
            }

            UpdatePos();
        }
    }
}