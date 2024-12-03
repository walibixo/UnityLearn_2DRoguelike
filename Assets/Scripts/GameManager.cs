using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerController _playerController;
    private UIDocument _uiDocument;
    private Label _foodLabel;

    public BoardManager BoardManager { get; private set; }
    public TurnManager TurnManager { get; private set; }

    public bool IsGameOver { get; private set; }

    private int _levelCount;
    private int _foodAmount;

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
        BoardManager = FindFirstObjectByType<BoardManager>();
        _uiDocument = FindFirstObjectByType<UIDocument>();
        _foodLabel = _uiDocument.rootVisualElement.Q<Label>("FoodLabel");

        TurnManager = new TurnManager();
        TurnManager.OnTick += OnNewTurn;

        UpdateFoodAmount(100);

        StartNewLevel();
    }

    public void StartNewLevel()
    {
        _levelCount++;

        BoardManager.ClearBoard();
        BoardManager.GenerateBoard();

        _playerController.Spawn(BoardManager.PlayerStart);
    }

    public void GameOver()
    {
        IsGameOver = true;
        Debug.Log("Game Over");
    }

    private void OnNewTurn()
    {
        if (_foodAmount > 0)
        {
            UpdateFoodAmount(-1);
        }
        else
        {
            GameOver();
        }
    }

    public void UpdateFoodAmount(int relativeAmount = 0)
    {
        _foodAmount += relativeAmount;
        _foodLabel.text = "Food : " + _foodAmount;
    }
}
