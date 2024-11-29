using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2Int _cellPosition;

    private BoardManager _boardManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _boardManager = FindFirstObjectByType<BoardManager>();
    }

    // Update is called once per frame
    void Update()
    {
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

    public void Spawn(int x, int y)
    {
        SetPosition(new Vector2Int(x, y));
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int newPosition = _cellPosition + direction;

        if (_boardManager.IsPassable(newPosition))
        {
            SetPosition(newPosition);
        }
    }

    private void SetPosition(Vector2Int position)
    {
        _cellPosition = position;
        transform.position = _boardManager.CellToWorld(position);
    }
}
