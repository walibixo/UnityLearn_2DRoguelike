using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUIHandler : MonoBehaviour
{
    private ScreenTransition _screenTransition;

    private void Awake()
    {
        _screenTransition = FindFirstObjectByType<ScreenTransition>();
    }

    public void StartGame()
    {
        _screenTransition.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
