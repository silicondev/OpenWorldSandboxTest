using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.Systems.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.World.Objects
{
    public class GameCamera : InGameObject
    {

        protected override void Build(GameObject obj)
        {
            obj.tag = "MainCamera";
            obj.AddComponent<Camera>();
        }

        protected override (Vector3 position, Vector3 velocity) CalculateMovement() => (Position, Velocity);

        protected override void OnStart()
        {

        }

        protected override void OnUpdate()
        {

        }

        protected override void OnObjectDispose()
        {

        }
    }
}
