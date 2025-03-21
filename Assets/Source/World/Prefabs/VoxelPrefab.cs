﻿using Assets.Source.Models;
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
        public Vector2 TopTexPos { get; set; }
        public Vector2 BottomTexPos { get; set; }
        public Vector2 FrontTexPos { get; set; }
        public Vector2 RightTexPos { get; set; }
        public Vector2 BackTexPos { get; set; }
        public Vector2 LeftTexPos { get; set; }

        public Vector2[] GetVoxelUV(int spriteSheetWidth, int spriteSheetHeight) =>
            TextureHelper.GetVoxelUV(spriteSheetWidth, spriteSheetHeight, 8, TopTexPos, BottomTexPos, FrontTexPos, RightTexPos, BackTexPos, LeftTexPos);

        public Vector2[] GetVoxelUV(VoxelFace face, int spriteSheetWidth, int spriteSheetHeight, int spriteSize)
        {
            switch (face)
            {
                case VoxelFace.TOP:
                    return TextureHelper.GetVoxelUV(spriteSheetWidth, spriteSheetHeight, spriteSize, TopTexPos);
                case VoxelFace.BOTTOM:
                    return TextureHelper.GetVoxelUV(spriteSheetWidth, spriteSheetHeight, spriteSize, BottomTexPos);
                case VoxelFace.LEFT:
                    return TextureHelper.GetVoxelUV(spriteSheetWidth, spriteSheetHeight, spriteSize, LeftTexPos);
                case VoxelFace.RIGHT:
                    return TextureHelper.GetVoxelUV(spriteSheetWidth, spriteSheetHeight, spriteSize, RightTexPos);
                case VoxelFace.FRONT:
                    return TextureHelper.GetVoxelUV(spriteSheetWidth, spriteSheetHeight, spriteSize, FrontTexPos);
                case VoxelFace.BACK:
                    return TextureHelper.GetVoxelUV(spriteSheetWidth, spriteSheetHeight, spriteSize, BackTexPos);
            }
            return new Vector2[6];
        }
    }
}
