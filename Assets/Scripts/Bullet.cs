using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int bulletDamage;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private bool isRotate;
    
    [SerializeField] private AudioClip shootClip;

    private float rotateSpeed = 120f;

    private void Update()
    {
        if (isRotate)
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }

    public void Fire(Vector2 fireDir)
    {
        SoundManager.Instance.PlaySoundClip(shootClip);
        GetComponent<Rigidbody2D>().velocity = fireDir * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("DestroyBorder")) // 화면 밖으로 벗어나면 Destroy
        {
            gameObject.SetActive(false);
        }
        else if (collider.CompareTag("Enemy"))
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            enemy.OnHit(bulletDamage);
            gameObject.SetActive(false);
        }
        else if (collider.CompareTag("Boss"))
        {
            Boss boss = collider.GetComponent<Boss>();
            boss.OnHit(bulletDamage);
            gameObject.SetActive(false);
        }
    }
}
