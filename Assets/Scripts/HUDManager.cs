using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image[] lifeImage;
    [SerializeField] private Image[] boomImage;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText;

    [SerializeField] private TextMeshProUGUI stageStartText;
    [SerializeField] private Animator stageStartAnimator;

    [SerializeField] private TextMeshProUGUI stageClearText;
    [SerializeField] private Animator stageClearAnimator;

    [SerializeField] private Animator fadeAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetScoreText(int score)
    {
        scoreText.SetText(string.Format("{0:n0}", score));
    }

    public void ShowGameOverPanel(bool result)
    {
        resultText.text = (result ? "YOU WIN!" : "YOU LOSE!");
        gameOverPanel.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);
    }

    public void SetLifeImage(int life)
    {
        for (int i = 0; i < lifeImage.Length; i++)
        {
            lifeImage[i].enabled = (i < life ? true : false);
        }
    }

    public void SetBoomImage(int numOfBoom)
    {
        for (int i = 0; i < boomImage.Length; i++)
        {
            boomImage[i].enabled = (i < numOfBoom ? true : false);
        }
    }

    public void ShowStageStartText(int currentStage)
    {
        stageStartText.text = $"Stage {currentStage}\nStart";
        stageStartAnimator.SetTrigger("OnStage");
    }

    public void ShowStageClearText(int currentStage)
    {
        stageClearText.text = $"Stage {currentStage}\nClear!!!";
        stageClearAnimator.SetTrigger("OnStage");
    }

    public void ShowFadeInOut(bool on)
    {
        fadeAnimator.SetTrigger((on ? "FadeIn" : "FadeOut"));
    }
}
