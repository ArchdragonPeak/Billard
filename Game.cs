using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
namespace GameCore
{
    public class Game
    {
        private readonly int screenWidth = 1500;
        private readonly int screenHeight = 1000;
        private readonly string title = "Billard";

        private bool running = true;
        private readonly Circle c = new(new(100, 100), 1f);

        private bool physicsRunning = false;

        public static readonly LinkedList<PhysicsObject> objects = new();

        public Game()
        {
            InitWindow(screenWidth, screenHeight, title);
            SetTargetFPS(240);
            objects.AddLast(c);
            int baseX = 300;
            int baseY = 402;
            int n = 1;
            // ball triangle
            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 4 - i; j++)
                {
                    objects.AddLast(new PoolBall(new Vector2(baseX + i * 25, (baseY + j * 25) + i * 25 / 2.2f), 20, n++, 25 / 2.2f));

                }
            }
            // white ball
            objects.AddLast(new PoolBall(new Vector2(baseX + 700, baseY + 50), 20, 0, 25 / 2.2f));
        }

        // game loop
        public void Run()
        {
            while (!WindowShouldClose() && running)
            {
                HandleKeyboard();
                HandleMouse();
                Draw();
                if (physicsRunning) Update();
            }

            Shutdown();
        }

        private void Draw()
        {
            BeginDrawing();
            ClearBackground(Color.SkyBlue);

            // field
            DrawRectangle(200, 200, 1000, 500, Color.DarkGreen);
            // boundaries up, down, left, right
            Vector2 d = new(200, 200);

            DrawLineEx(d, d + new Vector2(1000, 0), 10, Color.DarkBrown);
            DrawLineEx(d + new Vector2(0, 500), d + new Vector2(1000, 500), 10, Color.DarkBrown);

            DrawLineEx(d, d + new Vector2(0, 500), 10, Color.DarkBrown);
            DrawLineEx(d + new Vector2(1000, 0), d + new Vector2(1000, 500), 10, Color.DarkBrown);

            // pockets up: lmr -> down: lmr
            Color VeryDarkGray = new(50, 50, 50);
            DrawCircleGradient(200, 200, 25, Color.Black, VeryDarkGray);
            DrawCircleGradient(200 + 500, 200, 27, Color.Black, VeryDarkGray);
            DrawCircleGradient(200 + 1000, 200, 25, Color.Black, VeryDarkGray);

            DrawCircleGradient(200, 200 + 500, 25, Color.Black, VeryDarkGray);
            DrawCircleGradient(200 + 500, 200 + 500, 27, Color.Black, VeryDarkGray);
            DrawCircleGradient(200 + 1000, 200 + 500, 25, Color.Black, VeryDarkGray);

            // game objects
            foreach (PhysicsObject item in objects)
            {
                item.Draw();
            }


            DrawText($"FPS: {1 / GetFrameTime()}", 20, 20, 32, Color.White);
            DrawText($"Physics Running: {physicsRunning}", 20, 52, 32, physicsRunning ? Color.Green : Color.Orange);

            EndDrawing();
        }
        private void Update()
        {
            foreach (PhysicsObject item in objects)
            {
                item.Update();

                if (item is Circle circle)
                {
                    // boundaries
                    if (circle.Position.Y > 700 - circle.radius)
                    {
                        circle.Position = new(circle.Position.X, circle.Position.Y - 1);
                        circle.Velocity = new(circle.Velocity.X, circle.Velocity.Y * -0.9f);
                    }
                    if (circle.Position.Y <= 200 + circle.radius)
                    {
                        circle.Position = new(circle.Position.X, circle.Position.Y + 1);
                        circle.Velocity = new(circle.Velocity.X, circle.Velocity.Y * -0.9f);
                    }
                    if (circle.Position.X > 1200 - circle.radius)
                    {
                        circle.Position = new(circle.Position.X - 1, circle.Position.Y);
                        circle.Velocity = new(circle.Velocity.X * -0.9f, circle.Velocity.Y);
                    }
                    if (circle.Position.X <= 200 + circle.radius)
                    {
                        circle.Position = new(circle.Position.X + 1, circle.Position.Y);
                        circle.Velocity = new(circle.Velocity.X * -0.9f, circle.Velocity.Y);
                    }
                }

            }
        }

        // keyboard
        private void HandleKeyboard()
        {
            if (IsKeyPressed(KeyboardKey.Escape))
            {
                running = false;
            }

            if (IsKeyPressed(KeyboardKey.Space))
            {
                physicsRunning = !physicsRunning;
            }

            if (IsKeyPressed(KeyboardKey.F))
            {
                c.Position = new(100, 100);
                c.NetForce = Vector2.Zero;
                c.Velocity = Vector2.Zero;
            }

            if (IsKeyDown(KeyboardKey.Right))
            {
                // stuff
            }

            if (IsKeyDown(KeyboardKey.Left))
            {
                // stuff
            }
        }

        private void HandleMouse()
        {
            Vector2 pos = GetMousePosition();
            if (IsMouseButtonDown(MouseButton.Left))
            {
                foreach (PhysicsObject item in objects)
                {
                    if (item is Circle c)
                    {
                        if (GetDistance(pos, c.Position) < c.radius)
                        {
                            c.Position = pos;
                            c.NetForce = Vector2.Zero;
                            c.Velocity = GetMouseDelta() / GetFrameTime() / 1000;
                        }
                    }
                }

            }
        }
        public static float GetDistance(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
            //return (float)Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));
        }

        private void Shutdown()
        {
            CloseWindow();
        }
    }
}