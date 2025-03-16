using Assets.Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems.WorldGeneration
{
    public class Generator
    {
        public GenerationSettings Settings { get; }
        public Generator(GenerationSettings settings)
        {
            Settings = settings;
        }

        // Setup basic generator
        private Func<Location, GenerationSettings, Task<Voxel[,,]>> _chunkGenerator { get; set; } = (Location chunkId, GenerationSettings settings) =>
        {
            return Task.Factory.StartNew(() =>
            {
                Location worldLocation = chunkId * settings.ChunkSize;
                var voxels = new Voxel[settings.ChunkSize, settings.WorldHeight, settings.ChunkSize];

                for (int x = 0; x < settings.ChunkSize; x++)
                {
                    for (int z = 0; z < settings.ChunkSize; z++)
                    {
                        float perlin = Mathf.PerlinNoise((float)(x + worldLocation.X + settings.Seed) / (float)settings.Sharpness, (float)(z + worldLocation.Z + settings.Seed) / (float)settings.Sharpness);
                        float real = perlin * (float)settings.WorldHeight;
                        int height = Mathf.Clamp((int)real, 0, settings.WorldHeight);

                        for (int y = 0; y < settings.WorldHeight; y++)
                        {
                            var location = worldLocation + new Location(x, y, z);
                            if (y < height - 3)
                                voxels[x, y, z] = new Voxel(location, VoxelType.STONE);
                            else if (y < height)
                                voxels[x, y, z] = new Voxel(location, VoxelType.DIRT);
                            else if (y == height)
                                voxels[x, y, z] = new Voxel(location, VoxelType.GRASS);
                            else
                                voxels[x, y, z] = new Voxel(location, VoxelType.VOID);
                        }
                    }
                }

                return voxels;
            });
        };

        public Task<Voxel[,,]> GenerateChunk(Location chunkId) =>
            _chunkGenerator.Invoke(chunkId, Settings);

        public void SetGenerator(Func<Location, GenerationSettings, Task<Voxel[,,]>> generator) =>
            _chunkGenerator = generator;
    }
}
