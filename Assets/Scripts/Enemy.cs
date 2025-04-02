using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;
    private ObjectManager objectManager;

    [SerializeField] private AudioClip deathClip; // 죽었을 때의 소리

    [SerializeField] private ObjectType objectType; // 자신의 Object Type

    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;
    [SerializeField] private int score;
    private bool isDeath = false;

    private float posY = 5f; // posY보다 position 값이 높으면 아래로 움직임
    [SerializeField] private float moveSpeed;

    private ObjectType[] bulletTypes = new ObjectType[] { ObjectType.BulletEnemyA, ObjectType.BulletEnemyB };
    [SerializeField] private Transform firePos; // 총알 발사 위치
    private int bulletIndex = 0; // 사용하는 총알 번호
    [SerializeField] private float shootInterval = 1.5f; // 총알 발사 간격
    private float prevShootTime = 0f; // 마지막으로 총알을 발사한 시각
    private float maxShootPosY = -5f; // 화면 Y좌표의 맨 아래. 더 아래로 나가면 Shoot X
    [SerializeField] private Sprite originSprite; // 원래 상태의 Sprite
    [SerializeField] private Sprite damagedSprite; // 공격 받았을 때의 Sprite
    
    private ObjectType[] itemTypes = new ObjectType[] { ObjectType.ItemCoin, ObjectType.ItemPower, ObjectType.ItemBoom };
    private float[] itemWeights = {0.2f, 0.2f, 0.2f}; // 각 Item의 drop 확률 (Coin, Power, Boom)

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        objectManager = FindObjectOfType<ObjectManager>();
    }

    private void Update()
    {
        Fire();
    }

    public void Init() // 기본값 설정
    {
        currentHp = maxHp;
        isDeath = false;
    }

    public void OnHit(int damage) // 총알에 맞았을 때
    {
        currentHp -= damage;

        if (currentHp <= 0 && !isDeath) // hp가 0이하고 아직 안 죽었다면
        {
            SoundManager.Instance.PlaySoundClip(deathClip);
            isDeath = true; // 총알을 한 번에 여러 발 맞았을 때 Item이 여러 개 떨어지는 것을 방지
            GameManager.Instance.AddScore(score);
            DropItem(); // 일정 확률로 Random Item Drop
            gameObject.SetActive(false);

            Explosion();
        }
        else // HP가 남아있다면 피격 효과
        {
            CancelInvoke("ResetSprite");
            spriteRenderer.sprite = damagedSprite;
            Invoke("ResetSprite", 0.1f);
        }
    }

    private void ResetSprite() // 원래 Sprite로 복구
    {
        spriteRenderer.sprite = originSprite;
    }

    public void Move() // 어느 방향으로 이동할지 정해주는 함수
    {
        if (transform.position.y >= posY) // 화면 위쪽에서 Spawn되는 Enemy들
        {
            rigidbody2d.velocity = Vector2.down * moveSpeed;
        }
        else // 화면 왼, 오른쪽에서 Spawn되는 Enemy들
        {
            rigidbody2d.velocity = new Vector2((transform.position.x > 0 ? -1f : 1f) * moveSpeed, -1);
            transform.Rotate((transform.position.x > 0 ? Vector3.back : Vector3.forward) * 90f);
        }
    }

    private void Fire()
    {
        // 이전 총알을 발사한 시각이 ShootInterval 보다 크면 발사 가능
        if ((Time.time - prevShootTime > shootInterval))
        {
            CreateBullet();
            prevShootTime = Time.time;
        }
    }

    private void CreateBullet()
    {
        // 화면 아래로 나가면 안 보이는 곳에서 발사하는 것을 방지
        if (transform.position.y < maxShootPosY)
        {
            return;
        }

        if (objectType == ObjectType.EnemyS)
        {
            bulletIndex = 0;
        }
        else if (objectType == ObjectType.EnemyL)
        {
            bulletIndex = 1;
        }
        else
        {
            return;
        }

        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            GameObject bullet = objectManager.MakeObject(bulletTypes[bulletIndex]);
            bullet.transform.position = firePos.position;
            bullet.transform.rotation = Quaternion.identity;
            Vector3 fireDir = player.transform.position - bullet.transform.position; // Player 쪽으로 총알이 날라오게 방향 설정
            fireDir = fireDir.normalized;
            bullet.GetComponent<Bullet>().Fire(fireDir);
        }
    }

    private void DropItem()
    {
        int itemIndex = GetWeightRandomIndex();
        if (itemIndex == -1) // Item Drop X
        {
            return;
        }
        GameObject item = objectManager.MakeObject(itemTypes[itemIndex]);
        item.transform.position = transform.position;
        item.transform.rotation = Quaternion.identity;
        item.GetComponent<Item>().Init(); // 초기 상태 설정
    }

    private int GetWeightRandomIndex() { // 각 Item의 확률 구간에서 랜덤한 지점에 해당하는 Item이 무엇인지 return
        int randomIndex = Random.Range(0, 100); // 모든 확률 구간 중 랜덤 Index 설정
        int sum = 0;

        for (int i = 0; i < itemWeights.Length; i++) {
            sum += (int)(itemWeights[i] * 100f);
            if (randomIndex < sum) { // 랜덤 Index가 해당 Item 확률 구간에 있는지
                return i;
            }
        }

        return -1; // Item 중 아무것도 뽑히지 않았을 경우 Drop X
    }

    private void Explosion()
    {
        GameObject explosion = objectManager.MakeObject(ObjectType.Explosion);
        explosion.transform.position = transform.position;
        explosion.GetComponent<Explosion>().StartExplosion(objectType);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("DestroyBorder")) // 화면 밖으로 벗어나면 Destroy
        {
            gameObject.SetActive(false);
        }
    }
}
