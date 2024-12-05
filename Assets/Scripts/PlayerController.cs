using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveDuration;

    private Vector2Int _currentCellPosition;
    private bool _isMoving;

    private Coroutine _moveCoroutine;

    private BoardManager _boardManager;

    void Awake()
    {
        _boardManager = FindFirstObjectByType<BoardManager>();
    }

    void Update()
    {
        if (GameManager.Instance.IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameManager.Instance.StartNewGame();
            }

            return;
        }

        if (_isMoving)
        {
            return;
        }

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
        SetPosition(position, true);
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int newPosition = _currentCellPosition + direction;

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
        }
    }

    private void SetPosition(Vector2Int position, bool immediate = false)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }

        _moveCoroutine = StartCoroutine(Move(position, immediate));
    }

    private IEnumerator Move(Vector2Int position, bool immediate = false)
    {
        _isMoving = true;

        if (!immediate && _moveDuration > 0)
        {
            Vector3 startingPos = transform.position;
            Vector3 finalPos = _boardManager.CellToWorld(position);

            float elapsedTime = 0;
            while (elapsedTime < _moveDuration)
            {
                transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / _moveDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        transform.position = _boardManager.CellToWorld(position);
        _currentCellPosition = position;

        _isMoving = false;

        var cellObject = _boardManager.GetObject(position);
        if (cellObject != null)
        {
            cellObject.PlayerEntered();
        }
    }
}
