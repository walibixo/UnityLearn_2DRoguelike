using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    [SerializeField] private Tile _exitTile;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        GameManager.Instance.BoardManager.SetCellTile(_cellPosition, _exitTile);
    }

    public override void PlayerEntered(PlayerController playerController)
    {
        GameManager.Instance.StartNewLevel();
    }
}
