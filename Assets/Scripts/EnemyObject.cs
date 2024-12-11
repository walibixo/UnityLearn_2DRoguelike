using UnityEngine;

public class EnemyObject : CellObject
{
    [SerializeField] private int _enemyHitPoints;

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);

        GameManager.Instance.TurnManager.OnTick += OnNewTurn;
    }

    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnTick -= OnNewTurn;
    }

    private void OnNewTurn()
    {
        Vector2Int playerPosition = GameManager.Instance.PlayerController.CellPosition;

        Vector2Int direction = playerPosition - _cellPosition;

        Debug.DrawLine(transform.position, GameManager.Instance.BoardManager.CellToWorld(playerPosition), Color.red);

        if (IsPlayerInRange(direction))
        {
            return;
        }

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (!TryMove(new Vector2Int(Mathf.RoundToInt(Mathf.Sign(direction.x)), 0)))
            {
                TryMove(new Vector2Int(0, Mathf.RoundToInt(Mathf.Sign(direction.y))));
            }
        }
        else
        {
            if (!TryMove(new Vector2Int(0, Mathf.RoundToInt(Mathf.Sign(direction.y)))))
            {
                TryMove(new Vector2Int(Mathf.RoundToInt(Mathf.Sign(direction.x)), 0));
            }
        }
    }

    private bool IsPlayerInRange(Vector2Int direction)
    {
        return Mathf.Abs(direction.x) <= 1 && Mathf.Abs(direction.y) <= 1;
    }

    private bool TryMove(Vector2Int direction)
    {
        if (direction.x == 0 && direction.y == 0)
        {
            return false;
        }

        Vector2Int newCell = _cellPosition + direction;
        if (GameManager.Instance.BoardManager.IsEmpty(newCell))
        {
            GameManager.Instance.BoardManager.MoveObject(_cellPosition, newCell);
            _cellPosition = newCell;
            transform.position = GameManager.Instance.BoardManager.CellToWorld(newCell);
            return true;
        }

        return false;
    }

    public override bool PlayerTryEnter(PlayerController playerController)
    {
        if (_enemyHitPoints > 0)
        {
            playerController.Attack();

            _enemyHitPoints--;
        }

        if (_enemyHitPoints == 0)
        {
            Destroy(gameObject);
        }

        return false;
    }
}
