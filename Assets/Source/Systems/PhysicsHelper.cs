using Assets.Source.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Source.Systems
{
    public static class PhysicsHelper
    {
        public static (float distance, Vector3 velocityMultiplier) GetCollisionDistance(Bounds bounds, Vector3 position, Vector3 velocity)
        {
            var currentBounds = new BoundingBox(bounds);
            (Vector3? origin, RaycastHit? hit) = currentBounds.Raycast(velocity);

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
    }
}
