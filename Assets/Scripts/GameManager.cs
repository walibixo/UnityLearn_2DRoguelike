using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerController _playerController;
    private BoardManager _boardManager;

    public TurnManager TurnManager { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        _playerController = FindFirstObjectByType<PlayerController>();
        _boardManager = FindFirstObjectByType<BoardManager>();

        TurnManager = new TurnManager();
        _boardManager.GenerateBoard();
        _playerController.Spawn(1, 1);
    }
}
