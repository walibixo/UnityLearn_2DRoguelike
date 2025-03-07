using UnityEngine;
using UnityEngine.Tilemaps;

public class WallObject : CellObject
{
    [SerializeField] private Tile[] _wallTiles;

    [Header("Sounds")]
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _destroyedSound;

    private int _wallHitPoints;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        _wallHitPoints = _wallTiles.Length - 1;
        SetWallTile();
    }

    private void SetWallTile()
    {
        GameManager.Instance.BoardManager.SetCellTile(_cellPosition, _wallTiles[_wallHitPoints]);
    }

    public override bool PlayerTryEnter(PlayerController playerController)
    {
        if (_wallHitPoints > 0)
        {
            playerController.Attack();

            _wallHitPoints--;

            if (_wallHitPoints > 0)
            {
                GameManager.Instance.SoundManager.PlaySound(_hitSound, true);
            }
            else
            {
                GameManager.Instance.SoundManager.PlaySound(_destroyedSound, true);
            }

            SetWallTile();
            return false;
        }
        else
        {
            Destroy(gameObject);
            return true;
        }
    }
}
