using System;

[Serializable]
public class MoveInformation
{
    public Circle circle;
    public Cell originCell;
    public Cell targetCell;

    public MoveInformation(MoveInformationRPC moveInformationRPC)
    {
        circle = BoardManager.Instance.GetCircleFromID(moveInformationRPC.circleId);
        circle.team = (CircleTeam) moveInformationRPC.circleTeam;
        
        originCell = BoardManager.Instance.GetCellFromCoords(moveInformationRPC.originCellCoords);
        targetCell = BoardManager.Instance.GetCellFromCoords(moveInformationRPC.targetCellCoords);
    }

    public MoveInformation(Circle circle, Cell originCell, Cell targetCell)
    {
        this.circle = circle;
        this.originCell = originCell;
        this.targetCell = targetCell;
    }
}