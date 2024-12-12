using UnityEngine;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;
    
    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
        Paint(Color.white);
    }

    public void Paint(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }
}