using SFML.Graphics;
using SFML.Window;
using System;
using SFML.System;

namespace RayCast_SFML
{
    internal class Program
    {
        //Screen
        private const int SCREEN_WIDTH = 640;
        private const int SCREEN_HEIGHT = 480;
        private const int SCREEN_HALF_HEIGHT = SCREEN_HEIGHT / 2;
        private const int SCREEN_HALF_WIDTH = SCREEN_WIDTH / 2;
        private const int SCREEN_SCALE = 4;

        //Player
        private const double PLAYER_FOV = 60;
        private const double PLAYER_HALF_FOV = PLAYER_FOV / 2;
        private static double playerX = 3;
        private static double playerY = 3;
        private static double playerAngle = 45;
        private static double playerMoveSpeed = 4.0;
        private static double playerRotationSpeed = 40.0;

        //Ray
        private static double raycastingIncrementAngle;
        private static int raycastingPrecision = 64;

        //Projection
        private static double projectionWidth = SCREEN_WIDTH / SCREEN_SCALE;
        private static double projectionHeight = SCREEN_HEIGHT / SCREEN_SCALE;
        private static double projectionHalfWidth = projectionWidth / 2;
        private static double projectionHalfHeight = projectionHeight / 2;

        //Map
        private const int MAP_WIDTH = 24;
        private const int MAP_HEIGHT = 24;
        private const int MAP_BLOCK_SIZE = 8;

