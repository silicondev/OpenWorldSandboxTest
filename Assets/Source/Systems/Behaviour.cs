using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems
{
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

        protected void OnCollisionEnter(Collision collision)
        {
            OnCollisionOn?.Invoke(this, new CollisionEventArgs(collision));
        }

        protected void OnCollisionExit(Collision collision)
        {
            OnCollisionOff?.Invoke(this, new CollisionEventArgs(collision));
        }

        protected void OnTriggerEnter(Collider other)
        {
            OnTriggerOn?.Invoke(this, new ColliderEventArgs(other));
        }

        protected void OnTriggerExit(Collider other)
        {
            OnTriggerOff?.Invoke(this, new ColliderEventArgs(other));
        }

        public EventHandler<EventArgs> OnStart;
        public EventHandler<EventArgs> OnUpdate;
        public EventHandler<CollisionEventArgs> OnCollisionOn;
        public EventHandler<CollisionEventArgs> OnCollisionOff;
        public EventHandler<ColliderEventArgs> OnTriggerOn;
        public EventHandler<ColliderEventArgs> OnTriggerOff;
    }

    public class CollisionEventArgs : ColliderEventArgs
    {
        public Collision Collision { get; }
        public CollisionEventArgs(Collision collision) : base(collision.collider)
        {
            Collision = collision;
        }
    }

    public class ColliderEventArgs : EventArgs
    {
        public Collider Collider { get; }
        public ColliderEventArgs(Collider collider)
        {
            Collider = collider;
        }
    }
}
