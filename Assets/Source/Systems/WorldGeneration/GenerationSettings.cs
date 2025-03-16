using Assets.Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Systems.WorldGeneration
{
    public class GenerationSettings
    {
        public int WorldHeight { get; }
        public (int x, int y) WorldSize { get; }
        public int ChunkSize { get; }
        public (int x, int y) WorldSizeChunks { get; }
        public int Seed { get; }
        public int Sharpness { get; } = 128;

        public GenerationSettings(int worldHeight, int chunkSize, int seed, int chunkCountX, int chunkCountZ)
        {
            WorldSizeChunks = (chunkCountX, chunkCountZ);
            ChunkSize = chunkSize;
            WorldHeight = worldHeight;
            WorldSize = (WorldSizeChunks.x * ChunkSize, WorldSizeChunks.y * ChunkSize);
            Seed = seed;
        }
    }
}
