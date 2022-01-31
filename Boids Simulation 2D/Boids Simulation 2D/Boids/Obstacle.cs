
namespace Project.Boids
{
    public class Obstacle : ObjectInWorld
    {
        public double Radius { get; private set; }
        protected int TimeToLive = 100;

        public Obstacle(double posX, double posY, double radius) : base(posX, posY)
        {
            Radius = radius;
        }

        public void Update()
        {
            TimeToLive--;
        }

        public bool IsDead() => TimeToLive <= 0;
    }
}