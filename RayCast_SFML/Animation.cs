using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace RayCast_SFML
{
    internal class Animation
    {
        public double CurrentFrame;
        public double Speed;
        public bool IsPlaying;
        public bool IsFlip;
        public bool IsLoop;
        public Sprite Sprite;
        public List<IntRect> Frames;
        public List<IntRect> FramesFlip;

        public Animation()
        {
            CurrentFrame = 0;
            IsPlaying = true;
            IsFlip = false;
            IsLoop = true;
        }

        public void Tick(double time)
        {
            if (!IsPlaying) return;

            CurrentFrame += Speed * time;

            if (CurrentFrame > Frames.Count)
            {
                CurrentFrame -= Frames.Count;
                if (!IsLoop)
                {
                    IsPlaying = false;
                    return;
                }
            }

            int i = (int)CurrentFrame;
            Sprite.TextureRect = Frames[i];
            if (IsFlip) Sprite.TextureRect = FramesFlip[i];
        }
    }
}
