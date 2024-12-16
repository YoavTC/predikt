using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class Cell : MonoBehaviour
{
    [Header("Coordinates")]
    [ReadOnly] public int x;
    [ReadOnly] public int y;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Start() => GetComponents();
    
    private void GetComponents()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
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

    #region Occupying Circle
    private Circle occupyingCircle;
    
    public bool IsOccupied() => occupyingCircle;
    public Circle GetOccupyingCircle => occupyingCircle;
    
    public void UpdateOccupyingCircle(Circle circle = null)
    {
        occupyingCircle = circle;
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