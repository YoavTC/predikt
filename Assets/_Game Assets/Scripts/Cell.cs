using System;
using UnityEngine;

[Serializable]
public class Cell : MonoBehaviour
{
    [Header("Coordinates")]
    public int x;
    public int y;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start() => GetComponents();
    
    private void GetComponents()
    {
        spriteRenderer ??= GetComponent<SpriteRenderer>();
    }

    #region Cell Methods
    public void Paint(Color color)
    {
        spriteRenderer.color = color;
    }

    public bool Compare(int _x, int _y)
    {
        return x == _x && y == _y;
    }
    #endregion

    #region Obsolete
    [Obsolete("Used for cell generation. Keeping just in case")]
    public void Init(int _x, int _y)
    {
        x = _x;
        y = _y;
        Paint(Color.white);
    }
    #endregion
}