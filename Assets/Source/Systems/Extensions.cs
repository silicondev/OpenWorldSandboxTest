using Assets.Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static T GetMax<T>(this IEnumerable<T> arr, Func<T, double> valFunc)
        {
            double max = arr.Select(v => valFunc(v)).Max();
            return arr.FirstOrDefault(x => valFunc(x) == max);
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

        public static int GetIndex(this Index index, int length) =>
            index.IsFromEnd ? length - index.Value : index.Value;

        public static Index ToIndex(this int value) =>
            value >= 0 ? value : ^(-value);

        public static T Get<T>(this T[,] array, Index index0, Index index1) =>
            array[index0.GetIndex(array.GetLength(0)), index1.GetIndex(array.GetLength(1))];

        public static T Get<T>(this T[,,] array, Index index0, Index index1, Index index2) =>
            array[index0.GetIndex(array.GetLength(0)), index1.GetIndex(array.GetLength(1)), index2.GetIndex(array.GetLength(2))];

        public static T Get<T>(this T[,,,] array, Index index0, Index index1, Index index2, Index index3) =>
            array[index0.GetIndex(array.GetLength(0)), index1.GetIndex(array.GetLength(1)), index2.GetIndex(array.GetLength(2)), index3.GetIndex(array.GetLength(3))];

        public static void Set<T>(this T[,] array, Index index0, Index index1, T value) =>
            array[index0.GetIndex(array.GetLength(0)), index1.GetIndex(array.GetLength(1))] = value;

        public static void Set<T>(this T[,,] array, Index index0, Index index1, Index index2, T value) =>
            array[index0.GetIndex(array.GetLength(0)), index1.GetIndex(array.GetLength(1)), index2.GetIndex(array.GetLength(2))] = value;

        public static void Set<T>(this T[,,,] array, Index index0, Index index1, Index index2, Index index3, T value) =>
            array[index0.GetIndex(array.GetLength(0)), index1.GetIndex(array.GetLength(1)), index2.GetIndex(array.GetLength(2)), index3.GetIndex(array.GetLength(3))] = value;

        public static T GetOrNull<T>(this T[] array, int index)
        {
            if (index >= 0 && index < array.Length)
                return array[index];
            return default;
        }

        public static T GetOrNull<T>(this T[,] array, int index0, int index1)
        {
            if (index0 >= 0 && index0 < array.GetLength(0) &&
                index1 >= 0 && index1 < array.GetLength(1))
                return array[index0, index1];
            return default;
        }

        public static T GetOrNull<T>(this T[,,] array, int index0, int index1, int index2)
        {
            if (index0 >= 0 && index0 < array.GetLength(0) &&
                index1 >= 0 && index1 < array.GetLength(1) &&
                index2 >= 0 && index2 < array.GetLength(2))
                return array[index0, index1, index2];
            return default;
        }

        public static T GetOrNull<T>(this T[,,,] array, int index0, int index1, int index2, int index3)
        {
            if (index0 >= 0 && index0 < array.GetLength(0) &&
                index1 >= 0 && index1 < array.GetLength(1) &&
                index2 >= 0 && index2 < array.GetLength(2) &&
                index3 >= 0 && index2 < array.GetLength(3))
                return array[index0, index1, index2, index3];
            return default;
        }

        public static void ToOpaqueMode(this Material material)
        {
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHATEST_ON");
            material.DisableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
        }

        public static void ToFadeMode(this Material material)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }
    }
}
