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

        public bool IsGrounded { get; private set; }

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

            Build(obj);

            obj.GetComponent<MeshCollider>().sharedMesh = obj.GetComponent<MeshFilter>().mesh;
            obj.GetComponent<MeshCollider>().convex = true;

            obj.name = Guid.NewGuid().ToString();
            if (_parent != null)
                obj.transform.SetParent(_parent);

            obj.AddComponent<Behaviour>();
            obj.GetComponent<Behaviour>().OnStart += (object sender, EventArgs e) => OnStart();
            obj.GetComponent<Behaviour>().OnUpdate += (object sender, EventArgs e) => OnLocalUpdate();
            obj.GetComponent<Behaviour>().OnUpdate += (object sender, EventArgs e) => OnUpdate();
            return obj;
        }

        private Vector3 _previousPosition = new Vector3();

        private void OnLocalUpdate()
        {
            IsGrounded = Position.y == _previousPosition.y;
            _previousPosition = new Vector3(Position.x, Position.y, Position.z);
        }

        protected abstract void Build(GameObject obj);
        protected abstract void OnStart();
        protected abstract void OnUpdate();

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

    public class Behaviour : MonoBehaviour
    {
        protected void Start()
        {
            OnStart?.Invoke(this, null);
        }

        protected void Update()
        {
            OnUpdate?.Invoke(this, null);
        }

        public EventHandler<EventArgs> OnStart;
        public EventHandler<EventArgs> OnUpdate;
    }
}
