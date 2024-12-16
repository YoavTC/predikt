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
    [SerializeField] private Cell lastCell;
    [SerializeField] private List<Cell> allCells;
    
    [Preserve, UsedImplicitly]
    public void OnBoardCellsInitialized(List<Cell> initializedBoardCells)
    {
        allCells = initializedBoardCells;
    }

    // TODO: Implement this in an input class
    // private void CheckMousePosition()
    // {
    //     mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    //         
    //     Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition, cellLayerMask);
    //     if (hitCollider != null && hitCollider.TryGetComponent(out Cell cell))
    //     {
    //         if (lastCell != null && lastCell == cell)
    //         {
    //             lastCell = null;
    //             return;
    //         }
    //         
    //         Debug.Log(cell.name);
    //         
    //         lastCell = cell;
    //         PaintValidCells(cell);
    //     }
    // }
    
    /// Marked as internal to hide from inspector. Call "BoardManager.Instance.PaintValidCells" instead
    internal void PaintValidCells(Cell cell)
    {
        List<Cell> validMoveCells = BoardManager.Instance.GetValidCellMoves(cell);
        allCells = BoardManager.Instance.GetAllCells();
        
        foreach (Cell tempCell in allCells)
        {
            if (validMoveCells.Contains(tempCell)) tempCell.Paint(validColor);
            else if (!paintValidCellsOnly) tempCell.Paint(invalidColor);
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