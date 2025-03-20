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
using UnityEngine.UI;

namespace Assets.Source.World.Objects
{
    public class Player : InGameObject
    {
        public GameObject Camera { get; private set; } = new GameObject();

        public Location Location => Location.ClampVector(GameObject.transform.position);

        public Player()
        {

        }

        public Vector3 Front => Quaternion.Euler(new Vector3(0, Camera.transform.eulerAngles.y, 0)) * Vector3.forward;
        //public Vector3 Front => Camera.transform.forward;
        public Vector3 Right => Quaternion.Euler(new Vector3(0, Camera.transform.eulerAngles.y, 0)) * Vector3.right;
        public Vector3 Left => -Right;
        public Vector3 Back => -Front;

        /// <summary>
        /// Swapped for pranks
        /// </summary>
        public Vector2 Look { get; private set; } = Vector2.zero;

        protected override void Build(GameObject obj)
        {
            var newObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.GetComponent<MeshFilter>().mesh = newObj.GetComponent<MeshFilter>().mesh;
            obj.GetComponent<Renderer>().material = newObj.GetComponent<Renderer>().material;
            obj.transform.localScale = new Vector3(0.75f, 1.8f, 0.75f);

            GameObject.Destroy(newObj);
        }

        protected override void OnStart()
        {
            GameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            Gravity = 24f;
            //Gravity = 0f;

            Camera.tag = "MainCamera";
            Camera.name = "Camera";
            Camera.transform.SetParent(GameObject.transform);
            Camera.transform.localPosition = new Vector3(0, 0.35f, 0);
            Camera.AddComponent<Camera>();
        }

        private int _updateTimeCount = 0;
        private bool _updateTime => _updateTimeCount >= 1f / Time.deltaTime;

        private Vector3 _lastMouse = new Vector3(-1, -1, -1);

        private float _sensitivity = 3f;

        protected override void OnUpdate()
        {
            if (!GameSystem.Paused)
            {
                Vector2 mouseY = new Vector2(-Input.GetAxis("Mouse Y") * _sensitivity, 0);
                Vector2 mouseX = new Vector2(0, Input.GetAxis("Mouse X") * _sensitivity);

                Look += mouseX;

                Vector2 checkVert = Look + mouseY;

                if (checkVert.x <= 90 && checkVert.x >= -90)
                    Look += mouseY;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;

                Camera.transform.eulerAngles = Look;

                // block placing/breaking
                if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out RaycastHit hit, 5))
                {
                    var normal = hit.normal;
                    var keys = KeyboardHelper.GetPressKeys(KeyCode.Mouse0, KeyCode.Mouse1);

                    var pos = hit.point - (normal * 0.01f);
                    var block = GameSystem.WorldData.GetBlock(pos);

                    if (block != null && block.Type != VoxelType.VOID)
                    {
                        var box = block.GetBox();

                        GameSystem.DrawBlockLines(block.Location);

                        if (keys.Contains(KeyCode.Mouse1))
                        {
                            GameSystem.WorldData.SetBlock(block.Location, VoxelType.VOID);
                            var id = GameSystem.WorldData.GetChunk(block.Location).Id;
                            GameSystem.RegenChunk(id);
                        }
                    }
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            

            _updateTimeCount++;
            if (_updateTime)
            {

                _updateTimeCount = 0;
            }
        }

        private float _movementSpeed = 30f;
        private float _jumpPower = 13f;

        private Vector3[] _normals = new Vector3[0];
        private Location? _lastLocation = null;

        protected override (Vector3 position, Vector3 velocity) CalculateMovement()
        {
            var newVelocity = Velocity;

            var keys = new List<KeyCode>();
            keys.AddRange(KeyboardHelper.GetKeys(KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D));
            keys.AddRange(KeyboardHelper.GetPressKeys(KeyCode.Space, KeyCode.Escape));
            keys.AddRange(KeyboardHelper.ProcessKeyQueue());

            IsGrounded = _normals.Contains(Vector3.up);

            if (keys.Contains(KeyCode.W))
                newVelocity += Vector3.ClampMagnitude(Vector3.Scale(Front * _movementSpeed, new Vector3(1, 0, 1)) * Time.deltaTime, _movementSpeed);
            if (keys.Contains(KeyCode.A))
                newVelocity += Vector3.ClampMagnitude(Vector3.Scale(Left * _movementSpeed, new Vector3(1, 0, 1)) * Time.deltaTime, _movementSpeed);
            if (keys.Contains(KeyCode.S))
                newVelocity += Vector3.ClampMagnitude(Vector3.Scale(Back * _movementSpeed, new Vector3(1, 0, 1)) * Time.deltaTime, _movementSpeed);
            if (keys.Contains(KeyCode.D))
                newVelocity += Vector3.ClampMagnitude(Vector3.Scale(Right * _movementSpeed, new Vector3(1, 0, 1)) * Time.deltaTime, _movementSpeed);
            if (keys.Contains(KeyCode.Space))
            {
                if (IsGrounded)
                    newVelocity += new Vector3(0, _jumpPower, 0);
                //else
                //    KeyboardHelper.AddToQueue(KeyCode.Space, 1);
            }
            if (keys.Contains(KeyCode.Escape))
                GameSystem.Paused = !GameSystem.Paused;

            (newVelocity, _normals) = PhysicsHelper.CollideVelocity(Renderer.bounds, Position, newVelocity, 0.0001f);

            Vector3 newPosition = Position + newVelocity * Time.deltaTime;
            var newLocation = Location.ClampVector(newPosition);

            if (newLocation != _lastLocation)
            {
                if (_lastLocation != null)
                {
                    // Player has moved to a new block
                    OnPlayerMove?.Invoke(this, new PlayerMoveEventArgs(_lastLocation.Value, Location));
                }
                _lastLocation = newLocation;
            }

            return (newPosition, newVelocity);
        }

        protected override void OnTriggerEnter(Collider collider)
        {

            Console.WriteLine();
        }

        protected override void OnCollisionExit(Collision collision)
        {

        }

        public EventHandler<PlayerMoveEventArgs> OnPlayerMove;
    }

    public class PlayerMoveEventArgs : EventArgs
    {
        public Location Last;
        public Location Current;

        public PlayerMoveEventArgs(Location last, Location current)
        {
            Current = current;
            Last = last;
        }
    }
}
