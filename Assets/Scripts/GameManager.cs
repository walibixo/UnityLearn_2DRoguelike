using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private UIDocument _uiDocument;
    private Label _foodLabel;
    private VisualElement _gameOverPanel;
    private Label _gameOverLabel;

    public PlayerController PlayerController { get; private set; }
    public BoardManager BoardManager { get; private set; }
    public TurnManager TurnManager { get; private set; }
    public SoundManager SoundManager { get; private set; }
    public ScreenTransition ScreenTransition { get; private set; }

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

        PlayerController = FindFirstObjectByType<PlayerController>();
        BoardManager = FindFirstObjectByType<BoardManager>();
        TurnManager = FindFirstObjectByType<TurnManager>();
        SoundManager = FindFirstObjectByType<SoundManager>();
        ScreenTransition = FindFirstObjectByType<ScreenTransition>();

        _uiDocument = FindFirstObjectByType<UIDocument>();
        _foodLabel = _uiDocument.rootVisualElement.Q<Label>("FoodLabel");
        _gameOverPanel = _uiDocument.rootVisualElement.Q<VisualElement>("GameOverPanel");
        _gameOverLabel = _gameOverPanel.Q<Label>("GameOverLabel");
    }

    void Start()
    {
        TurnManager.OnStartPlayerTurn += OnNewTurn;

        StartNewGame();
    }

    public void StartNewGame()
    {
        LoadSavedGameState();
        IsGameOver = false;

        _gameOverPanel.style.visibility = Visibility.Hidden;

        UpdateFoodAmount();

        StartNewLevel();
    }

    public void StartNewLevel()
    {
        ScreenTransition.HideScreen();

        SaveGameState();

        _levelCount++;

        BoardManager.ClearBoard();
        BoardManager.GenerateBoard(_levelCount);

        PlayerController.Spawn(BoardManager.PlayerStart);

        TurnManager.Start();

        ScreenTransition.ShowScreen();
    }

    public void GameOver()
    {
        IsGameOver = true;
        _gameOverPanel.style.visibility = Visibility.Visible;
        _gameOverLabel.text = "Game Over!\n\nYou traveled through " + _levelCount + " levels\n\nPress Space to start again";

        ClearGameState();
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
        _foodLabel.text = "Food : " + _foodAmount.ToString().Replace('9', 'P').Replace('8', 'B');
    }

    private void SaveGameState()
    {
        PlayerPrefs.SetInt("LevelCount", _levelCount);
        PlayerPrefs.SetInt("FoodAmount", _foodAmount);
        PlayerPrefs.Save();
    }

    private void ClearGameState()
    {
        PlayerPrefs.DeleteKey("LevelCount");
        PlayerPrefs.DeleteKey("FoodAmount");
    }

    private void LoadSavedGameState()
    {
        _levelCount = PlayerPrefs.GetInt("LevelCount", 0);
        _foodAmount = PlayerPrefs.GetInt("FoodAmount", 100);
    }
}
