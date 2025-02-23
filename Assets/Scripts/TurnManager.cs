using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public event System.Action OnStartEnemyTurn;
    public event System.Action OnStartPlayerTurn;

    private PlayerController _playerController;

    private readonly ConcurrentDictionary<Guid, EnemyObject> _enemies = new();

    private int _turnCount;

    private void Awake()
    {
        _playerController = FindFirstObjectByType<PlayerController>();

        _turnCount = 0;
    }

    public void Start()
    {
        StopAllCoroutines();
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        _turnCount += 1;

        OnStartPlayerTurn?.Invoke();
        StartCoroutine(PlayerTurnCoroutine());
    }

    private IEnumerator PlayerTurnCoroutine()
    {
        while (_playerController.IsPerformingAction)
        {
            yield return null;
        }

        yield return null;

        foreach (var (_, enemy) in _enemies)
        {
            while (enemy.IsPerformingAction)
            {
                yield return null;
            }
        }

        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        _turnCount += 1;

        OnStartEnemyTurn?.Invoke();
        StartCoroutine(EnemyTurnCoroutine());
    }

    private IEnumerator EnemyTurnCoroutine()
    {
        foreach (var (_, enemy) in _enemies)
        {
            while (enemy.IsPerformingAction)
            {
                yield return null;
            }
        }

        yield return null;

        while (_playerController.IsPerformingAction)
        {
            yield return null;
        }

        StartPlayerTurn();
    }

    public void RegisterEnemy(EnemyObject enemy)
    {
        _enemies.TryAdd(enemy.Id, enemy);
    }

    public void UnregisterEnemy(EnemyObject enemy)
    {
        _enemies.TryRemove(enemy.Id, out _);
    }
}
