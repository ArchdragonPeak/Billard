using System.Globalization;
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

            //Console.WriteLine($"f: {NetForce} | v:{Velocity.Length()}");
            float dt = GetFrameTime();
            Velocity = Velocity.Length() > 0.01f ? Velocity * (1 - dt * 0.1f) : Vector2.Zero;
            //ApplyForce(new Vector2(0, 9.81f * Mass));
            Vector2 acceleration = NetForce.Length() > 0 ? NetForce / Mass : Vector2.Zero;
            Velocity += acceleration * dt;
            Position += Velocity * dt * 500;

            NetForce = Vector2.Zero;

            // expensive
            foreach (PhysicsObject item in Game.objects)
            {
                if (item == this) continue;
                Collide(item);
            }
        }

        protected abstract bool Collide(PhysicsObject item);


        public abstract void Draw();
    }

    public class Circle : PhysicsObject
    {
        public float radius { get; private set; }

        public Circle(Vector2 pos, float mass) : base(pos, mass)
        {
            radius = 50f;
        }

        public Circle(Vector2 pos, float mass, float radius) : base(pos, mass)
        {
            this.radius = radius;
        }

        public override void Draw()
        {
            DrawCircleV(Position, radius, Color.Black);
        }

        protected override bool Collide(PhysicsObject obj)
        {

            if (obj is Circle c)
            {
                if (Vector2.Distance(obj.Position, Position) <= c.radius + radius)
                {
                    Console.WriteLine($"{obj.Position} {Position}");

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
    public class PoolBall(Vector2 pos, float mass, int number, float radius) : Circle(pos, mass, radius)
    {
        public int Number { get; } = number;

        public override void Draw()
        {
            DrawCircleV(Position, radius, Color.Black);
            DrawTextEx(GetFontDefault(), number.ToString(), Position - new Vector2(5, 5), 16, 1, Color.White);
        }
    }

}