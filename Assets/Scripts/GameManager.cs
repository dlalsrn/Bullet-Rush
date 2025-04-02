using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private EnemySpawner enemySpawner;

    private Vector3 originPlayerPos = new Vector3(0f, -4f, 0f);
    private int blinkTime = 10; // 점멸 횟수
    private const int PLAYER_ORIGIN = 6;
    private const int PLAYER_RESPAWN = 7;

    public int Score { get; private set; }
    public int CurrentStage { get; private set; } = 1;
    public bool IsReady { get; private set; } = true; // Player 조작이 가능한지
    private bool isGameClear = false;

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

        enemySpawner = FindObjectOfType<EnemySpawner>();

        StageStart();
    }

    private void Start()
    {
        Score = 0;
    }

    public void RespawnPlayer(GameObject player, int life)
    {
        player.SetActive(false);
        HUDManager.Instance.SetLifeImage(life);
        
        if (life == 0) // 목숨을 모두 잃었다면
        {
            Destroy(player);
            GameOver();
        }
        else // 목숨이 남았다면 Respawn
        {
            StartCoroutine(PlayerSpawnRoutine(player));
        }
    }

    private IEnumerator PlayerSpawnRoutine(GameObject player)
    {
        yield return new WaitForSeconds(2f);

        player.transform.position = originPlayerPos;
        player.layer = PLAYER_RESPAWN; // Respawn Layer로 변경하여 적, 적 총알과의 충돌 무시 
        player.SetActive(true); // Player Respawn
        IsReady = true;
        
        for (int i = 0; i < blinkTime; i++) // 0.4 x 5 = 2초 정도 Blink, Blink 중에는 무적
        {
            player.GetComponent<SpriteRenderer>().enabled = false;
            yield return new WaitForSeconds(0.1f);
            player.GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        player.layer = PLAYER_ORIGIN; // 다시 충돌 가능하게 변경
    }

    public void AddScore(int score)
    {
        Score += score;
        HUDManager.Instance.SetScoreText(Score);
    }

    private void GameOver()
    {
        Invoke("ShowGameOverPanel", 2f);

        if (enemySpawner != null)
        {
            enemySpawner.StopSpawnEnemy();
        }
    }

    public void StageStart()
    {
        // Stage Start UI Load
        HUDManager.Instance.ShowStageStartText(CurrentStage);

        // Enemy Spawn
        enemySpawner.ReadSpawnFile();
        enemySpawner.StartSpawnEnemy();

        // Fade In
        HUDManager.Instance.ShowFadeInOut(true);

        IsReady = true;
    }

    public void StageEnd()
    {
        if (IsReady)
        {
            IsReady = false;

            HUDManager.Instance.ShowStageClearText(CurrentStage);

            // Stage Increase
            CurrentStage++;

            // Next Stage Start
            if (CurrentStage == 3) // 모든 Stage Clear
            {
                isGameClear = true;
                Invoke("GameOver", 3f);
            }
            else
            {
                // Fade Out
                HUDManager.Instance.ShowFadeInOut(false);

                // Player Position Set
                GameObject player = FindObjectOfType<Player>().gameObject;
                player.transform.position = originPlayerPos;

                Invoke("StageStart", 5f);
            }
        }
    }

    public void SetIsReady(bool isReady)
    {
        IsReady = isReady;
    }

    private void ShowGameOverPanel()
    {
        HUDManager.Instance.ShowGameOverPanel(isGameClear);
    }

    public void GameStart()
    {
        SceneManager.LoadScene("Scenes/GameScene");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Scenes/MenuScene");
    }
}