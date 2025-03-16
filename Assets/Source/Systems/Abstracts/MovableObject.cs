using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems.Abstracts
{
    public abstract class MovableObject : InGameObject
    {

        protected override void Build(GameObject obj) => Task.Factory.StartNew(() =>
        {
            obj.AddComponent<Rigidbody>();
            obj.GetComponent<Rigidbody>().useGravity = true;
            //obj.GetComponent<Rigidbody>().angularDrag = 50;

            BuildMovable(obj);
        });

        protected abstract void BuildMovable(GameObject obj);
    }
}
