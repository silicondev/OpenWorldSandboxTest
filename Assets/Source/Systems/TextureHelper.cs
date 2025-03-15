using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems
{
    public static class TextureHelper
    {
        public static Texture2D LoadFromImage(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException("File does not exist.");

            var bytes = File.ReadAllBytes(path);
            var texture = new Texture2D(2, 2);
            texture.filterMode = FilterMode.Point;
            texture.LoadImage(bytes);
            return texture;
        }

        public static Vector2[] GetVoxelUV(Texture2D sheet, int voxelSize, Vector2 topPos, Vector2 bottomPos, Vector2 frontPos, Vector2 rightPos, Vector2 backPos, Vector2 leftPos)
        {
            if (!int.TryParse((sheet.width / voxelSize).ToString(), out int voxelWidth) || !int.TryParse((sheet.height / voxelSize).ToString(), out int voxelHeight))
                throw new ArgumentException("Sheet size and voxel size do not divide equally.");

            if (topPos.x > voxelWidth || topPos.x < 0 || topPos.y > voxelHeight || topPos.y < 0)
                throw new ArgumentException("Top pos is not inside sprite sheet.");

            if (bottomPos.x > voxelWidth || bottomPos.x < 0 || bottomPos.y > voxelHeight || bottomPos.y < 0)
                throw new ArgumentException("Top pos is not inside sprite sheet.");

            if (frontPos.x > voxelWidth || frontPos.x < 0 || frontPos.y > voxelHeight || frontPos.y < 0)
                throw new ArgumentException("Top pos is not inside sprite sheet.");

            if (rightPos.x > voxelWidth || rightPos.x < 0 || rightPos.y > voxelHeight || rightPos.y < 0)
                throw new ArgumentException("Top pos is not inside sprite sheet.");

            if (backPos.x > voxelWidth || backPos.x < 0 || backPos.y > voxelHeight || backPos.y < 0)
                throw new ArgumentException("Top pos is not inside sprite sheet.");

            if (leftPos.x > voxelWidth || leftPos.x < 0 || leftPos.y > voxelHeight || leftPos.y < 0)
                throw new ArgumentException("Top pos is not inside sprite sheet.");

            var voxelCount = new Vector2(voxelWidth, voxelHeight);
            var voxelRel = new Vector2((float)voxelSize / (float)sheet.width, (float)voxelSize / (float)sheet.height);

            var uv = new Vector2[24];

            var frontRel = frontPos / voxelCount;
            var topRel = topPos / voxelCount;
            var backRel = backPos / voxelCount;
            var bottomRel = bottomPos / voxelCount;
            var leftRel = leftPos / voxelCount;
            var rightRel = rightPos / voxelCount;

            /*
            2 --- 3
            |     |
            |     |
            0 --- 1

            */

            Vector2 voxelRelX = new Vector2(voxelRel.x, 0);
            Vector2 voxelRelY = new Vector2(0, voxelRel.y);

            // FRONT 2 3 0 1
            uv[02] = frontRel;
            uv[03] = frontRel + voxelRelX;
            uv[00] = frontRel - voxelRelY;
            uv[01] = frontRel + voxelRelX - voxelRelY;

            // BACK 11 10 7 6
            uv[11] = backRel;
            uv[10] = backRel + voxelRelX;
            uv[07] = backRel - voxelRelY;
            uv[06] = backRel + voxelRelX - voxelRelY;

            // RIGHT 17 18 16 19
            uv[17] = rightRel;
            uv[18] = rightRel + voxelRelX;
            uv[16] = rightRel - voxelRelY;
            uv[19] = rightRel + voxelRelX - voxelRelY;

            // LEFT 21 22 20 23
            uv[21] = leftRel;
            uv[22] = leftRel + voxelRelX;
            uv[20] = leftRel - voxelRelY;
            uv[23] = leftRel + voxelRelX - voxelRelY;

            // TOP 4 5 8 9
            uv[04] = topRel;
            uv[05] = topRel + voxelRelX;
            uv[08] = topRel - voxelRelY;
            uv[09] = topRel + voxelRelX - voxelRelY;

            // BOTTOM 13 12 14 15
            uv[13] = bottomRel;
            uv[12] = bottomRel + voxelRelX;
            uv[14] = bottomRel - voxelRelY;
            uv[15] = bottomRel + voxelRelX - voxelRelY;

            return uv;
        }

        public static Vector2[] GetVoxelUV(Texture2D sheet, int voxelSize, Vector2 pos, float offset = 0.01f)
        {
            if (!int.TryParse((sheet.width / voxelSize).ToString(), out int voxelWidth) || !int.TryParse((sheet.height / voxelSize).ToString(), out int voxelHeight))
                throw new ArgumentException("Sheet size and voxel size do not divide equally.");

            var voxelCount = new Vector2(voxelWidth, voxelHeight);

            var voxelRel = new Vector2((float)voxelSize / (float)sheet.width, (float)voxelSize / (float)sheet.height);
            var voxelRelX = new Vector2(voxelRel.x, 0);
            var voxelRelY = new Vector2(0, voxelRel.y);

            var offsetX = new Vector2(offset * voxelRel.x, 0);
            var offsetY = new Vector2(0, offset * voxelRel.y);

            var rel = pos / voxelCount;

            var uv = new Vector2[6];

            /*

            1 --- 0
            | \   |
            |   \ |
            x --- 2

            5 --- x
            | \   |
            |   \ |
            3 --- 4


            */

            uv[0] = rel + voxelRel;
            uv[1] = rel + voxelRelY + offsetX;
            uv[2] = rel + voxelRelX + offsetY;

            uv[3] = rel + offsetX + offsetY;
            uv[4] = rel + voxelRelX + offsetY;
            uv[5] = rel + voxelRelY + offsetX;

            return uv;
        }
    }
}