        static int[,] map = new int[MAP_WIDTH, MAP_HEIGHT]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
            {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
            {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };

        static void Main(string[] args)
        {
            raycastingIncrementAngle = PLAYER_FOV / SCREEN_WIDTH;

            //Game screen
            RenderWindow screen = new RenderWindow(new VideoMode(SCREEN_WIDTH, SCREEN_HEIGHT), "Raycast");

            Clock clock = new Clock();
            Time previousTime = clock.ElapsedTime;
            Time currentTime;
            float fps;

            //Game loop
            while (screen.IsOpen)
            {
                Time elapsedTime = clock.Restart();

                //Cleat screen
                screen.Clear(Color.White);

                //Draw
                DrawBackground(screen);
                RayCasting(screen);
                DrawMap(screen);
                DrawPlayer(screen);

                //Player Input
                PlayerInput(elapsedTime);

                currentTime = clock.ElapsedTime;
                fps = 1 / (currentTime.AsSeconds() - previousTime.AsSeconds());
                previousTime = currentTime;

                //Display screen
                screen.Display();
            }
        }

        private static double DegToRad(double degree)
        {
            return degree * Math.PI / 180;
        }

        private static void RayCasting(RenderWindow context)
        {
            double rayAngle = playerAngle - PLAYER_HALF_FOV;

            for (int rayCount = 0; rayCount < SCREEN_WIDTH; rayCount++ )
            {
                double rayX = playerX;
                double rayY = playerY;

                double rayCos = Math.Cos(DegToRad(rayAngle)) / raycastingPrecision;
                double raySin = Math.Sin(DegToRad(rayAngle)) / raycastingPrecision;

                int wall = 0;
                while (wall == 0)
                {
                    rayX += rayCos;
                    rayY += raySin;
                    wall = map[(int)rayY, (int)rayX];
                }

                double distance = Math.Sqrt(Math.Pow(playerX - rayX, 2) + Math.Pow(playerY - rayY, 2));

                //Fish eye fix
                distance *= Math.Cos(DegToRad(rayAngle - playerAngle));

                int wallHeight = (int)(SCREEN_HALF_HEIGHT / distance);

                Color wallColor;
                Color blackColor = Color.Black;

                switch (wall)
                {
                    case 1: wallColor = Color.Magenta; break;
                    case 2: wallColor = Color.Blue; break;
                    case 3: wallColor = Color.Green; break;
                    case 4: wallColor = Color.Yellow; break;
                    default: wallColor = Color.White; break;
                }

                DrawLine(context, rayCount, SCREEN_HALF_HEIGHT - wallHeight, wallHeight, wallColor);

                rayAngle += raycastingIncrementAngle;
            }
        }

        private static void DrawLine(RenderWindow context, int x1, int y1, int wallHeight, Color color)
        {
            RectangleShape line = new RectangleShape
            {
                Position = new Vector2f(x1, y1),
                Size = new Vector2f(1, wallHeight * 2),
                FillColor = color
            };

            context.Draw(line);
        }

        private static void DrawMap(RenderWindow context)
        {
            for (int y = 0; y < MAP_HEIGHT; y++)
            {
                for (int x = 0; x < MAP_WIDTH; x++)
                {
                    Color blockColor;

                    switch (map[x, y])
                    {
                        case 1: blockColor = Color.Magenta; break;
                        case 2: blockColor = Color.Blue; break;
                        case 3: blockColor = Color.Green; break;
                        case 4: blockColor = Color.Yellow; break;
                        default: blockColor = Color.White; break;
                    }

                    RectangleShape block = new RectangleShape
                    {
                        Position = new Vector2f(y * MAP_BLOCK_SIZE, x * MAP_BLOCK_SIZE),
                        Size = new Vector2f(MAP_BLOCK_SIZE, MAP_BLOCK_SIZE),
                        OutlineColor = Color.Black,
                        FillColor = blockColor,
                        OutlineThickness = 1
                    };

                    context.Draw(block);
                }
            }
        }

        private static void DrawPlayer(RenderWindow context)
        {
            CircleShape player = new CircleShape
            {
                Radius = MAP_BLOCK_SIZE / 2,
                FillColor = Color.Red,
                OutlineColor = Color.Black,
                Position = new Vector2f((float)playerX * MAP_BLOCK_SIZE, (float)playerY * MAP_BLOCK_SIZE)
            };

            context.Draw(player);
        }

        private static void DrawBackground(RenderWindow context)
        {
            RectangleShape ceiling = new RectangleShape
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(SCREEN_WIDTH, SCREEN_HALF_HEIGHT),
                FillColor = new Color(117, 187, 253)
            };

            RectangleShape floor = new RectangleShape
            {
                Position = new Vector2f(0, SCREEN_HALF_HEIGHT),
                Size = new Vector2f(SCREEN_WIDTH, SCREEN_HALF_HEIGHT),
                FillColor = new Color(63, 155, 11)
            };

            context.Draw(ceiling);
            context.Draw(floor);
        }

        private static void PlayerInput(Time elapsedTime)
        {
            //move forward if no wall in front of you
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                double playerCos = Math.Cos(DegToRad(playerAngle)) * playerMoveSpeed;
                double playerSin = Math.Sin(DegToRad(playerAngle)) * playerMoveSpeed;
                double newX = playerX + playerCos * elapsedTime.AsSeconds();
                double newY = playerY + playerSin * elapsedTime.AsSeconds();

                //Collision test
                if (map[(int)newY, (int)newX] == 0)
                {
                    playerX = newX;
                    playerY = newY;
                }
            }
            //move backwards if no wall behind you
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                double playerCos = Math.Cos(DegToRad(playerAngle)) * playerMoveSpeed;
                double playerSin = Math.Sin(DegToRad(playerAngle)) * playerMoveSpeed;
                double newX = playerX - playerCos * elapsedTime.AsSeconds();
                double newY = playerY - playerSin * elapsedTime.AsSeconds();

                //Collision test
                if (map[(int)newY, (int)newX] == 0)
                {
                    playerX = newX;
                    playerY = newY;
                }
            }
            //rotate to the right
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                playerAngle += playerRotationSpeed * elapsedTime.AsSeconds();
            }
            //rotate to the left
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                playerAngle -= playerRotationSpeed * elapsedTime.AsSeconds();
            }
        }
    }
}
