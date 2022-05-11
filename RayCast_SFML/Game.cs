using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace RayCast_SFML
{
    internal class Game
    {
        //Game
        public string Title;

        //Папка с ассетами для игры
        public static string AssetsFolder = @"C:\Users\artem\source\repos\RayCast_SFML\RayCast_SFML\assets\";

        //Screen
        public int ScreenWidth; //Ширина экрана
        public int ScreenHeight; //Высота экрана
        private int _screenHalfHeight; //Половина высоты экрана
        private int _screenHalfWidth; //Половина ширины экрана

        //Player
        private Player _player;

        //Ray
        private static double raycastingIncrementAngle;
        private static int raycastingPrecision = 64;

        //Словарь текстур
        private Dictionary<string, Texture> _textures;

        //Словарь спрайтов
        private Dictionary<string, Sprite> _sprites;

        //Map
        private Map _map;

        //Window
        private RenderWindow _window;

        //Animations
        private AnimationManager _animationManager;

        //Clock
        private Clock _clock;

        public Game(string title, int screenWidth, int screenHeight)
        {
            Title = title;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;

            //Инициализация объектов игры
            Initialize();
        }

        public void Run()
        {
            //Game loop
            while (_window.IsOpen)
            {
                _window.DispatchEvents();

                //Game time
                Time elapsedTime = _clock.Restart();

                //Clear screen
                _window.Clear(Color.White);

                //Draw
                Draw();

                //Update
                Update(elapsedTime);

                //Display screen
                _window.Display();
            }
        }

        private double DegToRad(double degree)
        {
            return degree * Math.PI / 180;
        }

        private void DrawRayCasting()
        {
            double rayAngle = _player.Angle - _player.HalfFov;

            for (int rayCount = 0; rayCount < ScreenWidth; rayCount++)
            {
                double rayX = _player.X;
                double rayY = _player.Y;

                double rayCos = Math.Cos(DegToRad(rayAngle)) / raycastingPrecision;
                double raySin = Math.Sin(DegToRad(rayAngle)) / raycastingPrecision;

                int wall = 0;
                while (wall == 0)
                {
                    rayX += rayCos;
                    rayY += raySin;
                    wall = _map.Bitmap[(int)rayY, (int)rayX];
                }

                //Расчет дистанции до стены
                double distance = Math.Sqrt(Math.Pow(_player.X - rayX, 2) + Math.Pow(_player.Y - rayY, 2));

                //Корректировка фишай эффекта
                distance *= Math.Cos(DegToRad(rayAngle - _player.Angle));

                int wallHeight = (int)(_screenHalfHeight / distance);

                //Текстурирование
                Texture currTexture;

                switch (wall)
                {
                    case 1: currTexture = _textures["brickWall"]; break;
                    case 2: currTexture = _textures["whiteBrickWall"]; break;
                    case 3: currTexture = _textures["sand"]; break;
                    case 4: currTexture = _textures["brickWall"]; break;
                    default: currTexture = _textures["brickWall"]; break;
                }

                //Расчет позиции текстуры (сдвиг)
                double texturePosX = Math.Floor((currTexture.Width * (rayX + rayY)) % currTexture.Width);

                /*//Просто цветные стены
                Color wallColor;
                Color blackColor = Color.Black;

                switch (wall)
                {
                    case 1: wallColor = Color.Magenta; break;
                    case 2: wallColor = Color.Blue; break;
                    case 3: wallColor = Color.Green; break;
                    case 4: wallColor = Color.Yellow; break;
                    default: wallColor = Color.White; break;
                }*/

                //DrawLine(rayCount, _screenHalfHeight - wallHeight, wallHeight, wallColor);
                //DrawLineByPoints(rayCount, _screenHalfHeight - wallHeight, rayCount, _screenHalfHeight + wallHeight, wallColor);
                DrawTexture(currTexture, rayCount, wallHeight, texturePosX);

                rayAngle += raycastingIncrementAngle;
            }
        }

        private void DrawLine(int x1, int y1, int wallHeight, Color color)
        {
            RectangleShape line = new RectangleShape
            {
                Position = new Vector2f(x1, y1),
                Size = new Vector2f(1, wallHeight * 2),
                FillColor = color
            };

            _window.Draw(line);
        }

        private void DrawMap()
        {
            for (int y = 0; y < _map.Height; y++)
            {
                for (int x = 0; x < _map.Width; x++)
                {
                    Color blockColor;

                    switch (_map.Bitmap[x, y])
                    {
                        case 1: blockColor = Color.Magenta; break;
                        case 2: blockColor = Color.Blue; break;
                        case 3: blockColor = Color.Green; break;
                        case 4: blockColor = Color.Yellow; break;
                        default: blockColor = Color.White; break;
                    }

                    RectangleShape block = new RectangleShape
                    {
                        Position = new Vector2f(y * _map.BlockSize, x * _map.BlockSize),
                        Size = new Vector2f(_map.BlockSize, _map.BlockSize),
                        OutlineColor = Color.Black,
                        FillColor = blockColor,
                        OutlineThickness = 1
                    };

                    _window.Draw(block);
                }
            }
        }

        private void DrawPlayer()
        {
            CircleShape player = new CircleShape
            {
                Radius = (float)_player.Radius,
                Origin = new Vector2f((float)_player.Radius, (float)_player.Radius),
                FillColor = Color.Red,
                OutlineColor = Color.Black,
                Position = new Vector2f((float)_player.X * _map.BlockSize, (float)_player.Y * _map.BlockSize)
            };

            _window.Draw(player);
        }

        private void DrawBackground()
        {
            RectangleShape ceiling = new RectangleShape
            {
                Position = new Vector2f(0, 0),
                Size = new Vector2f(ScreenWidth, _screenHalfHeight),
                FillColor = new Color(117, 187, 253)
            };

            RectangleShape floor = new RectangleShape
            {
                Position = new Vector2f(0, _screenHalfHeight),
                Size = new Vector2f(ScreenWidth, _screenHalfHeight),
                FillColor = new Color(63, 155, 11)
            };

            _window.Draw(ceiling);
            _window.Draw(floor);
        }

        private void PlayerInput(Time elapsedTime)
        {
            double windowCenterX = 0.5f * _window.Size.X;
            double windowCenterY = 0.5f * _window.Size.Y;

            double rotation_horizontal = _player.Fov * (windowCenterX - Mouse.GetPosition(_window).X) / _window.Size.X;

            _player.Angle -= rotation_horizontal;

            Mouse.SetPosition(new Vector2i(_screenHalfWidth, _screenHalfHeight), _window);

            //move forward if no wall in front of you
            if (Keyboard.IsKeyPressed(Keyboard.Key.W))
            {
                double playerCos = Math.Cos(DegToRad(_player.Angle)) * _player.MoveSpeed;
                double playerSin = Math.Sin(DegToRad(_player.Angle)) * _player.MoveSpeed;
                double newX = _player.X + playerCos * elapsedTime.AsSeconds();
                double newY = _player.Y + playerSin * elapsedTime.AsSeconds();

                //Collision test
                if (_map.Bitmap[(int)newY, (int)newX] == 0)
                {
                    _player.X = newX;
                    _player.Y = newY;
                }
            }
            //move backwards if no wall behind you
            if (Keyboard.IsKeyPressed(Keyboard.Key.S))
            {
                double playerCos = Math.Cos(DegToRad(_player.Angle)) * _player.MoveSpeed;
                double playerSin = Math.Sin(DegToRad(_player.Angle)) * _player.MoveSpeed;
                double newX = _player.X - playerCos * elapsedTime.AsSeconds();
                double newY = _player.Y - playerSin * elapsedTime.AsSeconds();

                //Collision test
                if (_map.Bitmap[(int)newY, (int)newX] == 0)
                {
                    _player.X = newX;
                    _player.Y = newY;
                }
            }
            //rotate to the right
            if (Keyboard.IsKeyPressed(Keyboard.Key.D))
            {
                double playerCos = Math.Cos(DegToRad(_player.Angle - 90)) * _player.MoveSpeed;
                double playerSin = Math.Sin(DegToRad(_player.Angle - 90)) * _player.MoveSpeed;
                double newX = _player.X - playerCos * elapsedTime.AsSeconds();
                double newY = _player.Y - playerSin * elapsedTime.AsSeconds();

                //Collision test
                if (_map.Bitmap[(int)newY, (int)newX] == 0)
                {
                    _player.X = newX;
                    _player.Y = newY;
                }
            }
            //rotate to the left
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
            {
                double playerCos = Math.Cos(DegToRad(_player.Angle + 90)) * _player.MoveSpeed;
                double playerSin = Math.Sin(DegToRad(_player.Angle + 90)) * _player.MoveSpeed;
                double newX = _player.X - playerCos * elapsedTime.AsSeconds();
                double newY = _player.Y - playerSin * elapsedTime.AsSeconds();

                //Collision test
                if (_map.Bitmap[(int)newY, (int)newX] == 0)
                {
                    _player.X = newX;
                    _player.Y = newY;
                }
            }
        }

        private void DrawLineByPoints(double x1, double y1, double x2, double y2, Color color)
        {
            Vertex[] line =
            {
                new Vertex(new Vector2f((float)x1, (float)y1)){Color = color},
                new Vertex(new Vector2f((float)x2, (float)y2)){Color = color}
            };

            _window.Draw(line, PrimitiveType.Lines);
        }

        private void DrawTexture(Texture texture, double x, double wallHeight, double texturePosX)
        {
            Image wallImg = texture.Image;
            double yIncrementer = (wallHeight * 2) / texture.Height;
            double y = _screenHalfHeight - wallHeight;

            for (int i = 0; i < texture.Height; i++)
            {
                Color strokeColor = wallImg.GetPixel((uint)i, (uint)texturePosX);
                //double depth = _screenHalfHeight / wallHeight;
                //int c = (int)(255 / (1 + depth * depth * 0.0001));
                Vertex[] line =
                {
                    new Vertex(new Vector2f((float)x, (float)y)){Color = strokeColor},
                    new Vertex(new Vector2f((float)x, (float)(y + yIncrementer))){Color = strokeColor}
                };

                _window.Draw(line, PrimitiveType.Lines);

                y += yIncrementer;
            }
        }

        private void DrawSprites()
        {
            //Shotgun
            _window.Draw(_sprites["shotgun"]);
        }

        private int _mFrame;
        private int _mFps;
        private Clock _mClock = new();
        private void DrawFps()
        {
            Text fps = new Text()
            {
                Font = new Font(AssetsFolder + "fonts\\" + "ARIAL.TTF"),
                CharacterSize = 24,
                FillColor = Color.Green,
                Position = new Vector2f(ScreenWidth - 30, 0)
            };

            if (_mClock.ElapsedTime.AsSeconds() >= 1)
            {
                _mFps = _mFrame;
                _mFrame = 0;
                _mClock.Restart();
            }

            ++_mFrame;

            fps.DisplayedString = _mFps.ToString();

            _window.Draw(fps);
        }

        /// <summary>
        /// Инициализирует объекты игры
        /// </summary>
        private void Initialize()
        {
            _window = new RenderWindow(new VideoMode((uint)ScreenWidth, (uint)ScreenHeight), Title);
            _window.Closed += (object sender, EventArgs e) => { _window.Close();};

            _window.SetMouseCursorVisible(false);

            _player = new Player();

            _screenHalfWidth = ScreenWidth / 2;
            _screenHalfHeight = ScreenHeight / 2;

            raycastingIncrementAngle = _player.Fov / ScreenWidth;

            _textures = new Dictionary<string, Texture>();
            _textures["brickWall"] = new Texture("wall.png");
            _textures["whiteBrickWall"] = new Texture("whiteBrickWall.png");
            _textures["sand"] = new Texture("sand.png");

            _map = new Map(
                    new int[24, 24]
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
                    },
                    8
                );

            _sprites = new Dictionary<string, Sprite>();
            _sprites.Add("shotgun", new Sprite(new SFML.Graphics.Texture(AssetsFolder + "sprites\\" + "s_weapon_shotgun.png")));
            _sprites["shotgun"].Position = _player.ArmCoordinate;
            _sprites["shotgun"].Scale = new Vector2f(1.7f, 1.7f);
            _sprites["shotgun"].TextureRect = new IntRect(0, 0, 102, 147);

            _clock = new Clock();
        }

        /// <summary>
        /// Обновление объектов на экране
        /// </summary>
        private void Update(Time elapsedTime)
        {
            PlayerInput(elapsedTime);
        }

        /// <summary>
        /// Отрисовка объектов на экране
        /// </summary>
        private void Draw()
        {
            DrawBackground();
            DrawRayCasting();
            DrawSprites();
            DrawMap();
            DrawPlayer();
            DrawFps();
        }
    }
}
