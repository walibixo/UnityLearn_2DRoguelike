using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttacksHash = Animator.StringToHash("Attacks");

    [SerializeField] private float _moveDuration;
    [SerializeField] private float _attackDuration;

    private BoardManager _boardManager;
    private Animator _animator;

    private Vector2Int _currentCellPosition;
    private bool _isMoving;
    private bool _isAttacking;

    private Coroutine _moveCoroutine;
    private Coroutine _attackCoroutine;

    void Awake()
    {
        _boardManager = FindFirstObjectByType<BoardManager>();
        _animator = GetComponent<Animator>();
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

        if (_isMoving || _isAttacking)
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

    public void Attack()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }
        _attackCoroutine = StartCoroutine(AttackCoroutine());
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
        else if (cellObject.PlayerTryEnter(this))
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

        _moveCoroutine = StartCoroutine(MoveCoroutine(position, immediate));
    }

    private IEnumerator MoveCoroutine(Vector2Int position, bool immediate = false)
    {
        _isMoving = true;
        _animator.SetBool(IsMovingHash, true);

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
        _animator.SetBool(IsMovingHash, false);

        var cellObject = _boardManager.GetObject(position);
        if (cellObject != null)
        {
            cellObject.PlayerEntered(this);
        }
    }

    private IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        _animator.SetTrigger(AttacksHash);
        yield return new WaitForSeconds(_attackDuration);

        _isAttacking = false;
    }
}
