using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Follower : MonoBehaviour
{
    private ObjectManager objectManager;

    [SerializeField] private Transform firePos; // 총알 발사 위치
    [SerializeField] private float shootInterval = 0.2f; // 총알 발사 간격
    private float prevShootTime = 0f; // 마지막으로 총알을 발사한 시각

    private Vector3 followPos;
    [SerializeField] private int followDelay = 12;
    [SerializeField] private Transform target; // Target Object의 위치
    private Queue<Vector3> targetPos = new Queue<Vector3>();

    private void Awake()
    {
        objectManager = FindObjectOfType<ObjectManager>();
    }

    private void OnEnable()
    {
        transform.position = target.position; // 시작은 Target의 위치로 이동
    }

    private void Update()
    {
        Follow();
        Fire();
    }

    // Target Object를 따라가는 메소드
    private void Follow()
    {
       // Target이 움직였을 때만 Enqueue, 가만히 있으면 Enqueue X
        if ((targetPos.Count != 0) && (targetPos.Last() == target.position))
        {
            return;
        }

        targetPos.Enqueue(target.position);
        // Target의 Delay 만큼의 이전 프레임 위치로 이동
        // 즉, Delay가 12라고 가정할 때 Target이 1 프레임 때 A 위치에 있었다면, Follower는 13 프레임에 A 위치로 이동함
        if (targetPos.Count > followDelay)
        {
            transform.position = targetPos.Dequeue();
        }
    }

     // Bullet 발사 메소드
    private void Fire()
    {
        // 이전 총알을 발사한 시각이 ShootInterval 보다 크면 발사 가능
        if (Time.time - prevShootTime > shootInterval)
        {
            CreateBullet();
            prevShootTime = Time.time;
        }
    }

    private void CreateBullet()
    {
        GameObject bullet = objectManager.MakeObject(ObjectType.BulletFollower);
        bullet.transform.position = firePos.position;
        bullet.GetComponent<Bullet>().Fire(Vector2.up);
    }
}
