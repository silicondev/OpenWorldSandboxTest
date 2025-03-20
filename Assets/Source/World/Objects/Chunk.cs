using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.Systems.Abstracts;
using Assets.Source.Systems.WorldGeneration;
using Assets.Source.World.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Source.World.Objects
{
    public class Chunk : InGameObject
    {
        public Location Id { get; }

        #region Faces
        private readonly Vector3[] _downFace = new Vector3[]
        {
            new Vector3(0, -1, 0), new Vector3(1, -1, 0), new Vector3(0, -1, 1),
            new Vector3(1, -1, 1), new Vector3(0, -1, 1), new Vector3(1, -1, 0)
        };

        private readonly Vector3[] _upFace = new Vector3[]
        {
            new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(0, 0, 0),
            new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 1)
        };

        private readonly Vector3[] _leftFace = new Vector3[]
        {
            new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, -1, 1),
            new Vector3(0, -1, 0), new Vector3(0, -1, 1), new Vector3(0, 0, 0)
        };

        private readonly Vector3[] _rightFace = new Vector3[]
        {
            new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, -1, 0),
            new Vector3(1, -1, 1), new Vector3(1, -1, 0), new Vector3(1, 0, 1)
        };

        private readonly Vector3[] _backFace = new Vector3[]
        {
            new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(0, -1, 0),
            new Vector3(1, -1, 0), new Vector3(0, -1, 0), new Vector3(1, 0, 0)
        };

        private readonly Vector3[] _frontFace = new Vector3[]
        {
            new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(1, -1, 1),
            new Vector3(0, -1, 1), new Vector3(1, -1, 1), new Vector3(0, 0, 1)
        };
        #endregion

        public Chunk(Location location)
        {
            Id = location;
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

            obj.AddComponent<MeshCollider>();
            obj.GetComponent<MeshCollider>().sharedMesh = mesh;
            obj.GetComponent<MeshCollider>().convex = false;

            obj.AddComponent<LineRenderer>();
            obj.GetComponent<LineRenderer>().enabled = true;

            obj.transform.position = (Id * GameSystem.GenerationSettings.ChunkSize).ToVector3() + new Vector3(-1, 0, -1);
        }

        public async void Regenerate()
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

        public void DrawLines(Location blockLocation)
        {
            var renderer = GameObject.GetComponent<LineRenderer>();

            var block = GameSystem.WorldData.GetBlock(blockLocation);
            var box = block.GetBox();
            box.DrawLines(renderer, Color.red, 0.1f);
        }

        private Task<(Vector3[] vertices, int[] triangles, Vector2[] uv)> GetMeshData() => Task.Factory.StartNew(() =>
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uv = new List<Vector2>();
            var worldLocation = Id * GameSystem.GenerationSettings.ChunkSize;
            Location location = new Location(int.MinValue, int.MinValue, int.MinValue);

            var chunk = GameSystem.WorldData.GetChunkFromId(Id);

            if (chunk == null)
                return (new Vector3[0], new int[0], new Vector2[0]);

            try
            {
                for (int z = 0; z < GameSystem.GenerationSettings.ChunkSize; z++)
                {
                    for (int x = 0; x < GameSystem.GenerationSettings.ChunkSize; x++)
                    {
                        int wz = z + worldLocation.Z;
                        int wx = x + worldLocation.X;

                        var xpc = GameSystem.WorldData.GetChunk(worldLocation + new Location(x + 1, 0, z));
                        var xmc = GameSystem.WorldData.GetChunk(worldLocation + new Location(x - 1, 0, z));
                        var zpc = GameSystem.WorldData.GetChunk(worldLocation + new Location(x, 0, z + 1));
                        var zmc = GameSystem.WorldData.GetChunk(worldLocation + new Location(x, 0, z - 1));

                        for (int y = 0; y < GameSystem.GenerationSettings.WorldHeight; y++)
                        {
                            location = new Location(wx, y, wz);
                            var cLoc = new Location(x, y, z);

                            var voxel = chunk.Voxels[x, y, z];
                            var voxelU = chunk.Voxels.GetOrNull(x, y + 1, z);
                            var voxelD = chunk.Voxels.GetOrNull(x, y - 1, z);
                            var voxelR = xpc?.GetBlock(location.AddX(1));
                            var voxelL = xmc?.GetBlock(location.AddX(-1));
                            var voxelF = zpc?.GetBlock(location.AddZ(1));
                            var voxelB = zmc?.GetBlock(location.AddZ(-1));

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

                            if (voxelD != null && voxelD.Type == VoxelType.VOID)
                            {
                                // DOWN FACE
                                vertices.AddRange(cLoc.RangeCombineVector(_downFace));
                                uv.AddRange(prefab.GetVoxelUV(VoxelFace.BOTTOM, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                            }

                            if (voxelU != null && voxelU.Type == VoxelType.VOID)
                            {
                                // UP FACE
                                vertices.AddRange(cLoc.RangeCombineVector(_upFace));
                                uv.AddRange(prefab.GetVoxelUV(VoxelFace.TOP, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                            }

                            if (voxelL != null && voxelL.Type == VoxelType.VOID)
                            {
                                // LEFT FACE
                                vertices.AddRange(cLoc.RangeCombineVector(_leftFace));
                                uv.AddRange(prefab.GetVoxelUV(VoxelFace.LEFT, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                            }

                            if (voxelR != null && voxelR.Type == VoxelType.VOID)
                            {
                                // RIGHT FACE
                                vertices.AddRange(cLoc.RangeCombineVector(_rightFace));
                                uv.AddRange(prefab.GetVoxelUV(VoxelFace.RIGHT, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                            }

                            if (voxelB != null && voxelB.Type == VoxelType.VOID)
                            {
                                // BACK FACE
                                vertices.AddRange(cLoc.RangeCombineVector(_backFace));
                                uv.AddRange(prefab.GetVoxelUV(VoxelFace.BACK, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                            }

                            if (voxelF != null && voxelF.Type == VoxelType.VOID)
                            {
                                // FRONT FACE
                                vertices.AddRange(cLoc.RangeCombineVector(_frontFace));
                                uv.AddRange(prefab.GetVoxelUV(VoxelFace.FRONT, GameSystem.StandardTextureWidth, GameSystem.StandardTextureHeight));
                            }
                        }
                    }
                }

                for (int i = 0; i < vertices.Count(); i++)
                    triangles.Add(i);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error generating faces for block {location} in chunk {Id}:");
                Debug.LogException(e);
            }
            
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

        private void DrawDebugLines(float duration = 0f)
        {
            var chunkData = GameSystem.WorldData.GetChunk(Id);
            var voxels = chunkData.Voxels;
            foreach (var voxel in voxels)
            {
                Color c = Color.black;
                switch (voxel.Type)
                {
                    case VoxelType.VOID:
                        c = Color.white; 
                        break;
                    case VoxelType.GRASS:
                        c = Color.green;
                        break;
                    case VoxelType.DIRT:
                        c = Color.red;
                        break;
                    case VoxelType.STONE:
                        c = Color.grey;
                        break;
                }
                if (voxel.Type == VoxelType.GRASS)
                    voxel.GetBox().DrawDebugLines(c, false, duration);
            }
        }
    }
}
