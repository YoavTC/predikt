using System.Collections.Generic;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
    [SerializeField] private Transform circlesParent;

    private List<Circle> blackCircles = new List<Circle>();
    private List<Circle> whiteCircles = new List<Circle>();
    
    public void GetPieces()
    {
        for (int i = 0; i < circlesParent.childCount; i++)
        {
            if (circlesParent.GetChild(i).TryGetComponent(out Circle circle))
            {
                if (circle.team == CircleTeam.BLACK) blackCircles.Add(circle);
                else whiteCircles.Add(circle);
                
                circle.MoveToCell(BoardManager.Instance.GetCellFromCoords(
                    (int)(circle.transform.position.x + 0.5f),
                    (int)(circle.transform.position.y + 0.5f)));
            }
        }
    }

    public List<Circle> GetAllPieces()
    {
        List<Circle> allCircles = new List<Circle>();
        allCircles.AddRange(blackCircles);
        allCircles.AddRange(whiteCircles);
        return allCircles;
    }

    public void DealPieces(bool isBlack)
    {
        if (isBlack)
        {
            foreach (Circle circle in whiteCircles)
            {
                circle.DisableCollider();
            }
        } else {
            foreach (Circle circle in blackCircles)
            {
                circle.DisableCollider();
            }
        }
    }
}
