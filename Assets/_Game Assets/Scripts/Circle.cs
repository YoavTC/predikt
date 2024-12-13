using UnityEngine;

public class Circle : MonoBehaviour
{
    private Cell currentCell;
    
    public Cell GetCurrentCell()
    {
        if (currentCell != null) return currentCell;
        
        currentCell = BoardManager.Instance.GetCellFromCoords((int) transform.position.x, (int) transform.position.x);
        return currentCell;
    }
    
    public void MoveToCell(Cell cell)
    {
        currentCell = cell;
        transform.position = new Vector3(cell.x, cell.y, 0);
    }
}