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
        public GameObject Camera { get; private set; } = new GameObject();

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
            Gravity = 1f;

            Camera.tag = "MainCamera";
            Camera.name = "Camera";
            Camera.transform.SetParent(GameObject.transform);
            Camera.transform.localPosition = new Vector3(0, 0.35f, 0.099f);
            Camera.AddComponent<Camera>();
        }

        private int _updateTimeCount = 0;
        private bool _updateTime => _updateTimeCount >= 1f / Time.fixedDeltaTime;

        private Vector3 _lastMouse = new Vector3(-1, -1, -1);

        protected override void OnUpdate()
        {
            Rotation = new Quaternion(0, Rotation.y, 0, 0);

            if (!GameSystem.Paused)
            {
                if (_lastMouse == new Vector3(-1, -1, -1))
                    _lastMouse = Input.mousePosition;

                if (_lastMouse != Input.mousePosition)
                {
                    var diff = Input.mousePosition - _lastMouse;
                    var camRot = Camera.gameObject.transform.rotation;
                    //Camera.gameObject.transform.rotation = new Quaternion(camRot.x + diff.y, camRot.y, camRot.z, camRot.w);
                    //Rotation = new Quaternion(Rotation.x, Rotation.y + diff.x, Rotation.z, Rotation.w);

                    _lastMouse = Input.mousePosition;
                }
            }
            

            _updateTimeCount++;
            if (_updateTime)
            {

                _updateTimeCount = 0;
            }
        }

        private float _movementSpeed = 1f;
        private float _jumpPower = 2f;

        private Vector3[] _normals = new Vector3[0];

        protected override (Vector3 position, Vector3 velocity) CalculateMovement()
        {
            var newVelocity = Velocity;

            var keys = new List<KeyCode>();
            keys.AddRange(KeyboardHelper.GetKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D));
            keys.AddRange(KeyboardHelper.GetPressKeys(KeyCode.Space, KeyCode.Escape));
            keys.AddRange(KeyboardHelper.ProcessKeyQueue());

            IsGrounded = _normals.Contains(Vector3.up);

            if (keys.Contains(KeyCode.W))
                newVelocity += Vector3.ClampMagnitude(new Vector3(0, 0, _movementSpeed) * Time.fixedDeltaTime, _movementSpeed);
            if (keys.Contains(KeyCode.A))
                newVelocity += Vector3.ClampMagnitude(new Vector3(-_movementSpeed, 0, 0) * Time.fixedDeltaTime, _movementSpeed);
            if (keys.Contains(KeyCode.S))
                newVelocity += Vector3.ClampMagnitude(new Vector3(0, 0, -_movementSpeed) * Time.fixedDeltaTime, _movementSpeed);
            if (keys.Contains(KeyCode.D))
                newVelocity += Vector3.ClampMagnitude(new Vector3(_movementSpeed, 0, 0) * Time.fixedDeltaTime, _movementSpeed);
            if (keys.Contains(KeyCode.Space))
            {
                if (IsGrounded)
                    newVelocity += new Vector3(0, _jumpPower, 0);
                //else
                //    KeyboardHelper.AddToQueue(KeyCode.Space, 1);
            }
            if (keys.Contains(KeyCode.Escape))
                GameSystem.Paused = !GameSystem.Paused;

            (newVelocity, _normals) = PhysicsHelper.CollideVelocity(Renderer.bounds, Position, newVelocity, 0.00001f);

            return (Position + newVelocity * Time.fixedDeltaTime, newVelocity);
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
