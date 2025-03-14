using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.Systems.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Source.World.Objects
{
    public class Player : InGameObject
    {
        public Player()
        {

        }

        protected override void Build(GameObject obj)
        {
            var newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.GetComponent<MeshFilter>().mesh = newObj.GetComponent<MeshFilter>().mesh;
            obj.GetComponent<Renderer>().material = newObj.GetComponent<Renderer>().material;
            obj.GetComponent<MeshCollider>().convex = true;
            obj.GetComponent<MeshCollider>().isTrigger = true;
            obj.transform.localScale = new Vector3(0.75f, 1.8f, 0.75f);

            GameObject.Destroy(newObj);
        }

        protected override void OnStart()
        {
            GameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            GravityEnabled = true;
        }

        protected override void OnUpdate()
        {
            Rotation = new Quaternion(0, Rotation.y, 0, 0);
        }

        private float groundOffset = 0.0001f;

        protected override (Vector3 position, Vector3 velocity) CalculateMovement()
        {
            (float distance, Vector3 mult) = PhysicsHelper.GetCollisionDistance(Renderer.bounds, Position, Velocity * Time.fixedDeltaTime);

            return (Position + Vector3.ClampMagnitude(Velocity, distance - groundOffset), new Vector3(Velocity.x * mult.x, Velocity.y * mult.y, Velocity.z * mult.z));
        }

        protected override void OnTriggerEnter(Collider collider)
        {

            Console.WriteLine();
        }

        protected override void OnCollisionExit(Collision collision)
        {

        }
    }
}
