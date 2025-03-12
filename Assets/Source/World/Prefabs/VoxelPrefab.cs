using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.World.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

namespace Assets.Source.World.Prefabs
{
    public class VoxelPrefab
    {
        public Texture2D SpriteSheet { get; set; }
        public Vector2 TopTexPos { get; set; }
        public Vector2 BottomTexPos { get; set; }
        public Vector2 FrontTexPos { get; set; }
        public Vector2 RightTexPos { get; set; }
        public Vector2 BackTexPos { get; set; }
        public Vector2 LeftTexPos { get; set; }

        public static Dictionary<VoxelType, VoxelPrefab> Prefabs = new()
        {
            {
                VoxelType.GRASS,
                new VoxelPrefab()
                {
                    TopTexPos = new Vector2(2, 0),
                    BottomTexPos = new Vector2(1, 0),
                    FrontTexPos = new Vector2(0, 0),
                    RightTexPos = new Vector2(0, 0),
                    BackTexPos = new Vector2(0, 0),
                    LeftTexPos = new Vector2(0, 0),
                    SpriteSheet = TextureHelper.LoadFromImage(@"Assets\Textures\tile.png")
                }
            },
            {
                VoxelType.DIRT,
                new VoxelPrefab()
                {
                    TopTexPos = new Vector2(1, 0),
                    BottomTexPos = new Vector2(1, 0),
                    FrontTexPos = new Vector2(1, 0),
                    RightTexPos = new Vector2(1, 0),
                    BackTexPos = new Vector2(1, 0),
                    LeftTexPos = new Vector2(1, 0),
                    SpriteSheet = TextureHelper.LoadFromImage(@"Assets\Textures\tile.png")
                }
            },
            {
                VoxelType.STONE,
                new VoxelPrefab()
                {
                    TopTexPos = new Vector2(3, 0),
                    BottomTexPos = new Vector2(3, 0),
                    FrontTexPos = new Vector2(3, 0),
                    RightTexPos = new Vector2(3, 0),
                    BackTexPos = new Vector2(3, 0),
                    LeftTexPos = new Vector2(3, 0),
                    SpriteSheet = TextureHelper.LoadFromImage(@"Assets\Textures\tile.png")
                }
            },
        };

        public Vector2[] GetVoxelUV() =>
            TextureHelper.GetVoxelUV(SpriteSheet, 8, TopTexPos, BottomTexPos, FrontTexPos, RightTexPos, BackTexPos, LeftTexPos);

        public Vector2[] GetVoxelUV(VoxelFace face)
        {
            switch (face)
            {
                case VoxelFace.TOP:
                    return TextureHelper.GetVoxelUV(SpriteSheet, 8, TopTexPos);
                case VoxelFace.BOTTOM:
                    return TextureHelper.GetVoxelUV(SpriteSheet, 8, BottomTexPos);
                case VoxelFace.LEFT:
                    return TextureHelper.GetVoxelUV(SpriteSheet, 8, LeftTexPos);
                case VoxelFace.RIGHT:
                    return TextureHelper.GetVoxelUV(SpriteSheet, 8, RightTexPos);
                case VoxelFace.FRONT:
                    return TextureHelper.GetVoxelUV(SpriteSheet, 8, FrontTexPos);
                case VoxelFace.BACK:
                    return TextureHelper.GetVoxelUV(SpriteSheet, 8, BackTexPos);
            }
            return new Vector2[6];
        }
    }
}
