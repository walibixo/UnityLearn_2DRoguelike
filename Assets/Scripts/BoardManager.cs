using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public static readonly Vector2Int PlayerStart = new(1, 1);

    [Header("Board Size")]
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [Header("Base Tiles")]
    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private Tile[] _borderTiles;

    [Header("Food Objects")]
    [SerializeField] private FoodObject[] _foodPrefabs;
    [SerializeField] private int _foodAmountMin;
    [SerializeField] private int _foodAmountMax;

    [Header("Wall Objects")]
    [SerializeField] private WallObject[] _wallPrefabs;
    [SerializeField] private int _wallAmountMin;
    [SerializeField] private int _wallAmountMax;

    [Header("EnemyObjects")]
    [SerializeField] private EnemyObject[] _enemyPrefabs;
    [SerializeField] private int _enemyAmountMin;
    [SerializeField] private int _enemyAmountMax;

    [Header("Exit Object")]
    [SerializeField] private ExitCellObject _exitCellPrefab;

    public record CellData
    {
        public bool Passable;
        public CellObject ContainedObject;

        public CellData(bool passable, CellObject containedObject = null)
        {
            Passable = passable;
            ContainedObject = containedObject;
        }
    }

    private CellData[,] _cellsData;
    private List<Vector2Int> _emptyCells;

    private Tilemap _tilemap;
    private Grid _grid;

    void Awake()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        _grid = GetComponentInChildren<Grid>();

        _cellsData = new CellData[_width, _height];
        _emptyCells = new List<Vector2Int>();
    }

    public void GenerateBoard()
    {
        SetGroundTiles();
        SetExitObject();
        SetFoodObjects();
        SetWallObjects();
        SetEnemyObjects();
    }

    public void ClearBoard()
    {
        _emptyCells.Clear();

        for (int y = 0; y < _height; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                CellData cellData = _cellsData[x, y];
                if (cellData != null && cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(x, y, null);
            }
        }
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return _grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public void SetCellTile(Vector2Int position, Tile tile)
    {
        SetCellTile(position.x, position.y, tile);
    }

    public void SetCellTile(int x, int y, Tile tile)
    {
        _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
    }

    public bool IsPassable(Vector2Int position)
    {
        var cellData = GetCellData(position);
        return cellData != null && cellData.Passable;
    }

    public CellObject GetObject(Vector2Int position)
    {
        var cellData = GetCellData(position);
        return cellData != null ? cellData.ContainedObject : null;
    }

    private CellData GetCellData(Vector2Int position)
    {
        if (position.x < 0 || position.x >= _width
            || position.y < 0 || position.y >= _height)
        {
            return null;
        }

        return _cellsData[position.x, position.y];
    }

    private void SetGroundTiles()
    {
        for (int y = 0; y < _height; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                // Border tiles are walls
                if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1)
                {
                    SetCellTile(x, y, GetBorderTile());
                    SetCellData(x, y, false);
                    continue;
                }

                SetCellTile(x, y, GetGroundTile());
                SetCellData(x, y, true);

                _emptyCells.Add(new Vector2Int(x, y));
            }
        }

        _emptyCells.Remove(PlayerStart);
    }

    private Tile GetGroundTile() => _groundTiles[Random.Range(0, _groundTiles.Length)];

    private Tile GetBorderTile() => _borderTiles[Random.Range(0, _borderTiles.Length)];

    private void SetCellData(int x, int y, bool isPassable, CellObject containedObject = null)
    {
        _cellsData[x, y] = new CellData(isPassable, containedObject);
    }

    private void SetFoodObjects()
    {
        int foodAmount = Random.Range(_foodAmountMin, _foodAmountMax + 1);
        for (int i = 0; i < foodAmount; ++i)
        {
            SetObject(GetFoodPrefab());
        }
    }

    private FoodObject GetFoodPrefab() => _foodPrefabs[Random.Range(0, _foodPrefabs.Length)];

    private void SetWallObjects()
    {
        int wallAmount = Random.Range(_wallAmountMin, _wallAmountMax + 1);
        for (int i = 0; i < wallAmount; ++i)
        {
            SetObject(GetWallPrefab());
        }
    }

    private WallObject GetWallPrefab() => _wallPrefabs[Random.Range(0, _wallPrefabs.Length)];

    private void SetEnemyObjects()
    {
        int enemyAmount = Random.Range(_enemyAmountMin, _enemyAmountMax + 1);
        for (int i = 0; i < enemyAmount; ++i)
        {
            SetObject(GetEnemyPrefab());
        }
    }

    private EnemyObject GetEnemyPrefab() => _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];

    private void SetObject<T>(T prefab) where T : CellObject
    {
        Vector2Int position = _emptyCells[Random.Range(0, _emptyCells.Count)];
        SetObject(prefab, position);
    }

    private void SetObject<T>(T prefab, Vector2Int position) where T : CellObject
    {
        _emptyCells.Remove(position);

        T cellObject = Instantiate(prefab, CellToWorld(position), Quaternion.identity);
        cellObject.Init(position);

        SetCellData(position.x, position.y, true, cellObject);
    }

    private void SetExitObject()
    {
        Vector2Int position = new Vector2Int(_width - 2, _height - 2);
        SetObject(_exitCellPrefab, position);
    }
}
