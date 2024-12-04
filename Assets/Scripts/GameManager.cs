using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private PlayerController _playerController;
    private UIDocument _uiDocument;
    private Label _foodLabel;
    private VisualElement _gameOverPanel;
    private Label _gameOverLabel;

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
        _gameOverPanel = _uiDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        _gameOverLabel = _gameOverPanel.Q<Label>("GameOverLabel");

        TurnManager = new TurnManager();
        TurnManager.OnTick += OnNewTurn;

        StartNewGame();
    }

    public void StartNewGame()
    {
        _levelCount = 0;
        _foodAmount = 0;
        IsGameOver = false;

        _gameOverPanel.style.visibility = Visibility.Hidden;

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
        _gameOverPanel.style.visibility = Visibility.Visible;
        _gameOverLabel.text = "Game Over!\n\nYou traveled through " + _levelCount + " levels\n\nPress Space to start again";
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
