using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems.Interfaces
{
    internal interface IGameObject
    {
        GameObject Generate(Transform parent = null);
    }
}
