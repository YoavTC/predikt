using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] private Cell currentCell;
    
    public void MoveToCell(Cell cell)
    {
        currentCell = cell;
        transform.position = new Vector3(cell.x, cell.y, 0);
    }
}