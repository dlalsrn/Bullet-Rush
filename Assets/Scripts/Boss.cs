using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private ObjectManager objectManager;

    [SerializeField] private AudioClip deathClip; // 죽었을 때의 소리

    [SerializeField] private ObjectType objectType;

    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    [SerializeField] private int score;
    private bool isDeath = false;

    [SerializeField] private float amplitude = 1.7f; // 움직이는 폭
    [SerializeField] private float moveSpeed = 1.5f; // 움직이는 속도

    [SerializeField] private Transform[] firePos;
    private int numOfPattern = 5; // 패턴 개수
    [SerializeField] private int patternIndex = -1;
    private int currentPatternCount;
    private int[] maxPatternCount = new int[] { 10, 10, 100, 50, 10 };

    private Coroutine patternCoroutine = null;
    private Coroutine movingCoroutine = null;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        objectManager = FindObjectOfType<ObjectManager>();
    }

    public void Init()
    {
        currentHp = maxHp;
        isDeath = false;
        patternIndex = -1;
        currentPatternCount = 0;
        Invoke("Stop", 3.5f); // 보스가 등장 후 2초 뒤에 정지
    }

    public void Move()
    {
        rigidbody2d.velocity = Vector2.down * 1f;
    }

    private void Stop()
    {
        rigidbody2d.velocity = Vector2.zero;

        StartMoving(); // 좌우 무빙 시작
        StartPattern();
    }

    private void StartMoving()
    {
        movingCoroutine = StartCoroutine(MovingRoutine());
    }

    IEnumerator MovingRoutine()
    {
        yield return new WaitForSeconds(2f);
        float offset = 0f;

        while (true)
        {
            // 좌, 우 무빙
            offset += Time.deltaTime * moveSpeed; // 원점부터 부드럽게 움직이기 위해 deltaTime * moveSpeed만큼 더함
            float posX = Mathf.Sin(offset) * amplitude; // Sin 함수로 X 좌표 생성
            transform.position = new Vector3(posX, transform.position.y, transform.position.z);
            yield return null;
        }
    }

    private void StopMoving()
    {
        StopCoroutine(movingCoroutine);
    }

    private void StartPattern()
    {
        patternCoroutine = StartCoroutine(PatternRoutine());
    }

    private void StopPattern()
    {
        StopCoroutine(patternCoroutine);
    }

    IEnumerator PatternRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            patternIndex = (patternIndex == numOfPattern - 1 ? 0 : patternIndex + 1); // 1 ~ 5 패턴 반복
            currentPatternCount = 0;

            while (currentPatternCount < maxPatternCount[patternIndex])
            {
                if (patternIndex == 0)
                {
                    FireFoward();
                    yield return new WaitForSeconds(1f);
                }
                else if (patternIndex == 1)
                {
                    FireShot();
                    yield return new WaitForSeconds(1f);
                }
                else if (patternIndex == 2)
                {
                    FireWave();
                    yield return new WaitForSeconds(0.1f);
                }
                else if (patternIndex == 3)
                {
                    FireRandom();
                    yield return new WaitForSeconds(0.2f);
                }
                else if (patternIndex == 4)
                {
                    FireAround();
                    yield return new WaitForSeconds(1f);
                }

                currentPatternCount++;
            }

            yield return new WaitForSeconds(3f);
        }
    }

    // 앞으로 4발 발사
    private void FireFoward()
    {
        for (int index = 0; index < firePos.Length; index++)
        {
            // Bullet 생성 후 위치 조정
            GameObject bullet = objectManager.MakeObject(ObjectType.BulletBossA);
            bullet.transform.position = firePos[index].position;
            bullet.transform.rotation = Quaternion.identity;

            bullet.GetComponent<Bullet>().Fire(Vector2.down);
        }
    }

    // 플레이어 방향으로 샷건처럼 퍼지면서 여러 발 발사
    private void FireShot()
    {
        for (int index = 0; index < firePos.Length; index++)
        {
            Player player = FindObjectOfType<Player>();

            if (player != null)
            {
                // Bullet 생성 후 위치 조정
                GameObject bullet = objectManager.MakeObject(ObjectType.BulletEnemyB);
                bullet.transform.position = firePos[index].position;
                bullet.transform.rotation = Quaternion.identity;

                // Player 쪽으로 총알이 날라가게 방향 조정
                Vector3 fireDir = player.transform.position - bullet.transform.position;
                Vector3 randomVec = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0f);
                fireDir += randomVec;
                fireDir = fireDir.normalized;

                bullet.GetComponent<Bullet>().Fire(fireDir);
            }
        }
    }

    // Wave를 따라 움직이도록 발사
    private void FireWave()
    {
        // 맨 끝 2개 총구에서만 발사
        // Bullet 생성 후 위치 조정
        GameObject bulletL = objectManager.MakeObject(ObjectType.BulletBossB);
        bulletL.transform.position = firePos[0].position;
        bulletL.transform.rotation = Quaternion.identity;

        GameObject bulletR = objectManager.MakeObject(ObjectType.BulletBossB);
        bulletR.transform.position = firePos[3].position;
        bulletR.transform.rotation = Quaternion.identity;

        bulletL.GetComponent<Bullet>().Fire(Vector2.down);
        bulletR.GetComponent<Bullet>().Fire(Vector2.down);
    }

    // 랜덤 방향으로 발사
    private void FireRandom() 
    {
        // 맨 끝 2개 총구에서만 발사
        // Bullet 생성 후 위치 조정
        GameObject bulletL = objectManager.MakeObject(ObjectType.BulletEnemyA);
        bulletL.transform.position = firePos[0].position;
        bulletL.transform.rotation = Quaternion.identity;

        GameObject bulletR = objectManager.MakeObject(ObjectType.BulletEnemyA);
        bulletR.transform.position = firePos[3].position;
        bulletR.transform.rotation = Quaternion.identity;

        // 랜덤하게 방향 설정
        Vector3 fireDirL = new Vector3(Mathf.Cos(Random.Range(0, Mathf.PI)), -1f);
        fireDirL = fireDirL.normalized;
        Vector3 fireDirR = new Vector3(Mathf.Cos(Random.Range(0, Mathf.PI)), -1f);
        fireDirR = fireDirR.normalized;

        // 회전 값 설정
        bulletL.GetComponent<Rigidbody2D>().angularVelocity = 360f;
        bulletR.GetComponent<Rigidbody2D>().angularVelocity = 360f;

        bulletL.GetComponent<Bullet>().Fire(fireDirL);
        bulletR.GetComponent<Bullet>().Fire(fireDirR);
    }

    // 원형으로 발사
    private void FireAround()
    {
        int numOfTime = 40;

        for (int index = 0; index < numOfTime; index++)
        {
            // Bullet 생성 후 위치 조정
            GameObject bullet = objectManager.MakeObject(ObjectType.BulletBossB);
            bullet.transform.position = (firePos[1].position + firePos[2].position) / 2;
            bullet.transform.rotation = Quaternion.identity;

            Vector3 fireDir = new Vector3(Mathf.Cos(Mathf.PI * 2 * index / numOfTime), Mathf.Sin(Mathf.PI * 2 * index / numOfTime), 0f);
            fireDir = fireDir.normalized;

            // Bullet이 날아가는 방향을 바라보도록 각도 조정
            float angle = Mathf.Atan2(fireDir.y, fireDir.x) * 180f / Mathf.PI + 90f; // 90도 축 보정
            bullet.transform.Rotate(Vector3.forward * angle);

            bullet.GetComponent<Bullet>().Fire(fireDir);
        }
    }

    private void Explosion()
    {
        GameObject explosion = objectManager.MakeObject(ObjectType.Explosion);
        explosion.transform.position = transform.position;
        explosion.GetComponent<Explosion>().StartExplosion(objectType);
    }

    public void OnHit(int damage) // 총알에 맞았을 때
    {
        currentHp -= damage;

        if (currentHp <= 0 && !isDeath) // hp가 0이하고 아직 안 죽었다면
        {
            SoundManager.Instance.PlaySoundClip(deathClip);

            CancelInvoke("Stop");

            isDeath = true; // 총알을 한 번에 여러 발 맞았을 때 Item이 여러 개 떨어지는 것을 방지
            GameManager.Instance.AddScore(score);

            if (movingCoroutine != null)
            {
                StopCoroutine(movingCoroutine);
            }
            if (patternCoroutine != null)
            {
                StopCoroutine(patternCoroutine);
            }

            gameObject.SetActive(false);

            Explosion();

            GameManager.Instance.StageEnd();
        }
        else // HP가 남아있다면 피격 효과
        {
            animator.SetTrigger("OnHit");
        }
    }
}
