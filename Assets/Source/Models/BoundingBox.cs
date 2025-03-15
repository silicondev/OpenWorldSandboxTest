using Assets.Source.Interfaces;
using Assets.Source.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Source.Models
{
    public class BoundingBox : ICopyable<BoundingBox>
    {
        public Vector3 TopFrontRight { get; set; }
        public Vector3 TopFrontLeft { get; set; }
        public Vector3 TopRearRight { get; set; }
        public Vector3 TopRearLeft { get; set; }
        public Vector3 BottomFrontRight { get; set; }
        public Vector3 BottomFrontLeft { get; set; }
        public Vector3 BottomRearRight { get; set; }
        public Vector3 BottomRearLeft { get; set; }

        public BoundingBox(Vector3 tfr, Vector3 tfl, Vector3 trr, Vector3 trl, Vector3 bfr, Vector3 bfl, Vector3 brr, Vector3 brl)
        {
            TopFrontRight = tfr;
            TopFrontLeft = tfl;
            TopRearRight = trr;
            TopRearLeft = trl;
            BottomFrontRight = bfr;
            BottomFrontLeft = bfl;
            BottomRearRight = brr;
            BottomRearLeft = brl;
        }

        public BoundingBox(Bounds b) : this(
            b.max, 
            new Vector3(b.min.x, b.max.y, b.max.z),
            new Vector3(b.max.x, b.max.y, b.min.z),
            new Vector3(b.min.x, b.max.y, b.min.z),
            new Vector3(b.max.x, b.min.y, b.max.z),
            new Vector3(b.min.x, b.min.y, b.max.z),
            new Vector3(b.max.x, b.min.y, b.min.z),
            b.min)
        {

        }

        public bool ContainsPoint(Vector3 point) =>
            ContainsPointX(point) && ContainsPointY(point) && ContainsPointZ(point);

        public bool ContainsPointX(Vector3 point) =>
            point.x >= BottomRearLeft.x && point.x <= TopFrontRight.x;

        public bool ContainsPointY(Vector3 point) =>
            point.y >= BottomRearLeft.y && point.y <= TopFrontRight.y;

        public bool ContainsPointZ(Vector3 point) =>
            point.z >= BottomRearLeft.z && point.z <= TopFrontRight.z;

        public BoundingBox Translate(Vector3 vector)
        {
            var copy = Copy();
            copy.TopFrontRight += vector;
            copy.TopFrontLeft += vector;
            copy.TopRearRight += vector;
            copy.TopRearLeft += vector;
            copy.BottomFrontRight += vector;
            copy.BottomFrontLeft += vector;
            copy.BottomRearRight += vector;
            copy.BottomRearLeft += vector;
            return copy;
        }

        public BoundingBox Copy() =>
            new BoundingBox(TopFrontRight, TopFrontLeft, TopRearRight, TopRearLeft, BottomFrontRight, BottomFrontLeft, BottomRearRight, BottomRearLeft);

        public void DrawDebugLines(Color color)
        {
            Debug.DrawLine(TopFrontRight, TopFrontLeft, color);
            Debug.DrawLine(TopFrontRight, TopRearRight, color);
            Debug.DrawLine(TopFrontRight, BottomFrontRight, color);
            Debug.DrawLine(TopFrontLeft, TopRearLeft, color);
            Debug.DrawLine(TopFrontLeft, BottomFrontLeft, color);
            Debug.DrawLine(TopRearLeft, TopRearRight, color);
            Debug.DrawLine(TopRearLeft, BottomRearLeft, color);
            Debug.DrawLine(TopRearRight, BottomRearRight, color);
            Debug.DrawLine(BottomFrontRight, BottomFrontLeft, color);
            Debug.DrawLine(BottomFrontLeft, BottomRearLeft, color);
            Debug.DrawLine(BottomRearLeft, BottomRearRight, color);
            Debug.DrawLine(BottomRearRight, BottomFrontRight, color);
        }

        public void DrawDebugLines(Color color, Vector3 point)
        {
            Debug.DrawLine(TopFrontRight, point, color);
            Debug.DrawLine(TopFrontLeft, point, color);
            Debug.DrawLine(TopRearRight, point, color);
            Debug.DrawLine(TopRearLeft, point, color);
            Debug.DrawLine(BottomFrontRight, point, color);
            Debug.DrawLine(BottomFrontLeft, point, color);
            Debug.DrawLine(BottomRearRight, point, color);
            Debug.DrawLine(BottomRearLeft, point, color);
        }

        public Vector3[] LeastCombinedDistanceFace(Vector3 point)
        {
            var top = new Vector3[] { TopFrontRight, TopFrontLeft, TopRearRight, TopRearLeft };
            var bottom = new Vector3[] { BottomFrontRight, BottomFrontLeft, BottomRearRight, BottomRearLeft };
            var right = new Vector3[] { TopFrontRight, TopRearRight, BottomFrontRight, BottomRearRight };
            var left = new Vector3[] { TopFrontLeft, TopRearLeft, BottomFrontLeft, BottomRearLeft };
            var front = new Vector3[] { TopFrontRight, TopFrontLeft, BottomFrontRight, BottomFrontLeft };
            var back = new Vector3[] { TopRearRight, TopRearLeft, BottomRearRight, BottomRearLeft };

            var dict = new Dictionary<Vector3[], float>()
            {
                { top, top.Select(x => Vector3.Distance(x, point)).Sum() },
                { bottom, bottom.Select(x => Vector3.Distance(x, point)).Sum() },
                { right, right.Select(x => Vector3.Distance(x, point)).Sum() },
                { left, left.Select(x => Vector3.Distance(x, point)).Sum() },
                { front, front.Select(x => Vector3.Distance(x, point)).Sum() },
                { back, back.Select(x => Vector3.Distance(x, point)).Sum() }
            };

            var sorted = MathHelper.Sort(dict, x => x.Value);
            return sorted[0].Key;
        }

        public (Vector3 origin, RaycastHit hit)[] Raycast(Vector3 direction)
        {
            var list = new List<(Vector3 origin, RaycastHit hit)>();

            var dirX = new Vector3(direction.x, 0, 0);
            var dirY = new Vector3(0, direction.y, 0);
            var dirZ = new Vector3(0, 0, direction.z);

            var vertices = new Vector3[] { TopFrontRight, TopFrontLeft, TopRearRight, TopRearLeft, BottomFrontRight, BottomFrontLeft, BottomRearRight, BottomRearLeft };

            foreach (var vertex in vertices)
            {
                if (Physics.Raycast(vertex, dirX, out RaycastHit hitX, Vector3.Distance(vertex, vertex + dirX)))
                    list.Add((vertex, hitX));
                if (Physics.Raycast(vertex, dirY, out RaycastHit hitY, Vector3.Distance(vertex, vertex + dirY)))
                    list.Add((vertex, hitY));
                if (Physics.Raycast(vertex, dirZ, out RaycastHit hitZ, Vector3.Distance(vertex, vertex + dirZ)))
                    list.Add((vertex, hitZ));
            }

            foreach (var item in list)
            {
                Debug.DrawLine(item.origin, item.hit.point, Color.blue);
            }

            return list.ToArray();
        }

        public (Vector3? origin, RaycastHit? hit) RaycastMin(Vector3 direction)
        {
            var casts = Raycast(direction);
            if (casts.Length == 0)
                return (null, null);
            return casts.GetMin(x => Vector3.Distance(x.hit.point, x.origin));
        }
    }
}
