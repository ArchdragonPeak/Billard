using System.Diagnostics;
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
        //private readonly Circle c = new(new(100, 100), 1f);

        private bool physicsRunning = true;

        Random rand = new();

        public static LinkedList<PhysicsObject> objects = new();
        private Circle white;
        Vector2[] pocketArr;

        int round = 1;
        bool hasChanged = false;

        int strength = 5;

        public bool ballsMoving = false;

        readonly Color[] balls =
        [
            new Color(245, 245, 245, 255), // 0
            new Color(230, 200,  40, 255), // 1
            new Color( 40,  90, 180, 255), // 2
            new Color(200,  50,  50, 255), // 3
            new Color(140,  60, 160, 255), // 4
            new Color(235, 120,  40, 255), // 5
            new Color( 30, 110,  60, 255), // 6
            new Color(110,  40,  40, 255), // 7
            new Color( 25,  25,  25, 255), // 8
            new Color(230, 200,  40, 255), // 9
            new Color( 40,  90, 180, 255), // 10
            new Color(200,  50,  50, 255), // 11
            new Color(140,  60, 160, 255), // 12
            new Color(235, 120,  40, 255), // 13
            new Color( 30, 110,  60, 255), // 14
            new Color(110,  40,  40, 255)  // 15
        ];

        public Game()
        {
            InitWindow(screenWidth, screenHeight, title);
            SetTargetFPS(480);
            //ResetGame();
            ResetGame();

            pocketArr = [
                    new(200, 200), new(200 + 500, 200), new(200 + 1000, 200),
                    new(200, 200 + 500), new(200 + 500, 200 + 500), new(200 + 1000, 200 + 500)
                    ];

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

            // table
            DrawRectangleRounded(new Rectangle(170, 170, 1060, 560), 0.1f, 1, Color.DarkBrown);
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
            foreach (Vector2 pocket in pocketArr)
            {
                DrawCircleGradient((int)pocket.X, (int)pocket.Y, 25, Color.Black, VeryDarkGray);
            }

            if (white.Velocity.Length() == 0 && white.Active)
            {
                DrawLineV(white.Position, GetMousePosition(), white.Velocity.Length() == 0 ? Color.Blue : Color.Red);
                Vector2 opp = white.Position - (Vector2.Normalize((GetMousePosition() - white.Position))*500);
                Vector2 n = (GetMousePosition() - white.Position);
                n = new(-n.Y, n.X);
                n *= (strength/4f);

                // debug: normal
                //DrawLineV(opp + n/10, opp -n/10, Color.Black);
                Color transparentGreen = Color.Green;
                transparentGreen.A = 50;
                DrawTriangle(opp - (n / 7), opp + n / 7, white.Position, transparentGreen);
                //DrawLineV(white.Position, opp, white.Velocity.Length() == 0 ? Color.Blue : Color.Red);
            }


            // game objects
            foreach (PhysicsObject item in objects)
            {
                item.Draw();
            }


            DrawText($"FPS: {1 / GetFrameTime()}", 20, 20, 32, Color.White);
            DrawText($"Physics Running: {physicsRunning}", 20, 52, 32, physicsRunning ? Color.Green : Color.Orange);
            DrawText($"moving? {ballsMoving}", 20, 84, 32, Color.White);
            if (round > 0)
            {
                DrawText($"Round: {round}", 20, 84 + 32, 32, Color.White);

                DrawText($"{(round % 2 == 1 ? "Player1" : "Player2")}", 500, 20, 32, Color.White);
            }
            else
            {
                DrawText("Player1", 500, 20, 32, Color.White);
            }
            EndDrawing();
        }
        public void Update()
        {
            ballsMoving = false;
            foreach (PhysicsObject item in objects)
            {
                item.Update();
                if (item is PoolBall ball)
                {
                    foreach (Vector2 pocket in pocketArr)
                    {
                        if (Vector2.Distance(ball.Position, pocket) < 25)
                        {
                            ball.Active = false;
                            ball.Velocity = Vector2.Zero;
                            if (ball.Radius > ball.actualR * 0.7f) ball.Radius *= 0.99f;
                        }
                    }

                    // moving false, except one active ball is moving
                    if (ball.Velocity.Length() > 0) ballsMoving = true;

                }



                if (item is Circle circle)
                {
                    // boundaries
                    if (circle.Position.Y > 700 - circle.Radius)
                    {
                        circle.Position = new(circle.Position.X, circle.Position.Y - 1);
                        circle.Velocity = new(circle.Velocity.X, circle.Velocity.Y * -0.9f);
                    }
                    if (circle.Position.Y <= 200 + circle.Radius)
                    {
                        circle.Position = new(circle.Position.X, circle.Position.Y + 1);
                        circle.Velocity = new(circle.Velocity.X, circle.Velocity.Y * -0.9f);
                    }
                    if (circle.Position.X > 1200 - circle.Radius)
                    {
                        circle.Position = new(circle.Position.X - 1, circle.Position.Y);
                        circle.Velocity = new(circle.Velocity.X * -0.9f, circle.Velocity.Y);
                    }
                    if (circle.Position.X <= 200 + circle.Radius)
                    {
                        circle.Position = new(circle.Position.X + 1, circle.Position.Y);
                        circle.Velocity = new(circle.Velocity.X * -0.9f, circle.Velocity.Y);
                    }
                }

            }
            if(!ballsMoving && hasChanged)
            {
                round++;
            }

            hasChanged = ballsMoving;
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

            if (IsKeyDown(KeyboardKey.F))
            {
                /*c.Position = new(100, 100);
                c.NetForce = Vector2.Zero;
                c.Velocity = Vector2.Zero;*/
                ResetGame();
                //RandomReset();
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
            strength += (int)GetMouseWheelMove();
            if(strength > 10) strength = 10;
            if(strength < 1) strength = 1;

            if(GetMouseWheelMove()!=0)
            {
                Debug.WriteLine(strength);
            }

            Vector2 pos = GetMousePosition();
            if (IsMouseButtonReleased(MouseButton.Left) && physicsRunning)
            {
                if (!ballsMoving)
                {
                    //float distance = Vector2.Distance(white.Position, pos);
                    Vector2 newVelocity = (Vector2.Subtract(white.Position, pos)) / 150;
                    Debug.WriteLine(newVelocity.Length());
                    white.Velocity = Vector2.Normalize(newVelocity)*strength;
                    ballsMoving = true;
                }
                /*
                foreach (PhysicsObject item in objects)
                {
                    if (item is Circle c)
                    {
                        if (Vector2.Distance(pos, c.Position) < c.radius)
                        {
                            c.Position = pos;
                            c.NetForce = Vector2.Zero;
                            c.Velocity = GetMouseDelta() / GetFrameTime() / 1000;
                        }
                    }
                }*/

            }
        }

        public void Shoot(float theta, int strength)
        {
            white.Velocity = new(strength * MathF.Cos(theta), strength * MathF.Sin(theta));
            round++;
            ballsMoving = true;
        }
        public List<(float, float)> GetTable()
        {
            List<(float, float)> table = [];
            foreach (PhysicsObject item in objects)
            {
                if (item is PoolBall p)
                {
                    if (item.Active)
                    {
                        table.Add((p.Position.X, p.Position.Y));
                    }
                    else
                    {
                        table.Add((0, 0));
                    }

                }
            }
            return table;
        }
        public void ResetGame()
        {
            objects = new();
            round = 1;
            ballsMoving = false;
            int baseX = 300;
            int baseY = 402;
            int n = 1;

            // ball triangle
            int mass = 50;
            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 4 - i; j++)
                {
                    objects.AddLast(new PoolBall(new Vector2(baseX + i * 22, (baseY + j * 24) + i * 24 / 2.2f), mass, n++, 25 / 2.1f, balls[n - 1]));

                }
            }
            // white ball
            white = new PoolBall(new Vector2(baseX + 700, baseY + 50), 20, 0, 25 / 2.2f, Color.White);
            objects.AddLast(white);
        }

        public void RandomReset()
        {
            objects = new();
            round = 0;
            ballsMoving = false;
            int baseX = 230;
            int baseY = 230;
            int n = 1;
            bool notFound = true;
            Vector2 possiblePos = new();
            // ball triangle
            int mass = 50;


            // white ball
            white = new PoolBall(new Vector2(baseX + rand.Next(950), baseY + rand.Next(450)), 20, 0, 25 / 2.2f, Color.White);
            objects.AddLast(white);
            for (int i = 0; i <= 4; i++)
            {
                for (int j = 0; j <= 4 - i; j++)
                {
                    while (true)
                    {
                        possiblePos = new(baseX + rand.Next(950), baseY + rand.Next(450));
                        bool collision = false;

                        foreach (var item in objects)
                        {
                            if (Vector2.Distance(possiblePos, item.Position) <= 23)
                            {
                                collision = true;
                                break;
                            }
                        }

                        if (!collision) break;
                    }

                    objects.AddLast(new PoolBall(possiblePos, mass, n++, 25 / 2.1f, balls[n - 1]));
                }
            }
        }

        private void Shutdown()
        {
            CloseWindow();
        }
    }
}