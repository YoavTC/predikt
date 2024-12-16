using System.Collections.Generic;
using UnityEngine;

public class PiecesDealer : MonoBehaviour
{
    [SerializeField] private Transform circlesParent;

    private List<Circle> blackCircles = new List<Circle>();
    private List<Circle> whiteCircles = new List<Circle>();
    
    void Start()
    {
        GetPieces();
    }

    private void GetPieces()
    {
        for (int i = 0; i < circlesParent.childCount; i++)
        {
            if (circlesParent.GetChild(i).TryGetComponent(out Circle circle))
            {
                if (circle.team == CircleTeam.BLACK) blackCircles.Add(circle);
                else whiteCircles.Add(circle);
            }
        }
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
