using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace GameCore
{
    public abstract class PhysicsObject
    {
        public Vector2 Position { get; set; }
        public float Mass { get; set; }
        public Vector2 NetForce { get; set; } = Vector2.Zero;
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public bool Active { get; set; } = true;

        public PhysicsObject(Vector2 pos, float mass)
        {
            Position = pos;
            Mass = mass;
        }

        public float GetMagnitude()
        {
            return NetForce.Length();
        }

        public void ApplyForce(Vector2 f)
        {
            NetForce += f;
        }

        public void Update()
        {
            if (Active == false) return;

            //Console.WriteLine($"f: {NetForce} | v:{Velocity.Length()}");
            float dt = GetFrameTime();

            // damping
            float damping = 0.7f;
            Velocity = Velocity.Length() > 0.01f ? Velocity * (1 - dt * damping) : Vector2.Zero;

            //ApplyForce(new Vector2(0, 9.81f * Mass));
            Vector2 acceleration = NetForce.Length() > 0 ? NetForce / Mass : Vector2.Zero;
            Velocity += acceleration * dt;
            Position += Velocity * dt * 500;

            NetForce = Vector2.Zero;

            // expensive
            foreach (PhysicsObject item in Game.objects)
            {
                if (item == this) continue;
                if (item.Active) Collide(item);
            }
        }

        protected abstract bool Collide(PhysicsObject item);


        public abstract void Draw();
    }

    public class Circle : PhysicsObject
    {
        public float Radius { get; set; }

        public Circle(Vector2 pos, float mass) : base(pos, mass)
        {
            Radius = 50f;
        }

        public Circle(Vector2 pos, float mass, float radius) : base(pos, mass)
        {
            this.Radius = radius;
        }

        public override void Draw()
        {
            DrawCircleV(Position, Radius, Color.Black);
        }

        protected override bool Collide(PhysicsObject obj)
        {

            if (obj is Circle c)
            {
                if (Vector2.Distance(obj.Position, Position) <= c.Radius + Radius)
                {
                    float theta = MathF.Atan2(obj.Position.Y - Position.Y, obj.Position.X - Position.X);

                    float u1x = (Velocity.X * MathF.Sin(theta) - Velocity.Y * MathF.Cos(theta)) * MathF.Sin(theta) + (((Mass - obj.Mass) * (Velocity.X * MathF.Cos(theta) + Velocity.Y * MathF.Sin(theta)) + 2 * obj.Mass * (obj.Velocity.X * MathF.Cos(theta) + obj.Velocity.Y * MathF.Sin(theta))) / (Mass + obj.Mass) * MathF.Cos(theta));

                    float u1y = (-Velocity.X * MathF.Sin(theta) + Velocity.Y * MathF.Cos(theta)) * MathF.Cos(theta) + (((Mass - obj.Mass) * (Velocity.X * MathF.Cos(theta) + Velocity.Y * MathF.Sin(theta)) + 2 * obj.Mass * (obj.Velocity.X * MathF.Cos(theta) + obj.Velocity.Y * MathF.Sin(theta))) / (Mass + obj.Mass) * MathF.Sin(theta));

                    float u2x = (obj.Velocity.X * MathF.Sin(theta) - obj.Velocity.Y * MathF.Cos(theta)) * MathF.Sin(theta) + (((obj.Mass - Mass) * (obj.Velocity.X * MathF.Cos(theta) + obj.Velocity.Y * MathF.Sin(theta)) + 2 * Mass * (Velocity.X * MathF.Cos(theta) + Velocity.Y * MathF.Sin(theta))) / (Mass + obj.Mass) * MathF.Cos(theta));

                    float u2y = (-obj.Velocity.X * MathF.Sin(theta) + obj.Velocity.Y * MathF.Cos(theta)) * MathF.Cos(theta) + (((obj.Mass - Mass) * (obj.Velocity.X * MathF.Cos(theta) + obj.Velocity.Y * MathF.Sin(theta)) + 2 * Mass * (Velocity.X * MathF.Cos(theta) + Velocity.Y * MathF.Sin(theta))) / (Mass + obj.Mass) * MathF.Sin(theta));

                    Velocity = new(u1x, u1y);
                    obj.Velocity = new(u2x, u2y);

                    // fix for magnetism effect
                    Vector2 normal = Vector2.Normalize(Position - obj.Position);
                    Position += normal;
                    obj.Position -= normal;

                    Console.WriteLine($"{obj.Position} {Position} {theta}");

                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
    public class PoolBall(Vector2 pos, float mass, int number, float radius, Color color) : Circle(pos, mass, radius)
    {
        public int Number { get; } = number;
        public Color Color { get; set; } = color;
        public float actualR { get; } = radius;
        private Vector2 offset = new(3,-3);
        private Color reflection = new(255, 255, 255, 100);
        public override void Draw()
        {
            DrawCircleV(Position, Radius, Color);
            if (Active)
            {
                DrawCircleGradient((int)(Position.X+offset.X), (int)(Position.Y+offset.Y), Radius/2, reflection, Color);
                
                DrawTextEx(GetFontDefault(), Number.ToString(), Position - new Vector2(5, 5), 16, 1, Color.White);
            }
        }
    }

}