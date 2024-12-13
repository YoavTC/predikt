using System.Linq;
using External_Packages;
using UnityEngine;
using UnityEngine.Events;

public class BoardManager : Singleton<BoardManager>
{
    private Cell[] cells;
    private Cell[,] boardCells;

    private const int BOARD_SIZE = 9;
    
    private void Start()
    {
        GetCells();
        GetBoard();
        
        BoardCellsInitializedUnityEvent?.Invoke(boardCells);
    }

    #region Cell Board Initialization
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
    
    public Cell GetCellFromCoords(int x, int y)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].Compare(x, y)) return cells[i];
        }

        return null;
    }
    #endregion

    public UnityEvent<Cell[,]> BoardCellsInitializedUnityEvent;
}