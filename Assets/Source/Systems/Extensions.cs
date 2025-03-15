using Assets.Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public static Vector3 Copy(this Vector3 vector) =>
            new Vector3(vector.x, vector.y, vector.z);

        public static Quaternion Copy(this Quaternion quat) =>
            new Quaternion(quat.x, quat.y, quat.z, quat.w);

        public static Quaternion ClampX(this Quaternion quat, float high, float low)
        {
            if (quat.x > high)
                return new Quaternion(high, quat.y, quat.z, quat.w);
            else if (quat.x < low)
                return new Quaternion(low, quat.y, quat.z, quat.w);
            return quat.Copy();
        }

        public static Quaternion ClampY(this Quaternion quat, float high, float low)
        {
            if (quat.y > high)
                return new Quaternion(quat.x, high, quat.z, quat.w);
            else if (quat.y < low)
                return new Quaternion(quat.x, low, quat.z, quat.w);
            return quat.Copy();
        }
        public static Quaternion ClampZ(this Quaternion quat, float high, float low)
        {
            if (quat.z > high)
                return new Quaternion(quat.x, quat.y, high, quat.w);
            else if (quat.z < low)
                return new Quaternion(quat.x, quat.y, low, quat.w);
            return quat.Copy();
        }
        public static Quaternion ClampW(this Quaternion quat, float high, float low)
        {
            if (quat.w > high)
                return new Quaternion(quat.x, quat.y, quat.z, high);
            else if (quat.w < low)
                return new Quaternion(quat.x, quat.y, quat.z, low);
            return quat.Copy();
        }
    }
}
