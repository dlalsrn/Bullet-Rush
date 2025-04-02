using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Player,
    EnemyS,
    EnemyM,
    EnemyL,
    EnemyBoss,
    ItemCoin,
    ItemPower,
    ItemBoom,
    ItemBoomEffect,
    BulletPlayerA,
    BulletPlayerB,
    BulletEnemyA,
    BulletEnemyB,
    BulletFollower,
    BulletBossA,
    BulletBossB,
    Explosion,
}

public class ObjectManager : MonoBehaviour
{
    private Dictionary<ObjectType, GameObject[]> objectPools = new Dictionary<ObjectType, GameObject[]>();

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject[] itemPrefabs;
    [SerializeField] private GameObject[] bulletPrefabs;
    [SerializeField] private GameObject explosionPrefab;

    private GameObject[] targetPool; // 특정 Type의 Obejct Pool을 담을 배열

    [SerializeField] private Transform enemyPoolParent; // 생성된 Enemy Pool Object를 한 곳에 모아둘 부모 Object
    [SerializeField] private Transform itemPoolParent; // 생성된 Item Pool Object를 한 곳에 모아둘 부모 Object
    [SerializeField] private Transform bulletPoolParent; // 생성된 Bullet Pool Object를 한 곳에 모아둘 부모 Object
    [SerializeField] private Transform explosionPoolParent; // 생성된 Explosion Pool Object를 한 곳에 모아둘 부모 Object

    private void Awake()
    {
        Generate();

        // 큰 프로젝트에서는 위 배열을 만들기 위한 시간을 벌기위해 로딩을 추가함
    }

    private void Generate()
    {
        // Enemy
        objectPools[ObjectType.EnemyS] = CreatePool(enemyPrefabs[0], 20, enemyPoolParent);
        objectPools[ObjectType.EnemyM] = CreatePool(enemyPrefabs[1], 10, enemyPoolParent);
        objectPools[ObjectType.EnemyL] = CreatePool(enemyPrefabs[2], 10, enemyPoolParent);
        objectPools[ObjectType.EnemyBoss] = CreatePool(enemyPrefabs[3], 1, enemyPoolParent);

        // Item
        objectPools[ObjectType.ItemCoin] = CreatePool(itemPrefabs[0], 20, itemPoolParent);
        objectPools[ObjectType.ItemPower] = CreatePool(itemPrefabs[1], 10, itemPoolParent);
        objectPools[ObjectType.ItemBoom] = CreatePool(itemPrefabs[2], 10, itemPoolParent);
        objectPools[ObjectType.ItemBoomEffect] = CreatePool(itemPrefabs[3], 10, itemPoolParent);

        // Bullet
        objectPools[ObjectType.BulletPlayerA] = CreatePool(bulletPrefabs[0], 100, bulletPoolParent);
        objectPools[ObjectType.BulletPlayerB] = CreatePool(bulletPrefabs[1], 100, bulletPoolParent);
        objectPools[ObjectType.BulletEnemyA] = CreatePool(bulletPrefabs[2], 100, bulletPoolParent);
        objectPools[ObjectType.BulletEnemyB] = CreatePool(bulletPrefabs[3], 300, bulletPoolParent);
        objectPools[ObjectType.BulletFollower] = CreatePool(bulletPrefabs[4], 100, bulletPoolParent);
        objectPools[ObjectType.BulletBossA] = CreatePool(bulletPrefabs[5], 200, bulletPoolParent);
        objectPools[ObjectType.BulletBossB] = CreatePool(bulletPrefabs[6], 200, bulletPoolParent);

        // Explosion
        objectPools[ObjectType.Explosion] = CreatePool(explosionPrefab, 100, explosionPoolParent);
    }

    // 특정 Prefab을 poolSize만큼 만든 배열을 return. parent 자식으로 생성하여 관리 용이.
    private GameObject[] CreatePool(GameObject prefab, int poolSize, Transform parent = null)
    {
        GameObject[] pool = new GameObject[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(prefab, parent);
            pool[i].SetActive(false);
        }
        return pool;
    }

    // 생성하고 싶은 Object의 Type을 인자로 받음
    // 해당 Type의 Object 풀에서 비활성화 상태인 Obejct 하나를 return
    public GameObject MakeObject(ObjectType objType)
    {
        // 인자로 받은 objType이 Key인 GameObject 배열이 존재하는지
        if (objectPools.ContainsKey(objType))
        {
            targetPool = objectPools[objType];

            foreach (GameObject obj in targetPool)
            {
                if (!obj.activeSelf) // 특정 Object가 비활성화 상태면 return
                {
                    obj.SetActive(true);
                    return obj;
                }
            }
        }

        return null; // 모두 활성화 상태이면 null return
    }
}
