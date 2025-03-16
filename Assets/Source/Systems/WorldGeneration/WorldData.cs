using Assets.Source.Models;
using Assets.Source.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems.WorldGeneration
{
    public class WorldData
    {
        public Voxel this[Location l]
        {
            get => GetBlock(l);
            set => SetBlock(l, value.Type);
        }

        public Voxel this[Vector3 v]
        {
            get => GetBlock(v);
            set => SetBlock(v, value.Type);
        }

        public ChunkData[,] Chunks { get; private set; }

        private Generator _generator;

        public WorldData(Generator generator)
        {
            _generator = generator;
        }

        public async Task Generate(Location chunkId, int chunkLoadRadius, Generator? generator = null)
        {
            (RealRange rangeX, RealRange rangeY) = GetChunkLoadDistance(chunkId, chunkLoadRadius);
            await Generate(rangeX, rangeY, generator);
        }

        public async Task Generate(Generator? generator = null)
        {
            if (generator == null)
                generator = _generator;

            var xRange = new RealRange(-_generator.Settings.WorldSizeChunks.x / 2, _generator.Settings.WorldSizeChunks.x / 2);
            var yRange = new RealRange(-_generator.Settings.WorldSizeChunks.y / 2, _generator.Settings.WorldSizeChunks.y / 2);
            await Generate(xRange, yRange, generator);
        }

        public async Task Generate(RealRange xRange, RealRange yRange, Generator? generator = null)
        {
            if (generator == null)
                generator = _generator;

            bool firstGen = false;
            if (Chunks == null)
            {
                Chunks = new ChunkData[generator.Settings.WorldSizeChunks.x, generator.Settings.WorldSizeChunks.y];
                firstGen = true;
            }
            
            for (int x = xRange.Start; x < xRange.End; x++)
            {
                for (int z = yRange.Start; z < yRange.End; z++)
                {
                    var location = new Location(x, 0, z);
                    if (firstGen || GetChunkFromId(location) == null)
                    {
                        int ax = x + generator.Settings.WorldSizeChunks.x / 2;
                        int az = z + generator.Settings.WorldSizeChunks.y / 2;

                        if (ax > 0 && ax < generator.Settings.WorldSizeChunks.x &&
                            az > 0 && az < generator.Settings.WorldSizeChunks.y)
                        {
                            Chunks[ax, az] = new ChunkData(location, generator);
                            await Chunks[ax, az].Generate();
                        }
                    }
                }
            }
        }

        public ChunkData GetChunk(Location l)
        {
            var arrayWorldPos = l + new Location(_generator.Settings.WorldSize.x / 2, 0, _generator.Settings.WorldSize.y / 2);
            var id = arrayWorldPos / _generator.Settings.ChunkSize;

            if (Chunks == null ||
                id.X < 0 || id.X >= Chunks.GetLength(0) ||
                id.Z < 0 || id.Z >= Chunks.GetLength(1))
                return null;

            return Chunks[id.X, id.Z];
        }

        public ChunkData GetChunkFromId(Location id)
        {
            var arrayId = id + new Location(_generator.Settings.WorldSizeChunks.x / 2, 0, _generator.Settings.WorldSizeChunks.y / 2);

            if (Chunks == null ||
                arrayId.X < 0 || arrayId.X >= Chunks.GetLength(0) ||
                arrayId.Z < 0 || arrayId.Z >= Chunks.GetLength(1))
                return null;

            return Chunks[arrayId.X, arrayId.Z];
        }

        public Voxel GetBlock(Vector3 vector) =>
            GetBlock(Location.ClampVector(vector));

        public Voxel GetBlock(Location l) =>
            GetChunk(l)?.GetBlock(l);

        public void SetBlock(Vector3 vector, VoxelType v) =>
            SetBlock(Location.ClampVector(vector), v);

        public void SetBlock(Location l, VoxelType v) =>
            GetChunk(l)?.SetBlock(l, v);

        public static (RealRange xRange, RealRange yRange) GetChunkLoadDistance(Location chunkId, int chunkRadius)
        {
            Location start = chunkId - chunkRadius;
            Location end = chunkId + chunkRadius;
            return (new RealRange(start.X, end.X), new RealRange(start.Z, end.Z));
        }
    }
}
