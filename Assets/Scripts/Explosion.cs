using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator animator;

    private float explosionScale;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartExplosion(ObjectType objType)
    {
        switch (objType)
        {
            case ObjectType.EnemyS:
                explosionScale = 0.7f;
                break;
            case ObjectType.EnemyM:
            case ObjectType.Player:
                explosionScale = 1f;
                break;
            case ObjectType.EnemyL:
                explosionScale = 2f;
                break;
            case ObjectType.EnemyBoss:
                explosionScale = 3f;
                break; 
        }

        transform.localScale = Vector3.one * explosionScale;
        animator.SetTrigger("OnExplosion");

        Invoke("Deactivation", 0.25f); // 0.5초의 폭발 시간 대기
    }

    private void Deactivation()
    {
        gameObject.SetActive(false);
    }
}
