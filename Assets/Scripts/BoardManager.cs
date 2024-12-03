using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    public static readonly Vector2Int PlayerStart = new(1, 1);

    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private Tile[] _wallTiles;

    [SerializeField] private FoodObject[] _foodPrefabs;
    [SerializeField] private int _foodAmountMin;
    [SerializeField] private int _foodAmountMax;

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
    }

    public void GenerateBoard()
    {
        _cellsData = new CellData[_width, _height];
        _emptyCells = new List<Vector2Int>();

        SetGroundTiles();
        SetFoodTiles();
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return _grid.GetCellCenterWorld((Vector3Int)cellIndex);
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
                    _tilemap.SetTile(new Vector3Int(x, y, 0), GetWallTile());
                    SetCellData(x, y, false);
                    continue;
                }

                _tilemap.SetTile(new Vector3Int(x, y, 0), GetGroundTile());
                SetCellData(x, y, true);

                _emptyCells.Add(new Vector2Int(x, y));
            }
        }

        _emptyCells.Remove(PlayerStart);
    }

    private Tile GetGroundTile() => _groundTiles[Random.Range(0, _groundTiles.Length)];

    private Tile GetWallTile() => _wallTiles[Random.Range(0, _wallTiles.Length)];

    private void SetCellData(int x, int y, bool isPassable, CellObject containedObject = null)
    {
        _cellsData[x, y] = new CellData(isPassable, containedObject);
    }

    private void SetFoodTiles()
    {
        int foodAmount = Random.Range(_foodAmountMin, _foodAmountMax + 1);
        for (int i = 0; i < foodAmount; ++i)
        {
            Vector2Int position = _emptyCells[Random.Range(0, _emptyCells.Count)];
            _emptyCells.Remove(position);

            FoodObject food = Instantiate(GetFoodPrefab(), CellToWorld(position), Quaternion.identity);
            SetCellData(position.x, position.y, true, food);
        }
    }

    private FoodObject GetFoodPrefab() => _foodPrefabs[Random.Range(0, _foodPrefabs.Length)];
}
