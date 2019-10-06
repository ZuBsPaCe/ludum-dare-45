using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;
using Debug = UnityEngine.Debug;
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
        private Haystack _haystackPrefab = null;

        [SerializeField]
        private Tilemap _baseTilemap = null;

        [SerializeField]
        private Tilemap _wallTilemap = null;

        [SerializeField]
        private TileBase[] _outsideTiles = null;

        [SerializeField]
        private TileBase[] _grassTiles = null;

        [SerializeField]
        private TileBase[] _rockTiles = null;

        [SerializeField]
        private TileBase _fenceN = null;

        [SerializeField]
        private TileBase _fenceNW = null;

        [SerializeField]
        private TileBase _fenceW = null;

        [SerializeField]
        private TileBase _fenceSW = null;

        [SerializeField]
        private TileBase _fenceS = null;

        [SerializeField]
        private TileBase _fenceSE = null;

        [SerializeField]
        private TileBase _fenceE = null;

        [SerializeField]
        private TileBase _fenceNE = null;

        [SerializeField]
        private Coin _coinPrefab = null;

        [SerializeField]
        private int _borderSize = 10;

        #endregion Serializable Fields

        #region Private Vars

        private List<Player> _players = new List<Player>();
        private List<Sheep> _sheep = new List<Sheep>();
        private List<Haystack> _haystacks = new List<Haystack>();

        private Stopwatch _levelTime = new Stopwatch();
        private bool _levelStarted = false;
        private bool _levelDone = false;

        #endregion Private Vars

        #region Public Vars

        public static Game Instance { get; private set; }

        public Map Map { get; private set; }

        public int KilledSheep { get; private set; }
        public int Coins { get; private set; }

        public bool LevelDone
        {
            get { return _levelDone; }
        }

        public int SecondsLeft
        {
            get
            {
                if (AlivePlayerCount == 0)
                {
                    return 0;
                }


                return Mathf.Max(120 - Mathf.FloorToInt((float) _levelTime.Elapsed.TotalSeconds), 0);
            }
        }

        public int AlivePlayerCount
        {
            get
            {
                int count = 0;

                foreach (Player player in _players)
                {
                    if (player.IsAlive)
                    {
                        count += 1;
                    }
                }

                return count;
            }
        }

        public List<Player> Players
        {
            get { return _players; }
        }

        public void WakeUpSheep(Player player)
        {
            foreach (Sheep sheep in _sheep)
            {
                if (sheep.IsEating)
                {
                    if (Vector3.Distance(player.transform.position, sheep.transform.position) < Sheep.HearingDistance)
                    {
                        sheep.StartRunning(player);
                    }
                }
            }
        }

        #endregion Public Vars

        #region Public Methods

        public void RegisterDeadSheep(Sheep deadSheep, Player fromPlayer)
        {
            Debug.Assert(!deadSheep.IsAlive);

            KilledSheep += 1;

            SoundPlayer.Instance.Play(Sound.SheepHit, deadSheep.AudioSource);

            if (Master.Instance.CurrentDifficulty == Difficulty.Easy)
            {
                WakeUpSheep(fromPlayer);
            }

            int sheepAlive = 0;

            foreach (Sheep sheep in _sheep)
            {
                if (sheep.IsAlive)
                {
                    sheepAlive += 1;
                }
            }

            if (sheepAlive > 0)
            {
                Coin coin = Instantiate(_coinPrefab, deadSheep.transform.position, Quaternion.identity);
                Vector2 coinVec = (coin.transform.position - fromPlayer.transform.position).normalized;

                coin.Rigidbody.AddForce(coinVec * Random.Range(3f, 10f), ForceMode2D.Impulse);
            }

            if (sheepAlive == 0)
            {
                _levelTime.Stop();
                _levelDone = true;
                MainWindow.Instance.RunTimelineLevelWon();
            }
        }

        public void RegisterDeadPlayer(Player deadPlayer)
        {
            Debug.Assert(!deadPlayer.IsAlive);

            SoundPlayer.Instance.Play(Sound.PlayerHit, deadPlayer.AudioSource);

            int playersAlive = 0;

            foreach (Player player in _players)
            {
                if (player.IsAlive)
                {
                    playersAlive += 1;
                }
            }

            if (playersAlive == 0)
            {
                _levelTime.Stop();
                _levelDone = true;
                MainWindow.Instance.RunTimelineLevelFailed();
            }
        }

        public void RegisterCoinPickup(Coin coin, Player player)
        {
            if (_levelDone)
            {
                return;
            }

            SoundPlayer.Instance.Play(Sound.Bling, player.AudioSource);

            Destroy(coin.gameObject);
            Coins += 1;
        }

        public void Generate(int seed, int level, int playerCount, Difficulty difficulty)
        {
            Debug.Log($"Generating: Seed [{seed}], Level [{level}], Difficulty [{difficulty}]");


            Random.InitState(seed);


            // Reset GameObjects
            foreach (Player player in _players)
            {
                DestroyImmediate(player.gameObject);
            }
            _players.Clear();

            foreach (Sheep sheep in _sheep)
            {
                DestroyImmediate(sheep.gameObject);
            }
            _sheep.Clear();

            foreach (Haystack haystack in _haystacks)
            {
                DestroyImmediate(haystack.gameObject);
            }
            _haystacks.Clear();


            // Reset Status
            KilledSheep = 0;
            Coins = 0;


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
            else
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

                PlaceTileSquare(map, initialX, initialY, initialWidth, initialHeight, TileType.Open);
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
                            if (map.IsTileSurroundedBy(x, y, TileType.Open))
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
                            if (map.IsTileSurroundedBy(x, y, TileType.Open))
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
                            if (map.IsTileSurroundedBy(x, y, TileType.Open))
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
                            if (map.IsTileSurroundedBy(x, y, TileType.Open))
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
                                if (!map.IsValid(patchX, patchY) ||
                                    map.GetTile(patchX, patchY) != TileType.Outside)
                                {
                                    continue;
                                }

                                map.SetTile(patchX, patchY, TileType.Open);
                            }
                        }

                        placedPatches += 1;

                        Debug.Log("Placed Patch of Grass");
                    }
                }
            }

            // Add outside border
            map.AddBorder(_borderSize, TileType.Outside);
            

            // Cleanup: Remove small Patches of Outside Grass
            //{
            //    int outsideCount;
            //    const int removeCriteria = 3;

            //    for (int y = 0; y < map.Height; ++y)
            //    {
            //        outsideCount = _borderSize;
            //        for (int x = 0; x < map.Width; ++x)
            //        {
            //            if (map.GetTile(x, y) == TileType.Grass)
            //            {
            //                if (outsideCount > 0 && outsideCount <= removeCriteria)
            //                {
            //                    int removeX = x - 1;
            //                    while (map.GetTile(removeX, y) == TileType.Outside)
            //                    {
            //                        map.SetTile(removeX, x, TileType.Grass);
            //                        removeX -= 1;
            //                    }
            //                }

            //                outsideCount = 0;
            //            }
            //            else
            //            {
            //                Debug.Assert(map.GetTile(x, y) == TileType.Outside);
            //                outsideCount += 1;
            //            }
            //        }
            //    }

            //    for (int x = 0; x < map.Width; ++x)
            //    {
            //        outsideCount = _borderSize;
            //        for (int y = 0; y < map.Height; ++y)
            //        {
            //            if (map.GetTile(x, y) == TileType.Grass)
            //            {
            //                if (outsideCount > 0 && outsideCount <= removeCriteria)
            //                {
            //                    int removeY = y - 1;
            //                    while (map.GetTile(x, removeY) == TileType.Outside)
            //                    {
            //                        map.SetTile(x, removeY, TileType.Grass);
            //                        removeY -= 1;
            //                    }
            //                }

            //                outsideCount = 0;
            //            }
            //            else
            //            {
            //                Debug.Assert(map.GetTile(x, y) == TileType.Outside);
            //                outsideCount += 1;
            //            }
            //        }
            //    }
            //}


            
            
            // Add Player / Players
            Vector2Int center = new Vector2Int(map.Width / 2, map.Width / 2);
            int minSpawnX = center.x - 4;
            int maxSpawnX = center.x + 4;
            int minSpawnY = center.y - 4;
            int maxSpawnY = center.y + 4;

            {
                if (playerCount <= 1)
                {
                    map.SetTile(center.x, center.y, TileType.StartTile);
                }
                else if (playerCount == 2)
                {
                    map.SetTile(center.x - 2, center.y, TileType.StartTile);
                    map.SetTile(center.x + 2, center.y, TileType.StartTile);
                }
                else
                {
                    int restPlayers;
                    float radius;

                    if (playerCount <= 3)
                    {
                        radius = 1;
                        restPlayers = playerCount;
                    }
                    else
                    {
                        map.SetTile(center.x, center.y, TileType.StartTile);
                        radius = 2;
                        restPlayers = playerCount - 1;
                    }

                    float angle = 360f / restPlayers;
                    float currentAngle = Random.Range(0, 360);

                    for (int i = 0; i < restPlayers; ++i)
                    {
                        Vector2 v = Quaternion.Euler(0, 0, currentAngle) * Vector2.up;
                        v *= radius;

                        int shiftx;

                        if (Mathf.Abs(v.x) <= 0.5f)
                        {
                            shiftx = 0;
                        }
                        else
                        {
                            shiftx = v.x > 0 ? Mathf.CeilToInt(v.x) : Mathf.FloorToInt(v.x);
                        }

                        int shifty;

                        if (Mathf.Abs(v.y) <= 0.5f)
                        {
                            shifty = 0;
                        }
                        else
                        {
                            shifty = v.y > 0 ? Mathf.CeilToInt(v.y) : Mathf.FloorToInt(v.y);
                        }

                        int x = center.x + shiftx;
                        int y = center.y + shifty;

                        map.SetTile(x, y, TileType.StartTile);

                        minSpawnX = Mathf.Min(minSpawnX, x);
                        maxSpawnX = Mathf.Max(maxSpawnX, x);

                        minSpawnY = Mathf.Min(minSpawnY, y);
                        maxSpawnY = Mathf.Max(maxSpawnY, y);

                        currentAngle += angle;
                    }
                }

                //// Ensure that players are inside of a grass patch.

                //for (int x = minSpawnX; x <= maxSpawnX; ++x)
                //{
                //    for (int y = minSpawnY; y <= maxSpawnY; ++y)
                //    {
                //        if (map.GetTile(x, y) != TileType.StartTile)
                //        {
                //            map.SetTile(x, y, TileType.Grass);
                //        }
                //    }
                //}
            }


            // Add Sheep
            {
                int sheepCount;
                    
                if (difficulty == Difficulty.Easy)
                {
                    sheepCount = level + 1;
                }
                else if (difficulty == Difficulty.Normal)
                {
                    sheepCount = Mathf.FloorToInt(level * 1.5f) + 1;
                }
                else
                {
                    sheepCount = level * 2 + 1;
                }

                int addedSheeps = 0;
                int failCount = 0;

                while (addedSheeps < sheepCount && failCount < 1000)
                {
                    failCount += 1;

                    int x = Random.Range(0, map.Width);
                    int y = Random.Range(0, map.Height);

                    if (x >= minSpawnX && x <= maxSpawnX &&
                        y >= minSpawnY && y <= maxSpawnY)
                    {
                        continue;
                    }

                    if (map.GetTile(x, y) != TileType.Open)
                    {
                        continue;
                    }

                    map.SetTile(x, y, TileType.SheepTile);
                    addedSheeps += 1;
                    failCount = 0;
                }
            }


            // Add fences
            List<Vector2Int> nearFencePositions = new List<Vector2Int>();

            {
                List<Tuple<Vector2Int, TileType>> fencePositions = new List<Tuple<Vector2Int, TileType>>(); 

                for (int y = 0; y < map.Height; ++y)
                {
                    for (int x = 0; x < map.Width; ++x)
                    {
                        if (map.GetTile(x, y) == TileType.Outside)
                        {
                            bool grassN = false;
                            bool grassNE = false;
                            bool grassE = false;
                            bool grassSE = false;
                            bool grassS = false;
                            bool grassSW = false;
                            bool grassW = false;
                            bool grassNW = false;

                            if (map.IsValid(x, y + 1) &&
                                 map.GetTile(x, y + 1) != TileType.Outside)
                            {
                                // North Grass
                                grassN = true;
                                nearFencePositions.Add(new Vector2Int(x, y + 1));
                            }

                            if (map.IsValid(x + 1, y + 1) &&
                                 map.GetTile(x + 1, y + 1) != TileType.Outside)
                            {
                                // NorthEast Grass
                                grassNE = true;
                                nearFencePositions.Add(new Vector2Int(x + 1, y + 1));
                            }

                            if (map.IsValid(x + 1, y) &&
                                 map.GetTile(x + 1, y) != TileType.Outside)
                            {
                                // East Grass
                                grassE = true;
                                nearFencePositions.Add(new Vector2Int(x + 1, y));
                            }

                            if (map.IsValid(x + 1, y - 1) &&
                                 map.GetTile(x + 1, y - 1) != TileType.Outside)
                            {
                                // SouthEast Grass
                                grassSE = true;
                                nearFencePositions.Add(new Vector2Int(x + 1, y - 1));
                            }

                            if (map.IsValid(x, y - 1) &&
                                map.GetTile(x, y - 1) != TileType.Outside)
                            {
                                // South Grass
                                grassS = true;
                                nearFencePositions.Add(new Vector2Int(x, y - 1));
                            }

                            if (map.IsValid(x - 1, y - 1) &&
                                map.GetTile(x - 1, y - 1) != TileType.Outside)
                            {
                                // SouthWest Grass
                                grassSW = true;
                                nearFencePositions.Add(new Vector2Int(x - 1, y - 1));
                            }

                            if (map.IsValid(x - 1, y) &&
                                map.GetTile(x - 1, y) != TileType.Outside)
                            {
                                // West Grass
                                grassW = true;
                                nearFencePositions.Add(new Vector2Int(x - 1, y));
                            }

                            if (map.IsValid(x - 1, y + 1) &&
                                map.GetTile(x - 1, y + 1) != TileType.Outside)
                            {
                                // NorthWest Grass
                                grassNW = true;
                                nearFencePositions.Add(new Vector2Int(x - 1, y + 1));
                            }

                            if (grassN && grassE)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceNE));
                            }
                            else if (grassE && grassS)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceSE));
                            }
                            else if (grassS && grassW)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceSW));
                            }
                            else if (grassW && grassN)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceNW));
                            }
                            else if (grassN)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceS));
                            }
                            else if (grassE)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceW));
                            }
                            else if (grassS)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceN));
                            }
                            else if (grassW)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceE));
                            }
                            else if (grassNE)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceSW));
                            }
                            else if (grassSE)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceNW));
                            }
                            else if (grassSW)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceNE));
                            }
                            else if (grassNW)
                            {
                                fencePositions.Add(Tuple.Create(new Vector2Int(x, y), TileType.FenceSE));
                            }
                        }
                    }
                }

                foreach (var entry in fencePositions)
                {
                    map.SetTile(entry.Item1.x, entry.Item1.y, entry.Item2);
                }
            }

            // Add Pitchfork
            {
                for (int i = 0; i < 1000; ++i)
                {
                    int x = Random.Range(0, map.Width);
                    int y = Random.Range(0, map.Height);

                    if (x >= minSpawnX && x <= maxSpawnX &&
                        y >= minSpawnY && y <= maxSpawnY)
                    {
                        continue;
                    }

                    if (map.GetTile(x, y) == TileType.Open)
                    {
                        map.SetTile(x, y, TileType.Haystack);
                        break;
                    }
                }
            }

            // Add Blocked Tiles near Fences
            {
                int blockNearFences;

                if (difficulty == Difficulty.Easy)
                {
                    blockNearFences = level + Random.Range(2, 3);

                }
                else if (difficulty == Difficulty.Normal)
                {
                    blockNearFences = level + Random.Range(2, 4);
                }
                else
                {
                    blockNearFences = level + Random.Range(2, 5);
                }

                List<Vector2Int> addedPositions = new List<Vector2Int>();

                int addedBlockNearFences = 0;
                for (int i = 0; i < 1000 && addedBlockNearFences < blockNearFences; i++)
                {
                    Vector2Int position = nearFencePositions[Random.Range(0, nearFencePositions.Count)];
                    if (map.GetTile(position.x, position.y) != TileType.Open)
                    {
                        continue;
                    }

                    bool tooNear = false;

                    foreach (Vector2Int addedPosition in addedPositions)
                    {
                        if ((addedPosition - position).magnitude < 10)
                        {
                            if (Random.value < 0.8f)
                            {
                                tooNear = true;
                                break;
                            }
                        }
                    }

                    if (tooNear)
                    {
                        continue;
                    }

                    map.SetTile(position.x, position.y, TileType.Rock);
                    addedBlockNearFences += 1;
                    addedPositions.Add(position);
                }
            }


            // Add Blocked Tiles 
            {
                int blocks;

                if (difficulty == Difficulty.Easy)
                {
                    blocks = level + Random.Range(2, 3);

                }
                else if (difficulty == Difficulty.Normal)
                {
                    blocks = level + Random.Range(2, 4);
                }
                else
                {
                    blocks = level + Random.Range(2, 5);
                }


                int addedBlocks = 0;
                for (int i = 0; i < 1000 && addedBlocks < blocks; i++)
                {
                    int x = Random.Range(0, map.Width);
                    int y = Random.Range(0, map.Height);

                    if (x >= minSpawnX && x <= maxSpawnX &&
                        y >= minSpawnY && y <= maxSpawnY)
                    {
                        continue;
                    }

                    if (map.GetTile(x, y) == TileType.Open)
                    {
                        map.SetTile(x, y, TileType.Rock);
                        addedBlocks += 1;
                    }
                    else if (map.GetTile(x, y) == TileType.Outside)
                    {
                        map.SetTile(x, y, TileType.Rock);
                    }
                }
            }


            Map = map;

            int remainingPitchForks = Master.Instance.CurrentPitchForkCount;

            // Generate Real TileMap
            {
                List<Transform> playerTransforms = new List<Transform>();

                List<Vector2Int> setBlockedTiles = new List<Vector2Int>();

                for (int y = 0; y < map.Height; ++y)
                {
                    for (int x = 0; x < map.Width; ++x)
                    {
                        switch (map.GetTile(x, y))
                        {
                            case TileType.Outside:
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _outsideTiles[Random.Range(0, _outsideTiles.Length)]);
                                break;

                            case TileType.Open:
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                break;

                            case TileType.FenceN:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceN);
                                break;

                            case TileType.FenceNE:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceNE);
                                break;

                            case TileType.FenceNW:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceNW);
                                break;

                            case TileType.FenceS:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceS);
                                break;

                            case TileType.FenceSE:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceSE);
                                break;

                            case TileType.FenceSW:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceSW);
                                break;

                            case TileType.FenceE:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceE);
                                break;

                            case TileType.FenceW:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _fenceW);
                                break;
                               
                            case TileType.Rock:
                                map.SetTile(x, y, TileType.Blocked);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);
                                _wallTilemap.SetTile(new Vector3Int(x, y, 0), _rockTiles[Random.Range(0, _rockTiles.Length)]);
                                break;

                            case TileType.Haystack:
                                map.SetTile(x, y, TileType.Open);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);

                                Haystack haystack = Instantiate(_haystackPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                                _haystacks.Add(haystack);
                                break;

                            case TileType.StartTile:
                                map.SetTile(x, y, TileType.Open);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);

                                Player player = Instantiate(_playerPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                                _players.Add(player);
                                playerTransforms.Add(player.transform);

                                if (remainingPitchForks > 0)
                                {
                                    player.PickupPitchfork();
                                    remainingPitchForks -= 1;
                                }

                                break;

                            case TileType.SheepTile:
                                map.SetTile(x, y, TileType.Open);
                                _baseTilemap.SetTile(new Vector3Int(x, y, 0), _grassTiles[Random.Range(0, _grassTiles.Length)]);

                                Sheep sheep = Instantiate(_sheepPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                                _sheep.Add(sheep);

                                break;


                            default:
                                Debug.Assert(false, "Unknown Tile Type");
                                continue;
                        }

                    }
                }

                FollowCamera.Instance.FollowObjects = playerTransforms.ToArray();
            }

            _levelTime.Restart();
            _levelStarted = true;
        }

        #endregion Public Methods

        #region MonoBehaviour
	
        void Awake()
        {
            Debug.Assert(_playerPrefab);
            Debug.Assert(_sheepPrefab);
            Debug.Assert(_haystackPrefab);
            Debug.Assert(_baseTilemap);
            Debug.Assert(_wallTilemap);

            Debug.Assert(_outsideTiles != null && _outsideTiles.Length > 0);
            Debug.Assert(_grassTiles != null && _grassTiles.Length > 0);
            Debug.Assert(_rockTiles != null && _rockTiles.Length > 0);

            Debug.Assert(_fenceN);
            Debug.Assert(_fenceNE);
            Debug.Assert(_fenceE);
            Debug.Assert(_fenceSE);
            Debug.Assert(_fenceS);
            Debug.Assert(_fenceSW);
            Debug.Assert(_fenceW);
            Debug.Assert(_fenceNW);

            Debug.Assert(_coinPrefab);

            Debug.Log("Game Awake");
            Instance = this;

            Physics2D.autoSimulation = false;
        }

        void Update()
        {
            float physicStep = Mathf.Min(Time.deltaTime, 1f / 30f);
            Physics2D.Simulate(physicStep);

            if (_levelStarted && !_levelDone)
            {
#if UNITY_STANDALONE

                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Master.Instance.SetSceneType(SceneType.StartScreen);
                }
#endif

            }
        }

        #endregion MonoBehaviour

        #region Private Methods

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
