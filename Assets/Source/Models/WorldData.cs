using Assets.Source.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Models
{
    public class WorldData
    {
        public Voxel this[Location l]
        {
            get => GetBlock(l);
            set => SetBlock(l, value.Type);
        }

        public Voxel[,,] Voxels { get; private set; }

        public int Width => Voxels.GetLength(0);
        public int Height => Voxels.GetLength(1);
        public int Depth => Voxels.GetLength(2);

        private Func<int, int, Voxel[,,]> _generator;
        private int _worldSize;
        private int _worldHeight;

        public WorldData(int worldSize, int worldHeight, Func<int, int, Voxel[,,]>? generator = null)
        {
            _worldSize = worldSize;
            _worldHeight = worldHeight;
            if (generator == null)
                generator = (int worldSize, int worldHeight) =>
                {
                    var voxels = new Voxel[worldSize, worldHeight, worldSize];
                    for (int x = 0; x < worldSize; x++)
                        for (int z = 0; z < worldSize; z++)
                            for (int y = 0; y < worldHeight; y++)
                            {
                                voxels[x, y, z] = new Voxel(new Location(x - worldSize / 2, y, z - worldSize / 2), VoxelType.VOID);
                            }
                    return voxels;
                };
            _generator = generator;
        }

        public void Generate(Func<int, int, Voxel[,,]>? generator = null)
        {
            if (generator == null)
                generator = _generator;

            Voxels = generator(_worldSize, _worldHeight);
        }

        public Voxel GetBlock(Location l)
        {
            var arrayLocation = l + new Location(_worldSize / 2, 0, _worldSize / 2);

            if (arrayLocation.X >= Width || arrayLocation.X < 0 ||
                arrayLocation.Z >= Depth || arrayLocation.Z < 0 ||
                arrayLocation.Y >= Height || arrayLocation.Y < 0)
                return null;

            return Voxels[arrayLocation.X, arrayLocation.Y, arrayLocation.Z];
        }

        public void SetBlock(Location l, VoxelType v)
        {
            var arrayLocation = l + new Location(_worldSize / 2, 0, _worldSize / 2);

            if (arrayLocation.X >= Width || arrayLocation.X <= 0 ||
                arrayLocation.Z >= Depth || arrayLocation.Z <= 0 ||
                arrayLocation.Y >= Height || arrayLocation.Y <= 0)
                return;

            Voxels[arrayLocation.X, arrayLocation.Y, arrayLocation.Z].Type = v;
        }
    }
}
