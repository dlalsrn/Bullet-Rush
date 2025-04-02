using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EnemySpawner : MonoBehaviour
{
    private ObjectManager objectManager;

    private ObjectType[] enemyTypes = new ObjectType[] { ObjectType.EnemyS, ObjectType.EnemyM, ObjectType.EnemyL, ObjectType.EnemyBoss };
    [SerializeField] private Transform[] spawnPos;

    private List<Spawn> spawnList = new List<Spawn>();
    private int spawnIndex;
    private float spawnInterval;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        objectManager = FindObjectOfType<ObjectManager>();
    }

    public void ReadSpawnFile()
    {
        // Initialize
        spawnList.Clear();
        spawnIndex = 0;

        // Read
        TextAsset textFile = Resources.Load($"Stage {GameManager.Instance.CurrentStage}") as TextAsset;
        StringReader stringReader = new StringReader(textFile.text);

        while (stringReader != null)
        {
            string line = stringReader.ReadLine();

            if (line == "") // 빈 줄을 읽을 경우 끝
            {
                break;
            }

            string[] data = line.Split(','); // (Delay, 종류, 위치) 3가지 값이 존재
            Spawn spawnData = new Spawn(float.Parse(data[0]), data[1], int.Parse(data[2]));
            spawnList.Add(spawnData);
        }

        stringReader.Close();

        spawnInterval = spawnList[spawnIndex].Delay;
    }

    public void StartSpawnEnemy()
    {
        spawnCoroutine = StartCoroutine(SpawnEnemyRoutine());
    }

    private IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(spawnInterval);

        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        int enemyIndex;
        string enemyType = spawnList[spawnIndex].Type;
        int pointIndex = spawnList[spawnIndex].Point;
        
        if (enemyType == "S")
        {
            enemyIndex = 0;
        }
        else if (enemyType == "M")
        {
            enemyIndex = 1;
        }
        else if (enemyType == "L")
        {
            enemyIndex = 2;
        }
        else if (enemyType == "B")
        {
            enemyIndex = 3;
        }
        else // 잘못된 Type
        {
            return;
        }

        GameObject enemyObject = objectManager.MakeObject(enemyTypes[enemyIndex]);
        enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f); // 회전 값 초기화
        enemyObject.transform.position = spawnPos[pointIndex].position; // 위치 설정

        if (enemyType == "B") // 보스는 다르게 설정
        {
            Boss boss = enemyObject.GetComponent<Boss>();
            boss.Init();
            boss.Move();
        }
        else
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();
            enemy.Init();
            enemy.Move();
        }

        // SpawnIndex 증가
        spawnIndex++;
        if (spawnIndex == spawnList.Count)
        {
            StopSpawnEnemy();
            return;
        }
        else
        {
            spawnInterval = spawnList[spawnIndex].Delay;
        }
    }

    public void StopSpawnEnemy()
    {
        StopCoroutine(spawnCoroutine);
    }
}
