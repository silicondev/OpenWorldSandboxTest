using Assets.Source.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Models
{
    public class Voxel
    {
        public VoxelType Type { get; set; }
        public Location Location { get; set; }

        public Voxel(Location location, VoxelType type)
        {
            Location = location;
            Type = type;
        }
    }

    public enum VoxelType
    {
        VOID,
        GRASS,
        DIRT,
        STONE
    }

    public enum VoxelFace
    {
        TOP,
        BOTTOM,
        RIGHT,
        LEFT,
        FRONT,
        BACK
    }
}
