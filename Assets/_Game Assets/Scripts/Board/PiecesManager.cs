using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PiecesManager : MonoBehaviour
{
    [Header("Move Animation Settings")] 
    [SerializeField] private float moveAnimationDuration;
    [SerializeField] private Ease moveAnimationEasing;
    
    [SerializeField] private Transform circlesParent;

    [SerializeField] private List<Circle> blackCircles = new List<Circle>();
    [SerializeField] private List<Circle> whiteCircles = new List<Circle>();
    
    public void GetPieces()
    {
        for (int i = 0; i < circlesParent.childCount; i++)
        {
            if (circlesParent.GetChild(i).TryGetComponent(out Circle circle))
            {
                if (circle.team == CircleTeam.BLACK) blackCircles.Add(circle);
                else whiteCircles.Add(circle);

                circle.Init(i);
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

    public Circle GetPieceWithID(int id)
    {
        foreach (var circle in blackCircles)
        {
            if (circle.id == id) return circle;
        }
        
        foreach (var circle in whiteCircles)
        {
            if (circle.id == id) return circle;
        }

        return null;
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

    public IEnumerator CheckWinOrLose(Action<CircleTeam> loser)
    {
        // Wait for Destroy call to finish
        yield return new WaitForEndOfFrame();
        RemoveMissingPieces();
        
        if (blackCircles.Count <= 1) loser(CircleTeam.BLACK);
        if (whiteCircles.Count <= 1) loser(CircleTeam.WHITE);
    }
    
    private void RemoveMissingPieces()
    {
        blackCircles.RemoveAll(item => item == null);
        whiteCircles.RemoveAll(item => item == null);
    }
}
