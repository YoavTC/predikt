using NaughtyAttributes;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField] [ReadOnly] private Cell _currentCell;
    [SerializeField] private Color blackColor, whiteColor;
    
    public CircleTeam team;
    public int id;
    
    public Cell currentCell
    {
        get => _currentCell;
        private set => _currentCell = value;
    }

    public void Init(int _id)
    {
        id = _id;
        gameObject.name = $"{team.ToString()} [{id}]";
        GetComponent<SpriteRenderer>().color = team == CircleTeam.BLACK ? blackColor : whiteColor;

        MoveToCell(BoardManager.Instance.GetCellFromCoords(
            (int)(transform.position.x + 0.5f),
            (int)(transform.position.y + 0.5f)));
    }
    
    public void MoveToCell(Cell cell)
    {
        if (currentCell != null)
        {
            currentCell.UpdateOccupyingCircle();
        }
        
        currentCell = cell;
        currentCell.UpdateOccupyingCircle(this);
        transform.position = new Vector3(cell.x, cell.y, 0);
    }

    public void DisableCollider()
    {
        GetComponent<Collider2D>().enabled = false;
    }
}