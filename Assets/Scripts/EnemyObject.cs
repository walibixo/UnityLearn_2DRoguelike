using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public Guid Id { get; } = Guid.NewGuid();

    private readonly Vector2Int[] _directions
        = new Vector2Int[]
        {
            new (1, 0),
            new (-1, 0),
            new (0, 1),
            new (0, -1),
            new (0, 0)
        };

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

        _isHurting = true;
        _hurtCoroutine = StartCoroutine(HurtCoroutine(hurtPoints));

        IEnumerator HurtCoroutine(int hurtPoints)
        {
            _animator.SetTrigger(IsHurtHash);
            yield return new WaitForSeconds(_hurtDuration);

            _hitPoints -= hurtPoints;

            _isHurting = false;

            if (_hitPoints <= 0)
            {
                Destroy(gameObject);
            }
        }
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

        if (IsPlayerInAttackRange(direction))
        {
            AttackPlayer();
            return;
        }

        if (IsPlayerInAggroRange(direction))
        {
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
        else
        {
            TryMove(_directions[Random.Range(0, _directions.Length)]);
        }
    }

    private void AttackPlayer()
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

            GameManager.Instance.PlayerController.Hurt(_attackPoints);

            _isAttacking = false;
        }
    }

    private bool IsPlayerInAttackRange(Vector2Int direction)
    {
        return Mathf.Abs(direction.sqrMagnitude) <= 1;
    }

    private bool IsPlayerInAggroRange(Vector2Int direction)
    {
        Debug.Log(direction.sqrMagnitude);
        return Mathf.Abs(direction.sqrMagnitude) <= 5;
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
            _cellPosition = position;

            _isMoving = false;
            _animator.SetBool(IsMovingHash, false);
        }
    }

    private void FlipSprite(bool flip)
    {
        Vector3 scale = transform.localScale;
        scale.x = flip ? -1 : 1;
        transform.localScale = scale;
    }
}
