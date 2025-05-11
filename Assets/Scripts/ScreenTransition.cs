using System.Collections;
using UnityEngine;

public class ScreenTransition : MonoBehaviour
{
    [SerializeField]
    private Animator _transition;

    public void HideScreen()
    {
        _transition.SetTrigger("Start");
    }

    public void ShowScreen()
    {
        _transition.SetTrigger("End");
    }

    public void LoadScene(int sceneIndex)
    {
        PlayTransition(sceneIndex);
    }

    private void PlayTransition(int? sceneIndex = null)
    {
        StartCoroutine(RunTransitionCoroutine(sceneIndex));

        IEnumerator RunTransitionCoroutine(int? sceneIndex = null)
        {
            _transition.SetTrigger("Start");

            yield return new WaitForSeconds(1.0f);

            if (sceneIndex.HasValue)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex.Value);
            }

            _transition.SetTrigger("End");
        }
    }
}
