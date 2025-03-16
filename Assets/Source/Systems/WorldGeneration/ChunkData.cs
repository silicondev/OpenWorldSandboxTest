using Assets.Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems.WorldGeneration
{
    public class ChunkData
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

        public Location Id { get; }

        public Voxel[,,] Voxels { get; private set; }

        public int Width => Voxels.GetLength(0);
        public int Height => Voxels.GetLength(1);
        public int Depth => Voxels.GetLength(2);

        private Generator _generator;

        public ChunkData(Location id, Generator generator)
        {
            Id = id;
            _generator = generator;
        }

        public async Task Generate(Generator? generator = null)
        {
            if (generator == null)
                generator = _generator;

            Voxels = await generator.GenerateChunk(Id);
        }

        public Voxel GetBlock(Vector3 vector) =>
            GetBlock(Location.ClampVector(vector));

        public Voxel GetBlock(Location l)
        {
            //var arrayLocation = l + new Location(_generator.Settings.WorldSize.x / 2, 0, _generator.Settings.WorldSize.y / 2);
            var arrayLocation = l - (Id * _generator.Settings.ChunkSize);

            if (Voxels == null ||
                arrayLocation.X >= Width || arrayLocation.X < 0 ||
                arrayLocation.Z >= Depth || arrayLocation.Z < 0 ||
                arrayLocation.Y >= Height || arrayLocation.Y < 0)
                return null;

            return Voxels[arrayLocation.X, arrayLocation.Y, arrayLocation.Z];
        }

        public void SetBlock(Vector3 vector, VoxelType v) =>
            SetBlock(Location.ClampVector(vector), v);

        public void SetBlock(Location l, VoxelType v)
        {
            //var arrayLocation = l + new Location(_generator.Settings.WorldSize.x / 2, 0, _generator.Settings.WorldSize.y / 2);
            var arrayLocation = l - (Id * _generator.Settings.ChunkSize);

            if (Voxels == null ||
                arrayLocation.X >= Width || arrayLocation.X <= 0 ||
                arrayLocation.Z >= Depth || arrayLocation.Z <= 0 ||
                arrayLocation.Y >= Height || arrayLocation.Y <= 0)
                return;

            Voxels[arrayLocation.X, arrayLocation.Y, arrayLocation.Z].Type = v;
        }
    }
}
