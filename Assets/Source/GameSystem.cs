using Assets.Source.Models;
using Assets.Source.Systems;
using Assets.Source.Systems.Abstracts;
using Assets.Source.Systems.WorldGeneration;
using Assets.Source.World.Objects;
using Assets.Source.World.Prefabs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameSystem : MonoBehaviour
{
    public static InGameObjectCollection LoadedObjects = new();
    public static WorldData WorldData;

    public static GenerationSettings GenerationSettings { get; private set; }
    public static bool Paused { get; set; } = false;
    public static int RenderDistance { get; set; } = 3;

    public static Dictionary<VoxelType, VoxelPrefab> VoxelPrefabs = new()
    {
        {
            VoxelType.GRASS,
            new VoxelPrefab()
            {
                TopTexPos = new Vector2(2, 0),
                BottomTexPos = new Vector2(1, 0),
                FrontTexPos = new Vector2(0, 0),
                RightTexPos = new Vector2(0, 0),
                BackTexPos = new Vector2(0, 0),
                LeftTexPos = new Vector2(0, 0)
            }
        },
        {
            VoxelType.DIRT,
            new VoxelPrefab()
            {
                TopTexPos = new Vector2(1, 0),
                BottomTexPos = new Vector2(1, 0),
                FrontTexPos = new Vector2(1, 0),
                RightTexPos = new Vector2(1, 0),
                BackTexPos = new Vector2(1, 0),
                LeftTexPos = new Vector2(1, 0)
            }
        },
        {
            VoxelType.STONE,
            new VoxelPrefab()
            {
                TopTexPos = new Vector2(3, 0),
                BottomTexPos = new Vector2(3, 0),
                FrontTexPos = new Vector2(3, 0),
                RightTexPos = new Vector2(3, 0),
                BackTexPos = new Vector2(3, 0),
                LeftTexPos = new Vector2(3, 0)
            }
        },
    };

    private static Texture2D _standardTexture;
    public static Texture2D StandardTexture
    {
        get => _standardTexture;
        set
        {
            StandardTextureWidth = value.width;
            StandardTextureHeight = value.height;
            _standardTexture = value;
        }
    }
    public static int StandardTextureWidth { get; private set; }
    public static int StandardTextureHeight { get; private set; }

    public static Location[] GetLoadedChunks()
    {
        var chunksObjs = LoadedObjects.Where(x => x.Name.StartsWith("chunk_"));
        var chunks = chunksObjs.Cast<Chunk>().ToArray();
        return chunks.Select(x => x.Id).ToArray();
    }

    private Location? _lastChunkId = null;

    // Start is called before the first frame update
    async void Start()
    {
        Texture.allowThreadedTextureCreation = true;
        StandardTexture = TextureHelper.LoadFromImage(@"Assets\Textures\tile.png");

        transform.localPosition = new Vector3(0, 0, 0);

        GenerationSettings = new GenerationSettings(128, 16, Random.Range(0, 10000), 1024, 1024);

        // INITIALISE WORLD
        var generator = new Generator(GenerationSettings);

        WorldData = new WorldData(generator);
        await WorldData.Generate(new Location(0, 0, 0), RenderDistance + 2);
        RegenChunks(new Location(0, 0, 0));

        // INITIALISE PLAYER

        var player = new Player();
        AddChild(player);
        player.Name = "Player";
        player.Position = new Vector3(0, GenerationSettings.WorldHeight + 1, 0);

        player.OnPlayerMove += async (object sender, PlayerMoveEventArgs e) =>
        {
            Player movedPlayer = (Player)sender;
            var newChunkId = WorldData.GetChunk(e.Current).Id;
            if (_lastChunkId != newChunkId)
            {
                await WorldData.Generate(newChunkId, RenderDistance + 2);
                RegenChunks(newChunkId);
                _lastChunkId = newChunkId;
            }
        };

        LoadedObjects.Add(player);
    }

    private void RegenChunks(Location id)
    {
        (RealRange xRange, RealRange yRange) = WorldData.GetChunkLoadDistance(id, RenderDistance);

        var newNames = new List<string>();
        string format = "chunk_X,Z";
        var currentChunkNames = LoadedObjects.Where(x => x.Name.StartsWith("chunk_")).Select(x => x.Name).ToList();
        for (int cz = yRange.Start; cz <= yRange.End; cz++)
        {
            for (int cx = xRange.Start; cx <= xRange.End; cx++)
            {
                var location = new Location(cx, 0, cz);
                string name = location.ToString(format);
                if (currentChunkNames.Contains(name))
                {
                    newNames.Add(name);
                }
                else
                {
                    var chunk = new Chunk(location);
                    AddChild(chunk);
                    chunk.Name = name;
                    LoadedObjects.Add(chunk);
                    newNames.Add(name);
                }
            }
        }

        currentChunkNames.RemoveAll(x => newNames.Contains(x));
        foreach (var chunk in currentChunkNames)
        {
            LoadedObjects.Remove(chunk);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        LoadedObjects.Clear();
    }

    public void AddChild(InGameObject obj) =>
        obj.SetParent(transform);
}
