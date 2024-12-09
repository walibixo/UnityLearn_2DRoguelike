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
