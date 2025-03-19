using Assets.Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

namespace Assets.Source.Systems
{
    public static class PhysicsHelper
    {
        public static (float distance, Vector3 velocityMultiplier) GetCollisionDistance(Bounds bounds, Vector3 position, Vector3 velocity)
        {
            var currentBounds = new BoundingBox(bounds);
            (Vector3? origin, RaycastHit? hit) = currentBounds.RaycastMin(velocity);

            if (hit != null)
            {
                Debug.DrawLine(hit.Value.point, origin.Value, Color.white);
                float newDist = Vector3.Distance(hit.Value.point, origin.Value);
                var vecMult = (hit.Value.normal - Vector3.one) / -1;

                currentBounds.Translate(Vector3.ClampMagnitude(velocity, newDist)).DrawDebugLines(Color.red);
                return (newDist, vecMult);
            }
            else
            {
                currentBounds.Translate(velocity).DrawDebugLines(Color.red);
                return (Vector3.Distance(position, position + velocity), Vector3.one);
            }
        }

        public static (Vector3 vector, Vector3 normal)[] GetCollisions(Bounds bounds, Vector3 position, Vector3 velocity)
        {
            var currentBounds = new BoundingBox(bounds);
            var hits = currentBounds.Raycast(velocity);

            var list = new List<(Vector3 vector, Vector3 normal)>();
            foreach ((Vector3 origin, RaycastHit hit) in hits)
                list.Add((hit.point - origin, hit.normal));

            return list.DistinctBy(x => x.normal).ToArray();
        }

        public static (Vector3 newVelocity, Vector3[] normals) CollideVelocity(Bounds bounds, Vector3 position, Vector3 velocity, float offset)
        {
            var newVelocity = new Vector3(velocity.x, velocity.y, velocity.z);
            (Vector3 vector, Vector3 normal)[] hits = GetCollisions(bounds, position, newVelocity * Time.deltaTime);

            foreach ((Vector3 collisionVector, Vector3 collisionNormal) in hits)
            {
                switch ((collisionNormal.x, collisionNormal.y, collisionNormal.z))
                {
                    case (1, 0, 0):
                        newVelocity = new Vector3(collisionVector.x + offset, newVelocity.y, newVelocity.z);
                        break;
                    case (-1, 0, 0):
                        newVelocity = new Vector3(collisionVector.x - offset, newVelocity.y, newVelocity.z);
                        break;
                    case (0, 1, 0):
                        newVelocity = new Vector3(newVelocity.x, collisionVector.y + offset, newVelocity.z);
                        break;
                    case (0, -1, 0):
                        newVelocity = new Vector3(newVelocity.x, collisionVector.y - offset, newVelocity.z);
                        break;
                    case (0, 0, 1):
                        newVelocity = new Vector3(newVelocity.x, newVelocity.y, collisionVector.z + offset);
                        break;
                    case (0, 0, -1):
                        newVelocity = new Vector3(newVelocity.x, newVelocity.y, collisionVector.z - offset);
                        break;
                }
            }

            return (newVelocity, hits.Select(x => x.normal).ToArray());
        }

        public static Vector3[] Vector3Between(Vector3 a, Vector3 b, int count)
        {
            var list = new List<Vector3>();
            Vector3 difference = (a - b) / count;
            for (int i = 1; i <= count; i++)
            {
                list.Add(a + (difference * i));
            }
            return list.ToArray();
        }
    }
}
