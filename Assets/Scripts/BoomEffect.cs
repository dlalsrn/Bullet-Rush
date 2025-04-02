using UnityEngine;

public class BoomEffect : MonoBehaviour
{
    [SerializeField] private AudioClip boomClip;

    private int boomDamage = 100;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // 폭탄 범위 내에 있는 적에게 큰 데미지를 줌. 적의 총알도 삭제
        if (collider.CompareTag("Enemy"))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            enemy.OnHit(boomDamage);
        }
        else if (collider.CompareTag("Boss"))
        {
            Boss boss = collider.GetComponent<Boss>();
            boss.OnHit(boomDamage);
        }
        else if (collider.CompareTag("EnemyBullet"))
        {
            collider.gameObject.SetActive(false);
        }
    }

    public void Boom()
    {
        SoundManager.Instance.PlaySoundClip(boomClip);
        Invoke("HideBoomEffect", 1f);
    }

    private void HideBoomEffect()
    {
        gameObject.SetActive(false);
    }
}
