using System;
using System.Collections.Generic;
using UnityEngine;

public class ValidMoveCellsProjector : MonoBehaviour
{
    private Cell[,] boardCells;
    private Cell lastCell;

    [SerializeField] private Color targetedCellColor;
    [SerializeField] private Color validMoveCellsColor;

    private Camera mainCamera;
    
    public void OnBoardInitialized(Cell[,] initializedBoard)
    {
        boardCells = initializedBoard;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            ClearBoard();
            CheckMousePosition();
        }
    }

    private void CheckMousePosition()
    {
        // Cast a ray from the camera to the mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Check if the hit object is a cell
            GameObject clickedObject = hit.collider.gameObject;
            if (clickedObject.TryGetComponent(out Cell cell))
            {
                if (lastCell != null && lastCell == cell)
                {
                    lastCell = null;
                    return;
                }
                
                Debug.Log(cell.name);
                
                lastCell = cell;
                cell.Paint(targetedCellColor);
                PaintValidMoveCells(cell);
            }
        }
    }

    private List<Cell> GetValidMoveCells(Cell cell)
    {
        List<Cell> validMoveCells = new List<Cell>();

        // Directions to check: up, down, left, right, and diagonals
        int[] directionX = { -1, 0, 1, 0, -1, 1, -1, 1 }; // Left, None, Right, None, Diagonals
        int[] directionY = { 0, -1, 0, 1, -1, -1, 1, 1 }; // None, Down, None, Up, Diagonals

        // Iterate over all directions
        for (int i = 0; i < directionX.Length; i++)
        {
            int xOffset = directionX[i];
            int yOffset = directionY[i];

            int offsetX = cell.x + xOffset;
            int offsetY = cell.y + yOffset;

            // Continue exploring in that direction until out of bounds
            while (offsetX >= 0 && offsetX < boardCells.GetLength(0) && offsetY >= 0 && offsetY < boardCells.GetLength(1))
            {
                // Access the cell in that direction
                Cell validMoveCell = boardCells[offsetX, offsetY];
                
                validMoveCells.Add(validMoveCell);

                // Move to the next cell in the same direction
                offsetX += xOffset;
                offsetY += yOffset;
            }
        }

        return validMoveCells;
    }

    private void PaintValidMoveCells(Cell cell)
    {
        List<Cell> validMoveCells = GetValidMoveCells(cell);

        foreach (Cell validMoveCell in validMoveCells)
        {
            validMoveCell.Paint(validMoveCellsColor);
        }
    }

    private void ClearBoard()
    {
        foreach (Cell tempCell in boardCells)
        {
            tempCell.Paint(Color.white);
        }
    }
}