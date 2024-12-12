using System.Linq;
using External_Packages;
using UnityEngine.Events;

public class BoardManager : Singleton<BoardManager>
{
    private Cell[] boardCells;

    private void Start()
    {
        GetBoard();
    }

    private void GetBoard()
    {
        var children = HelperFunctions.GetChildrenWithComponent<Cell>(transform);
        boardCells = children.Select(a => a.GetComponent<Cell>()).ToArray();
    }
}