using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("Scenes/GameScene");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
