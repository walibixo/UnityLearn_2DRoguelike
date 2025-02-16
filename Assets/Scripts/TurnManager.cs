using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public event System.Action OnStartEnemyTurn;
    public event System.Action OnStartPlayerTurn;

    private PlayerController _playerController;

    private readonly ConcurrentBag<EnemyObject> _enemies = new();

    private int _turnCount;

    private void Awake()
    {
        _playerController = FindFirstObjectByType<PlayerController>();

        _turnCount = 0;
    }

    public void StartPlayerTurn()
    {
        _turnCount += 1;

        Debug.Log($"Turn {_turnCount} - Player");

        OnStartPlayerTurn?.Invoke();
        StartCoroutine(PlayerTurnCoroutine());
    }

    private IEnumerator PlayerTurnCoroutine()
    {
        while (_playerController.IsPerformingAction)
        {
            yield return null;
        }

        foreach (var enemy in _enemies)
        {
            while (enemy.IsPerformingAction)
            {
                yield return null;
            }
        }

        StartEnemyTurn();
    }

    public void StartEnemyTurn()
    {
        _turnCount += 1;

        Debug.Log($"Turn {_turnCount} - Enemies");

        OnStartEnemyTurn?.Invoke();
        StartCoroutine(EnemyTurnCoroutine());
    }

    private IEnumerator EnemyTurnCoroutine()
    {
        foreach (var enemy in _enemies)
        {
            while (enemy.IsPerformingAction)
            {
                yield return null;
            }
        }

        while (_playerController.IsPerformingAction)
        {
            yield return null;
        }

        StartPlayerTurn();
    }

    public void RegisterEnemy(EnemyObject enemy)
    {
        _enemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyObject enemy)
    {
        _enemies.TryTake(out enemy);
    }
}
