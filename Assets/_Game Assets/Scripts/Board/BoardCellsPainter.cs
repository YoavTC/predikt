using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;

public class BoardCellsPainter : MonoBehaviour
{
    [Header("Painter Settings")]
    [SerializeField] private bool paintValidCellsOnly;
    [SerializeField] private Color validColor, invalidColor;

    [Header("Board Cells")] 
    [SerializeField] private float cellGapWidth;
    private Cell lastCell;
    private List<Cell> allCells;
    private ITurnPerformListener turnPerformListenerImplementation;

    [Preserve, UsedImplicitly]
    public void OnBoardCellsInitialized(List<Cell> initializedBoardCells)
    {
        allCells = initializedBoardCells;

        for (int i = 0; i < allCells.Count; i++)
        {
            allCells[i].transform.localScale = Vector3.one - (Vector3.one * cellGapWidth);
        }
    }
    
    /// Marked as internal to hide from inspector. Call "BoardManager.Instance.PaintValidCells" instead
    internal void PaintValidCells(Cell cell)
    {
        var validMoveCells = BoardManager.Instance.GetValidCellMoves(cell, cell.GetOccupyingCircle.team);
        allCells = BoardManager.Instance.GetAllCells();
        
        foreach (Cell tempCell in allCells)
        {
            if (validMoveCells.Item1.Contains(tempCell))
            {
                tempCell.Paint(validColor);
            }
            
            if (validMoveCells.Item2.Contains(tempCell))
            {
                tempCell.Paint(invalidColor);
            }
        }
    }

    /// Marked as internal to hide from inspector. Call "BoardManager.Instance.ClearBoard" instead
    internal void ClearBoard()
    {
        foreach (Cell tempCell in allCells)
        {
            tempCell.Paint(Color.white);
        }
    }
}