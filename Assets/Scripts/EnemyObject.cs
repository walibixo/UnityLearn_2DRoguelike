using System.Collections;
using UnityEngine;

public class EnemyObject : CellObject
{
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int AttacksHash = Animator.StringToHash("Attacks");
    private static readonly int IsHurtHash = Animator.StringToHash("IsHurt");

    [SerializeField] private int _hitPoints;
    [SerializeField] private int _attackPoints;

    [SerializeField] private float _moveDuration;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _hurtDuration;

    private bool _isPerformingAction;
    private bool _isMoving;
    private bool _isAttacking;
    private bool _isHurting;

    public bool IsPerformingAction => _isPerformingAction || _isMoving || _isAttacking || _isHurting;

    private Coroutine _moveCoroutine;
    private Coroutine _attackCoroutine;
    private Coroutine _hurtCoroutine;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.TurnManager.RegisterEnemy(this);
        GameManager.Instance.TurnManager.OnStartEnemyTurn += OnNewTurn;
    }

    private void OnDestroy()
    {
        GameManager.Instance.TurnManager.UnregisterEnemy(this);
        GameManager.Instance.TurnManager.OnStartEnemyTurn -= OnNewTurn;
    }

    public override void Init(Vector2Int cell)
    {
        base.Init(cell);
    }

    public override bool PlayerTryEnter(PlayerController playerController)
    {
        playerController.Attack();
        Hurt(playerController.AttackPoints);

        return false;
    }

    public void Hurt(int hurtPoints)
    {
        if (_hurtCoroutine != null)
        {
            StopCoroutine(_hurtCoroutine);
        }
        _hurtCoroutine = StartCoroutine(HurtCoroutine(hurtPoints));
    }

    private void OnNewTurn()
    {
        _isPerformingAction = true;
        PerformAction();
        _isPerformingAction = false;
    }

    private void PerformAction()
    {
        Vector2Int playerPosition = GameManager.Instance.PlayerController.CellPosition;

        Vector2Int direction = playerPosition - _cellPosition;

        if (direction.x > 0)
        {
            FlipSprite(true);
        }
        else if (direction.x < 0)
        {
            FlipSprite(false);
        }

        if (IsPlayerInRange(direction))
        {
            AttackPlayer();
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

    private void AttackPlayer()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }
        _attackCoroutine = StartCoroutine(AttackCoroutine());
    }

    private bool IsPlayerInRange(Vector2Int direction)
    {
        return Mathf.Abs(direction.sqrMagnitude) <= 1;
    }

    private bool CanMove(Vector2Int direction, out Vector2Int newCell)
    {
        newCell = _cellPosition + direction;

        if (direction.x == 0 && direction.y == 0)
        {
            return false;
        }

        return GameManager.Instance.BoardManager.IsEmpty(_cellPosition + direction);
    }

    private bool TryMove(Vector2Int direction)
    {
        if (!CanMove(direction, out Vector2Int newCell))
        {
            return false;
        }

        GameManager.Instance.BoardManager.MoveObject(_cellPosition, newCell);

        SetPosition(newCell);

        return true;
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
        _cellPosition = position;

        _isMoving = false;
        _animator.SetBool(IsMovingHash, false);
    }

    private IEnumerator AttackCoroutine()
    {
        _isAttacking = true;

        _animator.SetTrigger(AttacksHash);
        yield return new WaitForSeconds(_attackDuration);

        GameManager.Instance.PlayerController.Hurt(_attackPoints);

        _isAttacking = false;
    }

    private IEnumerator HurtCoroutine(int hurtPoints)
    {
        _isHurting = true;

        _animator.SetTrigger(IsHurtHash);
        yield return new WaitForSeconds(_hurtDuration);

        _hitPoints -= hurtPoints;

        _isHurting = false;

        if (_hitPoints <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FlipSprite(bool flip)
    {
        Vector3 scale = transform.localScale;
        scale.x = flip ? -1 : 1;
        transform.localScale = scale;
    }
}
