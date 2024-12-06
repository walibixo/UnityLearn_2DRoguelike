using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    [SerializeField] private Tile[] _wallTiles;

    private int _wallHealth;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        _wallHealth = _wallTiles.Length - 1;
        SetWallTile();
    }

    private void SetWallTile()
    {
        GameManager.Instance.BoardManager.SetCellTile(_cellPosition, _wallTiles[_wallHealth]);
    }

    public override bool PlayerTryEnter(PlayerController playerController)
    {
        if (_wallHealth > 0)
        {
            playerController.Attack();

            _wallHealth--;
            SetWallTile();
            return false;
        }
        else
        {
            return true;
        }
    }
}
