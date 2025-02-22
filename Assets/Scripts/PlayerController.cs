using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttacksHash = Animator.StringToHash("Attacks");
    private static readonly int IsHurtHash = Animator.StringToHash("IsHurt");

    public Vector2Int CellPosition { get; private set; }
    public int AttackPoints { get; private set; }

    [SerializeField] private float _moveDuration;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _hurtDuration;

    private Animator _animator;

    private bool _isPerformingAction;
    private bool _isMoving;
    private bool _isAttacking;
    private bool _isHurting;

    public bool IsPerformingAction => _isPerformingAction || _isMoving || _isAttacking || _isHurting;

    private Coroutine _moveCoroutine;
    private Coroutine _attackCoroutine;
    private Coroutine _hurtCoroutine;

    void Awake()
    {
        _animator = GetComponent<Animator>();

        AttackPoints = 1;
    }

    private void Start()
    {
        GameManager.Instance.TurnManager.OnStartPlayerTurn += OnNewTurn;
    }

    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.OnStartPlayerTurn -= OnNewTurn;
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

        if (!_isPerformingAction)
        {
            return;
        }

        if (_isMoving || _isAttacking || _isHurting)
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
            FlipSprite(true);
            TryMove(Vector2Int.left);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            FlipSprite(false);
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

        _isAttacking = true;
        _attackCoroutine = StartCoroutine(AttackCoroutine());

        IEnumerator AttackCoroutine()
        {
            _animator.SetTrigger(AttacksHash);
            yield return new WaitForSeconds(_attackDuration);

            _isAttacking = false;
        }
    }

    public void Hurt(int hurtPoints)
    {
        if (_hurtCoroutine != null)
        {
            StopCoroutine(_hurtCoroutine);
        }

        _isHurting = true;
        _hurtCoroutine = StartCoroutine(HurtCoroutine());

        IEnumerator HurtCoroutine()
        {
            _animator.SetTrigger(IsHurtHash);
            yield return new WaitForSeconds(_hurtDuration);

            _isHurting = false;
        }
    }

    private void OnNewTurn()
    {
        _isPerformingAction = true;
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int newPosition = CellPosition + direction;

        if (!GameManager.Instance.BoardManager.IsPassable(newPosition))
        {
            return;
        }

        var cellObject = GameManager.Instance.BoardManager.GetObject(newPosition);
        if (cellObject == null)
        {
            SetPosition(newPosition);
        }
        else if (cellObject.PlayerTryEnter(this))
        {
            SetPosition(newPosition);
        }
        else
        {
            _isPerformingAction = false;
        }
    }

    private void SetPosition(Vector2Int position, bool immediate = false)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
        }

        _isMoving = true;
        _moveCoroutine = StartCoroutine(MoveCoroutine(position, immediate));

        IEnumerator MoveCoroutine(Vector2Int position, bool immediate = false)
        {
            _animator.SetBool(IsMovingHash, true);

            if (!immediate && _moveDuration > 0)
            {
                Vector3 startingPos = transform.position;
                Vector3 finalPos = GameManager.Instance.BoardManager.CellToWorld(position);

                float elapsedTime = 0;
                while (elapsedTime < _moveDuration)
                {
                    transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / _moveDuration));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }

            transform.position = GameManager.Instance.BoardManager.CellToWorld(position);
            CellPosition = position;

            _isMoving = false;
            _animator.SetBool(IsMovingHash, false);

            var cellObject = GameManager.Instance.BoardManager.GetObject(position);
            if (cellObject != null)
            {
                cellObject.PlayerEntered(this);
                yield return null;
            }

            if (!immediate)
                _isPerformingAction = false;
        }
    }

    private void FlipSprite(bool flip)
    {
        Vector3 scale = transform.localScale;
        scale.x = flip ? -1 : 1;
        transform.localScale = scale;
    }
}
