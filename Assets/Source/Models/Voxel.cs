using Assets.Source.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public BoundingBox GetBox() =>
            new BoundingBox(
                Location.ToVector3(),
                Location.ToVector3() - new Vector3(1, 0, 0),
                Location.ToVector3() - new Vector3(0, 0, 1),
                Location.ToVector3() - new Vector3(1, 0, 1),
                Location.ToVector3() - new Vector3(0, 1, 0),
                Location.ToVector3() - new Vector3(1, 1, 0),
                Location.ToVector3() - new Vector3(0, 1, 1),
                Location.ToVector3() - Vector3.one);
    }

    public enum VoxelType
    {
        VOID,
        GRASS,
        DIRT,
        STONE,
        PLANKS
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
