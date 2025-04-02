using UnityEngine;

public class Background : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private float moveSpeed;
    private float height; // sprite의 세로 길이
    private float moveDistance; // 움직인 거리
    private Vector3 startPos; // 해당 Sprite의 처음 위치

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        height = spriteRenderer.bounds.size.y;
    }

    private void Update()
    {
        // 0 ~ sprite의 y 값 사이를 반복
        // moveDistance는 moveSpeed를 기준으로 한 프레임동안 움직인 거리 
        moveDistance = Mathf.Repeat(Time.time * moveSpeed, height);

        // 처음 위치에서 1 프레임동안 움직인 거리를 더해준 값이 다음 프레임의 배경 위치
        // 0에서 height 만큼 position이 바뀌었다가 Repeat에 의해 0으로 다시 돌아옴
        transform.position = startPos + Vector3.down * moveDistance;
    }
}
