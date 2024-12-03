using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2Int _cellPosition;

    private BoardManager _boardManager;

    void Awake()
    {
        _boardManager = FindFirstObjectByType<BoardManager>();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            TryMove(Vector2Int.up);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            TryMove(Vector2Int.down);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            TryMove(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            TryMove(Vector2Int.right);
        }
    }

    public void Spawn(Vector2Int position)
    {
        SetPosition(position);
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int newPosition = _cellPosition + direction;

        if (!_boardManager.IsPassable(newPosition))
        {
            return;
        }

        GameManager.Instance.TurnManager.Tick();

        var cellObject = _boardManager.GetObject(newPosition);
        if (cellObject == null)
        {
            SetPosition(newPosition);
        }
        else if (cellObject.PlayerTryEnter())
        {
            SetPosition(newPosition);
            cellObject.PlayerEntered();
        }
    }

    private void SetPosition(Vector2Int position)
    {
        _cellPosition = position;
        transform.position = _boardManager.CellToWorld(position);
    }
}
