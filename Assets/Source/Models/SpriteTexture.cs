using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Models
{
    public class SpriteTexture
    {
        public Texture2D Texture2D { get; }
        public int Width { get; }
        public int Height { get; }
        public int SpriteSize { get; }

        public SpriteTexture(Texture2D texture, int spriteSize)
        {
            Width = texture.width;
            Height = texture.height;
            Texture2D = texture;
            SpriteSize = spriteSize;
        }
    }
}
