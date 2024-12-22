using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttacksHash = Animator.StringToHash("Attacks");
    private static readonly int IsHurtHash = Animator.StringToHash("IsHurt");

    public Vector2Int CellPosition { get; private set; }
    public int AttackPoints { get; private set; } = 1;

    [SerializeField] private float _moveDuration;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _hurtDuration;

    private Animator _animator;

    private bool _isMoving;
    private bool _isAttacking;
    private bool _isHurting;

    private Coroutine _moveCoroutine;
    private Coroutine _attackCoroutine;
    private Coroutine _hurtCoroutine;

    void Awake()
    {
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
        _attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    public void Hurt(int hurtPoints)
    {
        if (_hurtCoroutine != null)
        {
            StopCoroutine(_hurtCoroutine);
        }
        _hurtCoroutine = StartCoroutine(HurtCoroutine());
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
            GameManager.Instance.TurnManager.Tick();
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
        }

        if(!immediate)
            GameManager.Instance.TurnManager.Tick();
    }

    private IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        _animator.SetTrigger(AttacksHash);
        yield return new WaitForSeconds(_attackDuration);

        _isAttacking = false;
    }

    private IEnumerator HurtCoroutine()
    {
        _isHurting = true;

        _animator.SetTrigger(IsHurtHash);
        yield return new WaitForSeconds(_attackDuration);

        _isHurting = false;
    }

    private void FlipSprite(bool flip)
    {
        Vector3 scale = transform.localScale;
        scale.x = flip ? -1 : 1;
        transform.localScale = scale;
    }
}
