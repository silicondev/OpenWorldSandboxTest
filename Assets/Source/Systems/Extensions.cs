﻿using Assets.Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Systems
{
    public static class Extensions
    {
        public static Voxel GetVoxel(this Voxel[] data, Location location) =>
            data.FirstOrDefault(v => v.Location == location);

        public static bool IsNullOrVoid(this VoxelType? type) =>
            type == null || type == VoxelType.VOID;

        public static T GetMin<T>(this IEnumerable<T> arr, Func<T, double> valFunc)
        {
            double min = arr.Select(v => valFunc(v)).Min();
            return arr.FirstOrDefault(x => valFunc(x) == min);
        }
    }
}
