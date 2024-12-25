using System;
using UnityEngine;

[Serializable]
public class MoveInformationRPC
{
    public int circleId;
    public int circleTeam;
    
    public Vector2Int originCellCoords;
    public Vector2Int targetCellCoords;

    public MoveInformationRPC(MoveInformation moveInformation)
    {
        circleId = moveInformation.circle.id;
        circleTeam = (int) moveInformation.circle.team;

        originCellCoords = moveInformation.originCell.GetCoords();
        targetCellCoords = moveInformation.targetCell.GetCoords();
    }
}