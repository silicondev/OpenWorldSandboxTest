using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems.Abstracts
{
    public abstract class InGameObject : IDisposable
    {
        private GameObject _gameObject = null;
        private Transform _parent = null;
        public GameObject GameObject
        {
            get
            {
                if (_gameObject == null)
                    _gameObject = Setup();
                return _gameObject;
            }
        }

        public string Name
        {
            get => GameObject.name;
            set => GameObject.name = value;
        }

        public Vector3 Position
        {
            get => GameObject.transform.localPosition;
            set => GameObject.transform.localPosition = value;
        }

        public Quaternion Rotation
        {
            get => GameObject.transform.rotation;
            set => GameObject.transform.rotation = value;
        }

        public Vector3 Scale
        {
            get => GameObject.transform.localScale;
            set => GameObject.transform.localScale = value;
        }

        public Vector3 Velocity { get; set; }
        public float Drag { get; set; } = 16f;

        public Vector3 Center => 
            Renderer.bounds.center;

        public Mesh Mesh => 
            MeshFilter.mesh;

        public MeshFilter MeshFilter =>
            GameObject.GetComponent<MeshFilter>();

        public Renderer Renderer => 
            GameObject.GetComponent<Renderer>();

        public bool Enabled
        {
            get => GameObject != null && GameObject.activeSelf;
            set => GameObject.SetActive(value);
        }

        public bool IsGrounded { get; protected set; }

        public bool GravityEnabled { get; set; } = false;
        public bool CollisionEnabled { get; set; } = true;

        public InGameObject(Transform parent = null)
        {
            _parent = parent;
        }

        public InGameObject(string name, Transform parent = null) : this(parent)
        {
            Name = name;
        }

        private GameObject Setup()
        {
            var obj = new GameObject();

            obj.AddComponent<MeshRenderer>();
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshCollider>();

            obj.GetComponent<Renderer>().material.color = Color.white;
            obj.GetComponent<MeshCollider>().convex = false;

            Build(obj);

            obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;

            obj.name = Guid.NewGuid().ToString();
            if (_parent != null)
                obj.transform.SetParent(_parent);

            obj.AddComponent<Behaviour>();
            obj.GetComponent<Behaviour>().OnStart += (object sender, EventArgs e) => OnStart();
            obj.GetComponent<Behaviour>().OnUpdate += (object sender, EventArgs e) => OnLocalUpdate();
            obj.GetComponent<Behaviour>().OnCollisionOn += (object sender, CollisionEventArgs e) => OnCollisionEnter(e.Collision);
            obj.GetComponent<Behaviour>().OnCollisionOff += (object sender, CollisionEventArgs e) => OnCollisionExit(e.Collision);
            obj.GetComponent<Behaviour>().OnTriggerOn += (object sender, ColliderEventArgs e) => OnTriggerEnter(e.Collider);
            obj.GetComponent<Behaviour>().OnTriggerOff += (object sender, ColliderEventArgs e) => OnTriggerExit(e.Collider);
            return obj;
        }

        private Vector3 _previousPosition = new Vector3();

        bool keyPress = false;

        private void OnLocalUpdate()
        {
            OnUpdate();

            if (GravityEnabled)
            {
                // add gravity
                Velocity += Vector3.ClampMagnitude(Physics.gravity * Time.fixedDeltaTime, 1);
                //Velocity = new Vector3(-0.33f, -1f, -0.66f);
                //Velocity = new Vector3(0, -0.2f, 0);
            }

            Velocity *= Mathf.Clamp01(1.0f - (Drag * Time.fixedDeltaTime));

            if (Input.GetKey(KeyCode.K))
            {
                if (!keyPress)
                {
                    keyPress = true;
                    Velocity += new Vector3(0, 35, 0);
                }
            }
            else
            {
                keyPress = false;
            }

            (Position, Velocity) = CalculateMovement();

            IsGrounded = Position.y == _previousPosition.y;
            _previousPosition = new Vector3(Position.x, Position.y, Position.z);
        }

        protected abstract void Build(GameObject obj);
        protected abstract void OnStart();
        protected abstract void OnUpdate();
        protected abstract (Vector3 position, Vector3 velocity) CalculateMovement();
        protected virtual void OnCollisionEnter(Collision collision) { }
        protected virtual void OnCollisionExit(Collision collision) { }
        protected virtual void OnTriggerEnter(Collider collider) { }
        protected virtual void OnTriggerExit(Collider collider) { }

        public void SetParent(Transform parent) =>
            GameObject.transform.parent = parent;

        public void SetTexture(Texture2D texture) =>
            Renderer.material.SetTexture("_MainTex", texture);

        public void Dispose()
        {
            string name = Name;
            GameObject.Destroy(GameObject);
            OnDispose.Invoke(this, new InGameObjectEventArgs(name));
        }

        public event EventHandler<InGameObjectEventArgs> OnDispose;
    }

    public class InGameObjectEventArgs
    {
        public string Name { get; }

        public InGameObjectEventArgs(string name)
        {
            Name = name;
        }
    }
}
