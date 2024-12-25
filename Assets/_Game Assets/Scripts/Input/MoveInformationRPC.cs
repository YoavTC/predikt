using System;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public struct MoveInformationRPC : INetworkSerializable
{
    public ulong clientId;
        
    public int circleId;
    public int circleTeam;
    
    public Vector2Int originCellCoords;
    public Vector2Int targetCellCoords;

    public MoveInformationRPC(MoveInformation moveInformation, ulong clientId)
    {
        this.clientId = clientId;
        
        circleId = moveInformation.circle.id;
        circleTeam = (int) moveInformation.circle.team;

        originCellCoords = moveInformation.originCell.GetCoords();
        targetCellCoords = moveInformation.targetCell.GetCoords();
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        
        serializer.SerializeValue(ref circleId);
        serializer.SerializeValue(ref circleTeam);
        
        serializer.SerializeValue(ref originCellCoords);
        serializer.SerializeValue(ref targetCellCoords);
    }
}