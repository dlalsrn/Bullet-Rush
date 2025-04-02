public class Spawn
{
    public float Delay { get; private set; } // 생성 딜레이
    public string Type { get; private set; } // 타입
    public int Point { get; private set; } // 생성 위치

    public Spawn(float delay, string type, int point)
    {
        Delay = delay;
        Type = type;
        Point = point;
    }
}
