using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] private Cell _currentCell;
    
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
}