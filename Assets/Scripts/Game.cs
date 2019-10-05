using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace zs.Assets.Scripts
{
    public class Game : MonoBehaviour
    {
        #region Serializable Fields

        [SerializeField]
        private Player _playerPrefab = null;

        [SerializeField]
        private Sheep _sheepPrefab = null;

        [SerializeField]
        private Tilemap _baseTilemap = null;

        [SerializeField]
        private Tilemap _wallTilemap = null;

        [SerializeField]
        private TileBase[] _outsideTiles = null;

        [SerializeField]
        private TileBase[] _grassTiles = null;

        [SerializeField]
        private TileBase _fenceHorTile = null;

        [SerializeField]
        private TileBase _fenceVerTile = null;

        [SerializeField]
        private int _initialSeed = 0;

        [SerializeField]
        private int _initialLevel = 0;

        [SerializeField]
        private Difficulty _initialDifficulty = Difficulty.Hard;

        [SerializeField]
        private int _borderSize = 10;

        #endregion Serializable Fields

        #region Private Vars

        private List<Player> _players = new List<Player>();

        #endregion Private Vars

        #region Public Vars

        public static Game Instance { get; private set; }

        #endregion Public Vars

        #region Public Methods

        public void Generate()
        {
            int initialSeed = _initialSeed;

            if (initialSeed == 0)
            {
                initialSeed = Guid.NewGuid().GetHashCode();
            }

            Generate(initialSeed, _initialLevel, _initialDifficulty);
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            Debug.Assert(_playerPrefab);
            Debug.Assert(_sheepPrefab);
            Debug.Assert(_baseTilemap);
            Debug.Assert(_wallTilemap);

            Debug.Assert(_outsideTiles != null && _outsideTiles.Length > 0);
            Debug.Assert(_grassTiles != null && _grassTiles.Length > 0);

            Debug.Assert(_fenceHorTile);
            Debug.Assert(_fenceVerTile);

            Instance = this;
        }

        void Start()
        {
            Generate(_initialSeed, _initialLevel, _initialDifficulty);
        }
	
        void Update()
        {
        }

        #endregion MonoBehaviour

        #region Private Methods

        private void Generate(int seed, int level, Difficulty difficulty)
        {
            Debug.Log($"Generating: Seed [{seed}], Level [{level}], Difficulty [{difficulty}]");


            Random.InitState(seed);


            // Reset GameObjects
            foreach (Player player in _players)
            {
                DestroyImmediate(player.gameObject);
            }

            _players.Clear();

            // Reset Tilemaps
            _baseTilemap.ClearAllTiles();
            _wallTilemap.ClearAllTiles();


            // Initialize Level Params
            int width = 10;
            int height = 10;

            if (difficulty == Difficulty.Easy)
            {
                width += level;
                height += level;
            }
            else if (difficulty == Difficulty.Normal)
            {
                width += 2 * level;
                height += 2 * level;
            }
            else if (difficulty == Difficulty.Hard)
            {
                width += 3 * level;
                height += 3 * level;
            }


            Map map = new Map(width, height);


            // Create Initial Patch of Grass
            {
                int initialWidth = Random.Range(10, 10 + level + 1);
                int initialHeight = Random.Range(10, 10 + level + 1);
                int initialX = (width - initialWidth) / 2;
                int initialY = (height - initialHeight) / 2;

                PlaceTileSquare(map, initialX, initialY, initialWidth, initialHeight, TileType.Grass);
            }


            // Create Additional Patches of Grass on the outer side of existing Grass Patches
            {
                int patchCount = level + 1;
                int placedPatches = 0;
                int iteration = 0;

                while (placedPatches < patchCount && iteration < 1000)
                {
                    iteration += 1;

                    int approachDirection = Random.Range(0, 4);

                    bool apply = false;
                    int x = 0;
                    int y = 0;

                    if (approachDirection == 0)
                    {
                        // From South
                        x = Random.Range(0, map.Width);

                        for (y = 0; y < map.Height; ++y)
                        {
                            if (map.IsTileSurroundedBy(x, y, TileType.Grass))
                            {
                                apply = true;
                                break;
                            }
                        }
                    }
                    else if (approachDirection == 1)
                    {
                        // From East
                        y = Random.Range(0, map.Height);

                        for (x = map.Width - 1; x >= 0; --x)
                        {
                            if (map.IsTileSurroundedBy(x, y, TileType.Grass))
                            {
                                apply = true;
                                break;
                            }
                        }
                    }
                    else if (approachDirection == 2)
                    {
                        // From North
                        x = Random.Range(0, map.Width);

                        for (y = map.Height - 1; y >= 0; --y)
                        {
                            if (map.IsTileSurroundedBy(x, y, TileType.Grass))
                            {
                                apply = true;
                                break;
                            }
                        }
                    }
                    else if (approachDirection == 3)
                    {
                        // From West
                        y = Random.Range(0, map.Height);

                        for (x = 0; x < map.Width; ++x)
                        {
                            if (map.IsTileSurroundedBy(x, y, TileType.Grass))
                            {
                                apply = true;
                                break;
                            }
                        }
                    }


                    if (apply)
                    {
                        int patchWidth = Random.Range(10 - level, 10 + level + 1);
                        int patchHeight = Random.Range(10 - level, 10 + level + 1);

                        if (patchWidth < 3)
                        {
                            patchWidth = 3;
                        }

                        if (patchHeight < 3)
                        {
                            patchHeight = 3;
                        }

                        for (int patchY = y - patchHeight / 2; patchY <= y + patchHeight / 2; ++patchY)
                        {
                            for (int patchX = x - patchWidth / 2; patchX <= x + patchWidth / 2; ++patchX)
                            {
                                if (!map.IsValid(patchX, patchY))
                                {
                                    continue;
                                }

                                map.SetTile(patchX, patchY, TileType.Grass);
                            }
                        }

                        placedPatches += 1;

                        Debug.Log("Placed Patch of Grass");
                    }
                }
            }


            // Add outside border
            map.AddBorder(_borderSize, TileType.Outside);


            // Add fences
            {
                for (int y = 0; y < map.Height; ++y)
                {
                    for (int x = 0; x < map.Width; ++x)
                    {
                        if (map.GetTile(x, y) == TileType.Outside)
                        {
                            if (map.IsValid(x, y - 1) &&
                                map.GetTile(x, y - 1) == TileType.Grass)
                            {
                                // South Grass
                                map.SetTile(x, y, TileType.Fence);
                            }
                            else if (map.IsValid(x, y + 1) &&
                                     map.GetTile(x, y + 1) == TileType.Grass)
                            {
                                // North Grass
                                map.SetTile(x, y, TileType.Fence);
                            }
                            else if (map.IsValid(x + 1, y) &&
                                map.GetTile(x + 1, y) == TileType.Grass)
                            {
                                // East Grass
                                map.SetTile(x, y, TileType.Fence);
                            }
                            else if (map.IsValid(x - 1, y) &&
                                     map.GetTile(x - 1, y) == TileType.Grass)
                            {
                                // West Grass
                                map.SetTile(x, y, TileType.Fence);
                            }
                        }
                    }
                }
            }


            // Add Player
            {
                while (true)
                {
                    int x = Random.Range(0, map.Width);
                    int y = Random.Range(0, map.Height);

                    if (map.IsTileSurroundedBy(x, y, TileType.Grass))
                    {
                        map.SetTile(x, y, TileType.StartTile);
                        break;
                    }
                }
            }

            // Generate Real TileMap
            {
                for (int y = 0; y < map.Height; ++y)
                {
                    for (int x = 0; x < map.Width; ++x)
                    {
                        switch (map.GetTile(x, y))
                        {
                            case TileType.Outside:
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _outsideTiles[Random.Range(0, _outsideTiles.Length)]);
                                break;

                            case TileType.Grass:
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                break;

                            case TileType.Fence:
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);

                                if (map.GetTile(x, y - 1) == TileType.Grass)
                                {
                                    // South Grass
                                    _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceHorTile);
                                }
                                else if (map.GetTile(x, y + 1) == TileType.Grass)
                                {
                                    // North Grass
                                    _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceHorTile);
                                }
                                else if (map.GetTile(x + 1, y) == TileType.Grass)
                                {
                                    // East Grass
                                    _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceVerTile);
                                }
                                else if (map.GetTile(x - 1, y) == TileType.Grass)
                                {
                                    // West Grass
                                    _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceVerTile);
                                }
                                break;

                            case TileType.StartTile:
                                map.SetTile(x, y, TileType.Grass);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);

                                Player player = Instantiate(_playerPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                                _players.Add(player);
                                FollowCamera.Instance.FollowObject = player.transform;
                                break;

                            default:
                                Debug.Assert(false, "Unknown Tile Type");
                                continue;
                        }

                    }
                }
            }
        }

        private void PlaceTileSquare(Map map, int startX, int startY, int width, int height, TileType tileType)
        {
            for (int y = startY; y < startY + height; ++y)
            {
                for (int x = startX; x < startX + width; ++x)
                {
                    map.SetTile(x, y, tileType);
                }
            }
        }

        #endregion Private Methods
    }
}
