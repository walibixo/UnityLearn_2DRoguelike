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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        _cellsData = new CellData[_width, _height];

        SetGroundTiles();
        SetWallTiles();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetGroundTiles()
    {
        for (int y = 0; y < _height; ++y)
        {
            for (int x = 0; x < _width; ++x)
            {
                _tilemap.SetTile(new Vector3Int(x, y, 0), GetGroundTile());
                SetCellData(x, y, true);
            }
        }
    }
    private Tile GetGroundTile() => _groundTiles[Random.Range(0, _groundTiles.Length)];

    private void SetWallTiles()
    {
        for (int x = 0; x < _width; ++x)
        {
            _tilemap.SetTile(new Vector3Int(x, 0, 0), GetWallTile());
            SetCellData(x, 0, false);

            _tilemap.SetTile(new Vector3Int(x, _height - 1, 0), GetWallTile());
            SetCellData(x, _height - 1, false);
        }

        for (int y = 0; y < _height; ++y)
        {
            _tilemap.SetTile(new Vector3Int(0, y, 0), GetWallTile());
            SetCellData(0, y, false);

            _tilemap.SetTile(new Vector3Int(_width - 1, y, 0), GetWallTile());
            SetCellData(_width - 1, y, false);
        }
    }

    private Tile GetWallTile() => _wallTiles[Random.Range(0, _wallTiles.Length)];

    private void SetCellData(int x, int y, bool isPassable)
    {
        _cellsData[x, y] = new CellData(isPassable);
    }
}
