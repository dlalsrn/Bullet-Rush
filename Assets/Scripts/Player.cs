using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator animator;
    private ObjectManager objectManager;

    [SerializeField] private AudioClip itemClip; // Item 먹었을 때의 소리
    [SerializeField] private AudioClip deathClip; // 죽었을 때의 소리

    [SerializeField] private ObjectType objectType;

    private float xAxis;
    private float yAxis;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private int life = 3;

    [SerializeField] private GameObject[] followers;
    private const int MAXFOLLOWER = 2; // Follower는 최대 2개까지
    private int numOfFollower = 0;

    private ObjectType[] bulletTypes = new ObjectType[] { ObjectType.BulletPlayerA, ObjectType.BulletPlayerB };
    [SerializeField] private Transform firePos; // 총알 발사 위치
    [SerializeField] private float shootInterval = 0.2f; // 총알 발사 간격
    private float prevShootTime = 0f; // 마지막으로 총알을 발사한 시각

    private const int MAXPOWER = 5; // Power Item의 최대 효과는 5개까지
    private int numOfPower = 0; // 현재 먹은 Power Item의 개수, 3개 마다 총알 upgrade
    private int[][] numOfBullet; // numOfBullet[i] = i번째 파워를 먹었을 때 발사하는 총알의 Index들

    private const int MAXBOOM = 3; // Boom Item의 최대치
    private int numOfBoom = 0;
    
    private bool isTopReached = false; // Player가 화면 위쪽에 닿았는지
    private bool isLeftReached = false; // Player가 화면 왼쪽에 닿았는지
    private bool isRightReached = false; // Player가 화면 오른쪽에 닿았는지
    private bool isBottomReached = false; // Player가 화면 아래쪽에 닿았는지

    private bool isJoyControl; // Joy Panel을 누르고 있는지
    private bool[] joyControl = new bool[9]; // Joy Panel의 어디를 눌렀는지

    private bool isAttackButtonCliked; // Attack Button을 누르고 있는지

    private void Awake()
    {
        objectManager = FindObjectOfType<ObjectManager>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        InitNumOfBullet(); // Power에 따라 발사되는 총알의 개수 초기화
    }

    private void Update()
    {
        // 아직 플레이 준비가 안 됐다면
        if (!GameManager.Instance.IsReady)
        {
            animator.SetInteger("XAxis", 0);
            return;
        }

        Move();

        // 왼쪽 Shift를 누르거나 공격 Button을 누르고 있거나
        if (Input.GetKey(KeyCode.LeftShift) || isAttackButtonCliked)
        {
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseBoomItem();
        }
    }

    public void JoyPanel(int type)
    {
        for (int index = 0; index < 9; index++)
        {
            joyControl[index] = (index == type); // Joy Panel의 누르고 있는 부분을 true
        }
    }

    public void JoyDown()
    {
        isJoyControl = true;
    }

    public void JoyUp()
    {
        isJoyControl = false;
    }

    public void AttackButtonDown()
    {
        isAttackButtonCliked = true;
    }

    public void AttackButtonUp()
    {
        isAttackButtonCliked = false;
    }

    public void BoomButtonDown()
    {
        UseBoomItem();
    }

    private void Move()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");

        // JoyPanel로 플레이할 때
        if (isJoyControl)
        {
            for (int index = 0; index < 9; index++)
            {
                if (joyControl[index])
                {
                    // index의 값에 따라 방향 설정
                    yAxis = (index / 3 == 0 ? 1 : (index / 3 == 1 ? 0 : -1));
                    xAxis = (index % 3 == 0 ? -1 : (index % 3 == 1 ? 0 : 1));
                }
            }

            animator.SetInteger("XAxis", (int)xAxis);
        }
        else // 키보드로 플레이할 때
        {
            // 좌, 우 방향키를 눌렀거나 뗐을 때만 Set
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
            {
                animator.SetInteger("XAxis", (int)xAxis);
            }
        }

        // 위쪽, 아래쪽 벽에 닿았으면 더 넘어가지 못하게 처리
        if ((isTopReached && yAxis == 1) || (isBottomReached && yAxis == -1))
        {
            yAxis = 0;
        }
        
         // 왼쪽, 오른쪽 벽에 닿았으면 더 넘어가지 못하게 처리
        if ((isLeftReached && xAxis == -1) || (isRightReached && xAxis == 1))
        {
            xAxis = 0;
        }

        Vector3 dir = new Vector3(xAxis, yAxis, 0).normalized;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }

    private void Fire()
    {
        // 이전 총알을 발사한 시각이 ShootInterval 보다 크면 발사 가능
        // PC Version
        if (Time.time - prevShootTime > shootInterval)
        {
            CreateBullet();
            prevShootTime = Time.time;
        }
    }

    private void CreateBullet()
    {
        float adjustmentValue = 0.1f; // 총알 개수에 따른 위치 조절 값
        float bulletMargin = 0.2f; // 총알 사이의 간격
        int bullets = numOfBullet[numOfPower].Length;

        for (int i = 0; i < bullets; i++) // 현재 발사되는 총알 개수만큼 반복
        {
            // i번 째 총알의 생성 위치 조절
            int bulletIndex = numOfBullet[numOfPower][i]; // 현재 파워에서 발사되는 i번째 총알의 Prefab
            Vector3 spawnPos = new Vector3(firePos.position.x - (adjustmentValue * (bullets - 1)) + (bulletMargin * i), firePos.position.y, firePos.position.z);
            GameObject bullet = objectManager.MakeObject(bulletTypes[bulletIndex]);
            bullet.transform.position = spawnPos;
            bullet.transform.rotation = Quaternion.identity;
            bullet.GetComponent<Bullet>().Fire(Vector2.up);
        }
    }

    private void InitNumOfBullet()
    {
        // 각 배열은 해당 Index의 총알 Prefab을 발사한다는 뜻
        // ex) {0, 1, 0}은 일렬로 0번, 1번, 0번 총알 순으로 배치하여 발사한다는 의미 
        numOfBullet = new int[][]
        {
            new int[] { 0 }, // power = 1
            new int[] { 0, 0 }, // power = 2
            new int[] { 0, 0, 0 }, // power = 3
            new int[] { 0, 1, 0 }, // power = 4
            new int[] { 1, 0, 1 }, // power = 5
            new int[] { 1, 1, 1 }, // power = 6
        };
    }

    private void GetPowerItem()
    {
        // 총알 Upgrade가 최대치에 도달했다면 
        if (numOfPower == MAXPOWER)
        {
            AddFollower();
            GameManager.Instance.AddScore(500); // Power Item이 최대치면 대신 500점 증가
            return;
        }

        numOfPower++;
    }

    private void AddFollower()
    {
        if (numOfFollower == MAXFOLLOWER)
        {
            return;
        }

        // follower 활성화
        followers[numOfFollower++].SetActive(true);
    }
    
    private void GetBoomItem()
    {
        if (numOfBoom == MAXBOOM)
        {
            return;
        }

        numOfBoom++;
        HUDManager.Instance.SetBoomImage(numOfBoom);
    }

    private void UseBoomItem() // 범위 내에 있는 모든 적, 적의 총알 제거
    {
        // Boom Item이 있고 Space를 눌렀다면
        if (numOfBoom != 0)
        {
            numOfBoom--;
            GameObject boomEffect = objectManager.MakeObject(ObjectType.ItemBoomEffect);
            boomEffect.transform.position = transform.position;
            boomEffect.GetComponent<BoomEffect>().Boom();
            HUDManager.Instance.SetBoomImage(numOfBoom);
        }
    }

    private void GetItem(ItemType itemType)
    {
        SoundManager.Instance.PlaySoundClip(itemClip);
        switch (itemType)
        {
            case ItemType.Boom:
                GetBoomItem();
                break;
            case ItemType.Power:
                GetPowerItem();
                break;
            case ItemType.Coin:
                GameManager.Instance.AddScore(1000);
                break;
        }
    }

    private void Explosion()
    {
        GameObject explosion = objectManager.MakeObject(ObjectType.Explosion);
        explosion.transform.position = transform.position;
        explosion.GetComponent<Explosion>().StartExplosion(objectType);
    }

    private void OnHit()
    {
        if (!GameManager.Instance.IsReady)
        {
            return;
        }

        GameManager.Instance.SetIsReady(false);
        SoundManager.Instance.PlaySoundClip(deathClip);
        life--;
        Explosion();
        GameManager.Instance.RespawnPlayer(gameObject, life);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("PlayerBorder"))
        {
            if (collider.name == "Top")
            {
                isTopReached = true;
            }

            if (collider.name == "Left")
            {
                isLeftReached = true;
            }

            if (collider.name == "Right")
            {
                isRightReached = true;
            }

            if (collider.name == "Bottom")
            {
                isBottomReached = true;
            }
        }
        else if (collider.CompareTag("Enemy") || collider.CompareTag("EnemyBullet") || collider.CompareTag("Boss"))
        {
            OnHit();
        }
        else if (collider.CompareTag("Item"))
        {
            GetItem(collider.GetComponent<Item>().ItemType);
            collider.gameObject.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("PlayerBorder"))
        {
            if (collider.name == "Top")
            {
                isTopReached = false;
            }

            if (collider.name == "Left")
            {
                isLeftReached = false;
            }

            if (collider.name == "Right")
            {
                isRightReached = false;
            }

            if (collider.name == "Bottom")
            {
                isBottomReached = false;
            }
        }
    }
}
