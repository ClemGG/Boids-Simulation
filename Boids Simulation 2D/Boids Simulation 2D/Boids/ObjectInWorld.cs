using System;

namespace Project.Boids
{
    public class ObjectInWorld
    {
        public double PosX, PosY;

        public ObjectInWorld(double posX, double posY)
        {
            PosX = posX;
            PosY = posY;
        }

        public double DistanceTo(ObjectInWorld other)
        {
            return Math.Sqrt(SqrDistanceTo(other));
        }

        public double SqrDistanceTo(ObjectInWorld other)
        {
            return (other.PosX - PosX) * (other.PosX - PosX) + (other.PosY - PosY) * (other.PosY - PosY);
        }
    }
}