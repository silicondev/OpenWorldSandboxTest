using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems
{
    public static class MathHelper
    {
        public static int Sum(this IEnumerable<int> nums) =>
            (int)nums.Select(x => (double)x).Sum();

        public static float Sum(this IEnumerable<float> nums) =>
            (float)nums.Select(x => (double)x).Sum();

        public static double Sum(this IEnumerable<double> nums)
        {
            double total = 0d;
            foreach (var num in nums)
                total += num;
            return total;
        }

        public static Vector2 Sum(this IEnumerable<Vector2> vectors) =>
            new Vector2(vectors.Select(x => x.x).Sum(), vectors.Select(x => x.y).Sum());

        public static Vector3 Sum(this IEnumerable<Vector3> vectors) =>
            new Vector3(vectors.Select(x => x.x).Sum(), vectors.Select(x => x.y).Sum(), vectors.Select(x => x.z).Sum());

        public static Vector4 Sum(this IEnumerable<Vector4> vectors) =>
            new Vector4(vectors.Select(x => x.x).Sum(), vectors.Select(x => x.y).Sum(), vectors.Select(x => x.z).Sum(), vectors.Select(x => x.w).Sum());
    }
}
