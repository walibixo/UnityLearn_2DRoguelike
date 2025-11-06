using System;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

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

    private readonly string[] _survivalLogs = new[]
    {
        "Only the hope of others keeps me moving N.E..",
        "Every dawn means one more step towards the North-East.",
        "They say the sun still shines N.E. I have to see it.",
        "The map is clear. The survivors are North-East.",
        "I won't stop until I see their fires in the N.E. distance.",
        "This journey ends in the North-East, one way or another.",
        "The road N.E. is a graveyard. I pray it's not mine.",
        "Just one more day of walking. Just one more, heading N.E..",
        "I can almost smell the smoke from the N.E. encampment.",
        "My legs ache, but my hope is set on the North-East.",
        "Another day, another horde, still pushing for the N.E..",
        "The voices of the N.E. survivors call me forward.",
        "Finding them is the only reason to keep fighting this far N.E..",
        "Maybe they have medicine N.E. Maybe they have answers.",
        "This entire trek N.E. is based on a whisper. A deadly whisper.",
        "I follow a dead rumor N.E. It's all I have left.",
        "If I stop, the horde wins. N.E. is the only escape.",
        "I'm one cut away from becoming one of them. Press N.E..",
        "They said 'go N.E.' like it was easy. It's a lie, but I obey.",
        "Don't look back. Just keep moving N.E..",
        "I'm tired of walking. I must be close to the N.E. now."
    };

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
        _foodLabel.text = $"Survival Log: Day {FixMissingFontNumbers(_levelCount)}{Environment.NewLine}Rations Left: {FixMissingFontNumbers(_foodAmount)}{Environment.NewLine}{_survivalLogs[_levelCount % _survivalLogs.Length]}";
    }

    private string FixMissingFontNumbers(object input)
    {
        return input.ToString().Replace('9', 'P').Replace('8', 'B');
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
