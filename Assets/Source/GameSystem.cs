using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.World.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static InGameObjectCollection LoadedObjects = new();
    public static WorldData WorldData { get; private set; }
    public static int WorldSize => 128;
    public static int WorldHeight => 128;
    public static int ChunkSize => 16;
    public static bool Paused { get; set; } = false;

    public static Chunk[] GetChunks()
    {
        var chunksObjs = LoadedObjects.Where(x => x.Name.StartsWith("chunk_"));
        return chunksObjs.Cast<Chunk>().ToArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.localPosition = new Vector3(0, 0, 0);

        // INITIALISE WORLD

        WorldData = new WorldData(WorldSize, WorldHeight, (int worldSize, int worldHeight) =>
        {
            var voxels = new Voxel[worldSize, worldHeight, worldSize];
            for (int x = 0; x < worldSize; x++)
            {
                for (int z = 0; z < worldSize; z++)
                {
                    //int height = 5 + Random.Range(1, 4);
                    //int height = 10;
                    //int height = Mathf.Clamp((int)(Mathf.PerlinNoise(x, z) * (float)WorldHeight), 0, worldHeight);
                    float perlin = Mathf.PerlinNoise((float)x / (float)worldSize, (float)z / (float)worldSize);
                    float real = perlin * (float)WorldHeight;
                    int height = Mathf.Clamp((int)real, 0, worldHeight);


                    for (int y = 0; y < worldHeight; y++)
                    {
                        var location = new Location(x - worldSize / 2, y, z - worldSize / 2);
                        if (y < height - 3)
                            voxels[x,y,z] = new Voxel(location, VoxelType.STONE);
                        else if (y < height)
                            voxels[x,y,z] = new Voxel(location, VoxelType.DIRT);
                        else if (y == height)
                            voxels[x,y,z] = new Voxel(location, VoxelType.GRASS);
                        else
                            voxels[x,y,z] = new Voxel(location, VoxelType.VOID);
                    }
                }
            }
            return voxels;
        });

        WorldData.Generate();

        int worldChunkSize = WorldSize / ChunkSize;

        for (int cz = 0; cz < worldChunkSize; cz++)
        {
            for (int cx = 0; cx < worldChunkSize; cx++)
            {
                var chunk = new Chunk(new Location(cx - worldChunkSize / 2, 0, cz - worldChunkSize / 2));
                chunk.SetParent(transform);
                chunk.Name = $"chunk_{chunk.Name}";
                LoadedObjects.Add(chunk);
            }
        }

        // INITIALISE PLAYER

        var player = new Player();
        player.SetParent(transform);
        player.Name = "Player";
        player.Position = new Vector3(0, WorldHeight + 5, 0);
        LoadedObjects.Add(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        LoadedObjects.Clear();
    }

    public void AddChild(GameObject obj) =>
        obj.transform.SetParent(transform);
}
