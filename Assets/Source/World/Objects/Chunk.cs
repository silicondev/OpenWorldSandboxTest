using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.Systems.Abstracts;
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

        public override void Build(GameObject obj)
        {
            //obj.SetMesh(GenerateMesh());
            obj.GetComponent<MeshFilter>().mesh = GenerateMesh();
            obj.GetComponent<Renderer>().material.SetTexture("_MainTex", TextureHelper.LoadFromImage(@"Assets\Textures\tile.png"));
            obj.transform.position = (Location * GameSystem.ChunkSize).ToVector3();
        }

        public void Regenerate()
        {
            GameObject.SetMesh(GenerateMesh());
        }

        private Mesh GenerateMesh()
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uv = new List<Vector2>();
            var worldLocation = Location * GameSystem.ChunkSize;

            var tasks = new List<Task<(Vector3[], int[], Vector2[])>>();
            for (int z = 0; z < GameSystem.ChunkSize; z++)
            {
                for (int x = 0; x < GameSystem.ChunkSize; x++)
                {
                    int wz = z + worldLocation.Z;
                    int wx = x + worldLocation.X;

                    //tasks.Add(Task.Factory.StartNew(() => GetFaces(x, z, wx, wz)));

                    (Vector3[] newVertices, int[] newTriangles, Vector2[] newUV) = GetFaces(x, z, wx, wz);
                    int c = vertices.Count();

                    vertices.AddRange(newVertices);
                    uv.AddRange(newUV);

                    foreach (int triangle in newTriangles)
                        triangles.Add(triangle + c);
                }
            }

            //Task.WaitAll(tasks.ToArray());

            //foreach (var task in tasks)
            //{
            //    var result = task.Result;

            //    int c = vertices.Count();

            //    vertices.AddRange(result.Item1);
            //    uv.AddRange(result.Item3);

            //    foreach (int triangle in result.Item2)
            //        triangles.Add(triangle + c);
            //}

            var mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uv.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        private (Vector3[], int[], Vector2[]) GetFaces(int x, int z, int wx, int wz)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uv = new List<Vector2>();

            try
            {
                for (int y = 0; y < GameSystem.WorldHeight; y++)
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

                    var prefab = VoxelPrefab.Prefabs[voxel.Type];

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
                        int c = vertices.Count;
                        vertices.Add(new Vector3(x + 0, y - 1, z + 0));
                        vertices.Add(new Vector3(x + 1, y - 1, z + 0));
                        vertices.Add(new Vector3(x + 0, y - 1, z + 1));

                        vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                        vertices.Add(new Vector3(x + 0, y - 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y - 1, z + 0));

                        uv.AddRange(prefab.GetVoxelUV(VoxelFace.BOTTOM));

                        triangles.Add(c);
                        triangles.Add(c + 1);
                        triangles.Add(c + 2);
                        triangles.Add(c + 3);
                        triangles.Add(c + 4);
                        triangles.Add(c + 5);
                    }

                    if (voxelU == null || voxelU.Type == VoxelType.VOID)
                    {
                        // UP FACE
                        int c = vertices.Count;
                        vertices.Add(new Vector3(x + 0, y + 0, z + 1));
                        vertices.Add(new Vector3(x + 1, y + 0, z + 1));
                        vertices.Add(new Vector3(x + 0, y + 0, z + 0));

                        vertices.Add(new Vector3(x + 1, y + 0, z + 0));
                        vertices.Add(new Vector3(x + 0, y + 0, z + 0));
                        vertices.Add(new Vector3(x + 1, y + 0, z + 1));

                        uv.AddRange(prefab.GetVoxelUV(VoxelFace.TOP));

                        triangles.Add(c);
                        triangles.Add(c + 1);
                        triangles.Add(c + 2);
                        triangles.Add(c + 3);
                        triangles.Add(c + 4);
                        triangles.Add(c + 5);
                    }

                    if (voxelL == null || voxelL.Type == VoxelType.VOID)
                    {
                        // LEFT FACE
                        int c = vertices.Count;
                        vertices.Add(new Vector3(x + 0, y + 0, z + 1));
                        vertices.Add(new Vector3(x + 0, y + 0, z + 0));
                        vertices.Add(new Vector3(x + 0, y - 1, z + 1));

                        vertices.Add(new Vector3(x + 0, y - 1, z + 0));
                        vertices.Add(new Vector3(x + 0, y - 1, z + 1));
                        vertices.Add(new Vector3(x + 0, y + 0, z + 0));

                        uv.AddRange(prefab.GetVoxelUV(VoxelFace.LEFT));

                        triangles.Add(c);
                        triangles.Add(c + 1);
                        triangles.Add(c + 2);
                        triangles.Add(c + 3);
                        triangles.Add(c + 4);
                        triangles.Add(c + 5);
                    }

                    if (voxelR == null || voxelR.Type == VoxelType.VOID)
                    {
                        // RIGHT FACE
                        int c = vertices.Count;
                        vertices.Add(new Vector3(x + 1, y + 0, z + 0));
                        vertices.Add(new Vector3(x + 1, y + 0, z + 1));
                        vertices.Add(new Vector3(x + 1, y - 1, z + 0));

                        vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y - 1, z + 0));
                        vertices.Add(new Vector3(x + 1, y + 0, z + 1));

                        uv.AddRange(prefab.GetVoxelUV(VoxelFace.RIGHT));

                        triangles.Add(c);
                        triangles.Add(c + 1);
                        triangles.Add(c + 2);
                        triangles.Add(c + 3);
                        triangles.Add(c + 4);
                        triangles.Add(c + 5);
                    }

                    if (voxelB == null || voxelB.Type == VoxelType.VOID)
                    {
                        // BACK FACE
                        int c = vertices.Count;
                        vertices.Add(new Vector3(x + 0, y + 0, z + 0));
                        vertices.Add(new Vector3(x + 1, y + 0, z + 0));
                        vertices.Add(new Vector3(x + 0, y - 1, z + 0));

                        vertices.Add(new Vector3(x + 1, y - 1, z + 0));
                        vertices.Add(new Vector3(x + 0, y - 1, z + 0));
                        vertices.Add(new Vector3(x + 1, y + 0, z + 0));

                        uv.AddRange(prefab.GetVoxelUV(VoxelFace.BACK));

                        triangles.Add(c);
                        triangles.Add(c + 1);
                        triangles.Add(c + 2);
                        triangles.Add(c + 3);
                        triangles.Add(c + 4);
                        triangles.Add(c + 5);
                    }

                    if (voxelF == null || voxelF.Type == VoxelType.VOID)
                    {
                        // FRONT FACE
                        int c = vertices.Count;
                        vertices.Add(new Vector3(x + 1, y + 0, z + 1));
                        vertices.Add(new Vector3(x + 0, y + 0, z + 1));
                        vertices.Add(new Vector3(x + 1, y - 1, z + 1));

                        vertices.Add(new Vector3(x + 0, y - 1, z + 1));
                        vertices.Add(new Vector3(x + 1, y - 1, z + 1));
                        vertices.Add(new Vector3(x + 0, y + 0, z + 1));

                        uv.AddRange(prefab.GetVoxelUV(VoxelFace.FRONT));

                        triangles.Add(c);
                        triangles.Add(c + 1);
                        triangles.Add(c + 2);
                        triangles.Add(c + 3);
                        triangles.Add(c + 4);
                        triangles.Add(c + 5);
                    }

                    Console.WriteLine($"Processed {wx},{y},{wz} with {vertices.Count} so far this pillar");
                }

                return (vertices.ToArray(), triangles.ToArray(), uv.ToArray());
            }
            catch (Exception what)
            {
                Debug.LogError(what);
            }

            return (new Vector3[0], new int[0], new Vector2[0]);
        }
    }
}
