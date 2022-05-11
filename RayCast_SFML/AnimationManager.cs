using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace RayCast_SFML
{
    internal class AnimationManager
    {
        public string CurrentAnimation;
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();

        public void Create(string name, SFML.Graphics.Texture texture, int x, int y, int h, int w, int count, double speed, int step = 0, bool isLoop = true)
        {
            Animation animation = new Animation();
            animation.Speed = speed;
            animation.IsLoop = isLoop;
            animation.Sprite.Texture = texture;
            animation.Sprite.Origin = new Vector2f(0, h);

            for (int i = 0; i < count; i++) {
                animation.Frames.Add(new IntRect(x + i * step, y, w, h));
                animation.FramesFlip.Add(new IntRect(x + i * step + w, y, -w, h));
            }

            Animations[name] = animation;
            CurrentAnimation = name;
        }

        public void Set(string name)
        {
            CurrentAnimation = name;
            Animations[CurrentAnimation].IsFlip = false;
        }

        public void Draw(RenderWindow window, int x = 0, int y = 0)
        {
            Animations[CurrentAnimation].Sprite.Position = new Vector2f(x, y);
            window.Draw(Animations[CurrentAnimation].Sprite);
        }

        public void Flip(bool b = true)
        {
            Animations[CurrentAnimation].IsFlip = b;
        }

        public void Tick(double time)
        {
            Animations[CurrentAnimation].Tick(time);
        }

        public void Pause()
        {
            Animations[CurrentAnimation].IsPlaying = false;
        }

        public void Play()
        {
            Animations[CurrentAnimation].IsPlaying = true;
        }

        public void Play(string name)
        {
            Animations[name].IsPlaying = true;
        }

        public bool IsPlaying()
        {
            return Animations[CurrentAnimation].IsPlaying;
        }

        public double GetH()
        {
            return Animations[CurrentAnimation].Frames[0].Height;
        }
        public double GetW()
        {
            return Animations[CurrentAnimation].Frames[0].Width;
        }
    }
}
