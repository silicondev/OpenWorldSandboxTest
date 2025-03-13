using Assets.Source.Systems.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.World.Objects
{
    public class Player : MovableObject
    {
        public Player()
        {

        }

        protected override void BuildMovable(GameObject obj)
        {
            var newObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.GetComponent<MeshFilter>().mesh = newObj.GetComponent<MeshFilter>().mesh;
            obj.GetComponent<Renderer>().material = newObj.GetComponent<Renderer>().material;

            GameObject.Destroy(newObj);
        }

        protected override void OnStart()
        {

        }

        protected override void OnUpdate()
        {
            Rotation = new Quaternion(0, Rotation.y, 0, 0);
            GameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);

            //if (IsGrounded)
            //{
            //    var vel = GameObject.GetComponent<Rigidbody>().velocity;
            //    var newVel = new Vector3(vel.x > 0.01 ? vel.x : 0, 0, vel.z > 0.01 ? vel.z : 0);
            //    GameObject.GetComponent<Rigidbody>().velocity = newVel;
            //}
        }
    }
}
