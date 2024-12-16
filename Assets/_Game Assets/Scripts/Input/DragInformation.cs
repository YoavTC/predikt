using System;

[Serializable]
public class DragInformation
{
    private readonly Circle _draggedCircle;
    public readonly Cell _originalCell;
    public Cell targetedCell;

    public DragInformation(Circle draggedCircle)
    {
        _draggedCircle = draggedCircle;
        _originalCell = UpdateTargetedCell();
        targetedCell = _originalCell;
    }

    public Cell UpdateTargetedCell()
    {
        targetedCell = BoardManager.Instance.GetCellFromCoords(
            (int)(_draggedCircle.transform.position.x + 0.5f),
            (int)(_draggedCircle.transform.position.y + 0.5f));
        
        return targetedCell;
    }

    public bool IsValidMoveCell()
    {
        return BoardManager.Instance.IsValidCellMove(_originalCell, targetedCell);
    }
}