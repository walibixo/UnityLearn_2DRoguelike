using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    [SerializeField] private Tile[] _groundTiles;
    [SerializeField] private Tile[] _wallTiles;

    public record CellData
    {
        public bool Passable;

        public CellData(bool passable) => Passable = passable;
    }

    private CellData[,] _cellsData;

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

        SetGroundTiles();
    }

    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return _grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public bool IsPassable(Vector2Int position)
    {
        if (position.x < 0 || position.x >= _width
            || position.y < 0 || position.y >= _height)
        {
            return false;
        }

        return _cellsData[position.x, position.y].Passable;
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
            }
        }
    }
    private Tile GetGroundTile() => _groundTiles[Random.Range(0, _groundTiles.Length)];

    private Tile GetWallTile() => _wallTiles[Random.Range(0, _wallTiles.Length)];

    private void SetCellData(int x, int y, bool isPassable)
    {
        _cellsData[x, y] = new CellData(isPassable);
    }
}
