using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems
{
    public static class MeshHelper
    {
        public static Mesh Combine(IEnumerable<MeshFilter> meshes)
        {
            CombineInstance[] combine = new CombineInstance[meshes.Count()];

            for (int i = 0; i < meshes.Count(); i++)
            {
                combine[i].mesh = meshes.ElementAt(i).sharedMesh;
                combine[i].transform = meshes.ElementAt(i).transform.localToWorldMatrix;
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine);
            return mesh;
        }

        public static void SetMesh(this GameObject obj, Mesh mesh)
        {
            var filter = obj.GetComponent<MeshFilter>();
            if (filter == null)
                filter = obj.AddComponent<MeshFilter>();
            filter.mesh = mesh;
        }
    }
}
