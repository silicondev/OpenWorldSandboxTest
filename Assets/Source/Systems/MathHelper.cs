using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Source.Systems
{
    public static class MathHelper
    {
        //public static int Sum(this IEnumerable<int> nums) =>
        //    (int)nums.Select(x => (double)x).Sum();

        //public static float Sum(this IEnumerable<float> nums) =>
        //    (float)nums.Select(x => (double)x).Sum();

        //public static double Sum(this IEnumerable<double> nums)
        //{
        //    double total = 0d;
        //    foreach (var num in nums)
        //        total += num;
        //    return total;
        //}

        public static Vector2 SumVector(this IEnumerable<Vector2> vectors) =>
            new Vector2(vectors.Select(x => x.x).Sum(), vectors.Select(x => x.y).Sum());

        public static Vector3 SumVector(this IEnumerable<Vector3> vectors) =>
            new Vector3(vectors.Select(x => x.x).Sum(), vectors.Select(x => x.y).Sum(), vectors.Select(x => x.z).Sum());

        public static Vector4 SumVector(this IEnumerable<Vector4> vectors) =>
            new Vector4(vectors.Select(x => x.x).Sum(), vectors.Select(x => x.y).Sum(), vectors.Select(x => x.z).Sum(), vectors.Select(x => x.w).Sum());

        public static T[] Sort<T>(IEnumerable<T> arr, Func<T, double> comp)
        {
            // QUICK SORT
            if (arr == null || arr.Count() == 0)
                return new T[0];
            else if (arr.Count() == 1)
                return new T[] { arr.ElementAt(0) };

            int pivot = (int)(arr.Count() / 2f);
            var lt = new List<T>();
            var gt = new List<T>();
            for (int i = 0; i < arr.Count(); i++)
            {
                if (i == pivot)
                    continue;
                if (comp(arr.ElementAt(i)) > comp(arr.ElementAt(pivot)))
                    gt.Add(arr.ElementAt(i));
                else
                    lt.Add(arr.ElementAt(i));
            }
            var list = new List<T>();
            list.AddRange(Sort(lt, comp));
            list.Add(arr.ElementAt(pivot));
            list.AddRange(Sort(gt, comp));

            return list.ToArray();
        }
    }
}
