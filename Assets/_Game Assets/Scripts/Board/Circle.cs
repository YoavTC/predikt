using System;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] [ReadOnly] private Cell _currentCell;
    [SerializeField] private Color blackColor, whiteColor;

    private float moveAnimationDuration;
    private Ease moveAnimationEasing;
    
    public CircleTeam team;
    public int id;
    
    public Cell currentCell
    {
        get => _currentCell;
        private set => _currentCell = value;
    }

    public void Init(int _id, float _moveAnimationDuration, Ease _moveAnimationEasing)
    {
        id = _id;
        gameObject.name = $"{team.ToString()} [{id}]";
        GetComponent<SpriteRenderer>().color = team == CircleTeam.BLACK ? blackColor : whiteColor;

        moveAnimationDuration = _moveAnimationDuration;
        moveAnimationEasing = _moveAnimationEasing;

        MoveToCell(BoardManager.Instance.GetCellFromCoords(
            (int)(transform.position.x + 0.5f),
            (int)(transform.position.y + 0.5f)), false);
    }
    
    public void MoveToCell(Cell cell, bool animate = true, Action OnFinishCallback = null)
    {
        if (currentCell != null && currentCell.GetOccupyingCircle == this)
        {
            currentCell.UpdateOccupyingCircle();
        }
        
        currentCell = cell;
        currentCell.UpdateOccupyingCircle(this);
        
        // transform.position = new Vector3(cell.x, cell.y, 0);
        transform.DOMove(new Vector3(cell.x, cell.y), moveAnimationDuration).SetEase(moveAnimationEasing).OnComplete(() => OnFinishCallback?.Invoke());
    }
    
    public void DisableCollider()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}