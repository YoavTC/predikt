using System.Collections.Generic;
using System.Linq;
using External_Packages;
using UnityEngine;
using UnityEngine.Events;

public class BoardManager : Singleton<BoardManager>
{
    [Header("Board")]
    private Cell[] cells;
    private Cell[,] boardCells;
    private const int BOARD_SIZE = 9;
    
    [Header("Components")]
    [SerializeField] private BoardCellsPainter boardCellsPainter;
    [SerializeField] private PiecesManager piecesManager;
    
    [Header("Events")]
    public UnityEvent<List<Cell>> BoardCellsInitializedUnityEvent;
    
    private void Start()
    {
        GetCells();
        GetBoard();
        GetComponents();
        
        BoardCellsInitializedUnityEvent?.Invoke(cells.ToList());
        
        // Store initial board state
    }
    public void ResetBoard()
    {
        
    }

    #region GetComponents Initialization
    private void GetCells()
    {
        var children = HelperFunctions.GetChildrenWithComponent<Cell>(transform);
        cells = children.Select(a => a.GetComponent<Cell>()).ToArray();
    }
    
    private void GetBoard()
    {
        boardCells = new Cell[BOARD_SIZE, BOARD_SIZE];

        for (int x = 0; x < BOARD_SIZE; x++)
        {
            for (int y = 0; y < BOARD_SIZE; y++)
            {
                Cell cell = GetCellFromCoords(x, y);
                boardCells[x, y] = cell;
            }
        }
    }

    private void GetComponents()
    {
        if (boardCellsPainter == null) boardCellsPainter = FindFirstObjectByType<BoardCellsPainter>();
        if (piecesManager == null) piecesManager = FindFirstObjectByType<PiecesManager>();
    }
    #endregion

    #region Cell-Board Utility
    public Circle GetCircleFromID(int id)
    {
        return piecesManager.GetPieceWithID(id);
    }

    public Cell GetCellFromCoords(int x, int y)
    {
        return GetCellFromCoords(new Vector2Int(x, y));
    }
    
    public Cell GetCellFromCoords(Vector2Int cellCoords)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].Compare(cellCoords)) return cells[i];
        }

        return null;
    }

    public bool IsValidCellMove(Cell origin, Cell target)
    {
        List<Cell> validCellMoves = GetValidCellMoves(origin);
        return validCellMoves.Contains(target);
    }

    public List<Cell> GetAllCells()
    {
        return cells.ToList();
    }

    public (List<Cell>, List<Cell>) GetValidCellMoves(Cell cell, CircleTeam team)
    {
        List<Cell> validMoveCells = GetValidCellMoves(cell);
        List<Cell> actualValidMoveCells = new List<Cell>();

        for (int i = 0; i < validMoveCells.Count; i++)
        {
            if (validMoveCells[i].GetOccupyingCircle?.team != team)
            {
                actualValidMoveCells.Add(validMoveCells[i]);
            }
        }

        for (int i = 0; i < actualValidMoveCells.Count; i++)
        {
            validMoveCells.Remove(actualValidMoveCells[i]);
        }
        
        return (actualValidMoveCells, validMoveCells);
    }
    
    public List<Cell> GetValidCellMoves(Cell cell)
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

        validMoveCells.Remove(cell);
        return validMoveCells;
    }
    #endregion

    #region BoardCellsPainter Communicator
    public void ClearBoard()
    {
        boardCellsPainter.ClearBoard();
    }

    public void PaintValidCells(Cell cell)
    {
        boardCellsPainter.PaintValidCells(cell);
    }
    #endregion
}