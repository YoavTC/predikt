using NaughtyAttributes;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public CircleTeam team;
    
    [SerializeField] [ReadOnly] private Cell _currentCell;
    
    public Cell currentCell
    {
        get => _currentCell;
        private set => _currentCell = value;
    }
    
    public void MoveToCell(Cell cell)
    {
        currentCell = cell;
        currentCell.UpdateOccupyingCircle(this);
        transform.position = new Vector3(cell.x, cell.y, 0);
    }

    public void DisableCollider()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}