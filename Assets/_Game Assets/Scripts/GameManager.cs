using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [Header("Components")] 
    [SerializeField] private PiecesDealer piecesDealer;
    
    void Start()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        if (piecesDealer == null) piecesDealer = GetComponent<PiecesDealer>();
    }

    // Call on server to enable start game button
    [ServerRpc]
    public void OnClientJoinSessionServerRpc()
    {
        Debug.Log($"[{OwnerClientId}] Client joined Server RPC");
    }

    [ClientRpc, UsedImplicitly]
    public void StartGameClientRpc()
    {
        piecesDealer.DealPieces(IsHost);
        Debug.Log($"[{OwnerClientId}] Dealing pieces Client RPC...");
        Debug.Log($"[{OwnerClientId}] I am{(IsHost ? "" : " not")} host!");
    }
}
