using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class MainMenu : MonoBehaviour
{
    public void LoadEasyLevel()
    {
        // SceneLoader.Instance.LoadScene(SceneType.Easy);
        SceneManager.LoadScene("Easy");
    }

    public void LoadMediumLevel()
    {
        // SceneLoader.Instance.LoadScene(SceneType.Easy);
        SceneManager.LoadScene("Medium");
    }

    public void LoadHardLevel()
    {
        // SceneLoader.Instance.LoadScene(SceneType.Easy);
        SceneManager.LoadScene("Hard");
    }
}
