using External_Packages;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    [Header("Components")] 
    [SerializeField] private PiecesManager piecesManager;
    [SerializeField] private UIManager uiManager;
    
    void Start()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        if (piecesManager == null) piecesManager = GetComponent<PiecesManager>();
        if (uiManager == null) uiManager = GetComponent<UIManager>();
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
        piecesManager.DealPieces(IsHost);
        Debug.Log($"[{OwnerClientId}] Dealing pieces Client RPC...");
        Debug.Log($"[{OwnerClientId}] I am{(IsHost ? "" : " not")} host!");
    }
    
    public void LocalPlayerLockMove()
    {
        PlayerLockMoveClientRpc(OwnerClientId);
    }

    [ClientRpc]
    private void PlayerLockMoveClientRpc(ulong clientID)
    {
        Debug.Log(clientID == OwnerClientId ?
            "Announcement from myself, skipping!":
            "Other player!!");
        if (clientID == OwnerClientId) return;
        
        uiManager.UpdateEnemyLockState(LockState.LOCKED);
    }
}
