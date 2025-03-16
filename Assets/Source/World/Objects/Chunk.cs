using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.Systems.Abstracts;
using Assets.Source.Systems.WorldGeneration;
using Assets.Source.World.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.World.Objects
{
    public class Chunk : InGameObject
    {
        public Location Location { get; }

        public Chunk(Location location)
        {
            Location = location;
        }

        protected override async void Build(GameObject obj)
        {
            (Vector3[] vertices, int[] triangles, Vector2[] uv) = await GetMeshData();

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();

            if (obj == null || obj.gameObject == null || obj.activeSelf == false)
                return;

            obj.GetComponent<MeshFilter>().mesh = mesh;
            obj.GetComponent<Renderer>().material.SetTexture("_MainTex", GameSystem.StandardTexture);
            obj.GetComponent<MeshCollider>().sharedMesh = mesh;
            obj.transform.position = (Location * GameSystem.GenerationSettings.ChunkSize).ToVector3() + new Vector3(-1, 0, -1);
        }

        public async Task Regenerate()
        {
            (Vector3[] vertices, int[] triangles, Vector2[] uv) = await GetMeshData();

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();
            GameObject.GetComponent<MeshFilter>().mesh = mesh;
            GameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        private Task<(Vector3[] vertices, int[] triangles, Vector2[] uv)> GetMeshData() => Task.Factory.StartNew(() =>
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uv = new List<Vector2>();
            var worldLocation = Location * GameSystem.GenerationSettings.ChunkSize;

            for (int z = 0; z < GameSystem.GenerationSettings.ChunkSize; z++)
            {
                for (int x = 0; x < GameSystem.GenerationSettings.ChunkSize; x++)
                {
                    int wz = z + worldLocation.Z;
                    int wx = x + worldLocation.X;

                    for (int y = 0; y < GameSystem.GenerationSettings.WorldHeight; y++)
                    {
                        var location = new Location(wx, y, wz);

                        var voxel = GameSystem.WorldData[location];
                        var voxelU = GameSystem.WorldData[location.AddY(1)];
                        var voxelD = GameSystem.WorldData[location.AddY(-1)];
                        var voxelR = GameSystem.WorldData[location.AddX(1)];
                        var voxelL = GameSystem.WorldData[location.AddX(-1)];
                        var voxelF = GameSystem.WorldData[location.AddZ(1)];
                        var voxelB = GameSystem.WorldData[location.AddZ(-1)];

                        if (voxel == null || voxel.Type == VoxelType.VOID)
                            continue;

                        var prefab = GameSystem.VoxelPrefabs[voxel.Type];

                        /*

                        1 --- 0
                        | \   |
                        |   \ |
                        x --- 2

                        5 --- x
                        | \   |
                        |   \ |
                        3 --- 4


                        */

                        if (voxelD == null || voxelD.Type == VoxelType.VOID)
                        {
                            // DOWN FACE
                            vertices.Add(new Vector3(x + 0, y - 1, z + 0));
                            vertices.Add(new Vector3(x + 1, y - 1, z + 0));
                            vertices.Add(new Vector3(x + 0, y - 1, z + 1));

                            vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                            vertices.Add(new Vector3(x + 0, y - 1, z + 1));
                            vertices.Add(new Vector3(x + 1, y - 1, z + 0));

                            uv.AddRange(prefab.GetVoxelUV(VoxelFace.BOTTOM, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                        }

                        if (voxelU == null || voxelU.Type == VoxelType.VOID)
                        {
                            // UP FACE
                            vertices.Add(new Vector3(x + 0, y + 0, z + 1));
                            vertices.Add(new Vector3(x + 1, y + 0, z + 1));
                            vertices.Add(new Vector3(x + 0, y + 0, z + 0));

                            vertices.Add(new Vector3(x + 1, y + 0, z + 0));
                            vertices.Add(new Vector3(x + 0, y + 0, z + 0));
                            vertices.Add(new Vector3(x + 1, y + 0, z + 1));

                            uv.AddRange(prefab.GetVoxelUV(VoxelFace.TOP, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                        }

                        if (voxelL == null || voxelL.Type == VoxelType.VOID)
                        {
                            // LEFT FACE
                            vertices.Add(new Vector3(x + 0, y + 0, z + 1));
                            vertices.Add(new Vector3(x + 0, y + 0, z + 0));
                            vertices.Add(new Vector3(x + 0, y - 1, z + 1));

                            vertices.Add(new Vector3(x + 0, y - 1, z + 0));
                            vertices.Add(new Vector3(x + 0, y - 1, z + 1));
                            vertices.Add(new Vector3(x + 0, y + 0, z + 0));

                            uv.AddRange(prefab.GetVoxelUV(VoxelFace.LEFT, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                        }

                        if (voxelR == null || voxelR.Type == VoxelType.VOID)
                        {
                            // RIGHT FACE
                            vertices.Add(new Vector3(x + 1, y + 0, z + 0));
                            vertices.Add(new Vector3(x + 1, y + 0, z + 1));
                            vertices.Add(new Vector3(x + 1, y - 1, z + 0));

                            vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                            vertices.Add(new Vector3(x + 1, y - 1, z + 0));
                            vertices.Add(new Vector3(x + 1, y + 0, z + 1));

                            uv.AddRange(prefab.GetVoxelUV(VoxelFace.RIGHT, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                        }

                        if (voxelB == null || voxelB.Type == VoxelType.VOID)
                        {
                            // BACK FACE
                            vertices.Add(new Vector3(x + 0, y + 0, z + 0));
                            vertices.Add(new Vector3(x + 1, y + 0, z + 0));
                            vertices.Add(new Vector3(x + 0, y - 1, z + 0));

                            vertices.Add(new Vector3(x + 1, y - 1, z + 0));
                            vertices.Add(new Vector3(x + 0, y - 1, z + 0));
                            vertices.Add(new Vector3(x + 1, y + 0, z + 0));

                            uv.AddRange(prefab.GetVoxelUV(VoxelFace.BACK, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                        }

                        if (voxelF == null || voxelF.Type == VoxelType.VOID)
                        {
                            // FRONT FACE
                            vertices.Add(new Vector3(x + 1, y + 0, z + 1));
                            vertices.Add(new Vector3(x + 0, y + 0, z + 1));
                            vertices.Add(new Vector3(x + 1, y - 1, z + 1));

                            vertices.Add(new Vector3(x + 0, y - 1, z + 1));
                            vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                            vertices.Add(new Vector3(x + 0, y + 0, z + 1));

                            uv.AddRange(prefab.GetVoxelUV(VoxelFace.FRONT, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                        }
                    }
                }
            }

            for (int i = 0; i < vertices.Count(); i++)
                triangles.Add(i);

            return (vertices.ToArray(), triangles.ToArray(), uv.ToArray());
        });
        
        protected override void OnStart()
        {
            //GameObject.layer = LayerMask.NameToLayer("Environment");
        }

        protected override void OnUpdate()
        {

        }

        protected override (Vector3 position, Vector3 velocity) CalculateMovement() =>
            (Position, Vector3.zero);
    }
}
