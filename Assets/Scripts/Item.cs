using UnityEngine;

public enum ItemType
{
    Boom,
    Power,
    Coin
}

public class Item : MonoBehaviour
{
    [field:SerializeField] public ItemType ItemType { get; private set; }

    private Rigidbody2D rigidbody2d;
    private float moveSpeed = 2f;

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void Init()
    {
        rigidbody2d.velocity = Vector2.down * moveSpeed;
    }
}
