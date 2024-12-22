using System.Collections;
using System.Collections.Generic;
using System.Linq;
using External_Packages;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

// public class GameManager : NetworkSingleton<GameManager>
public class GameManager : NetworkSingleton<GameManager>
{
    [Header("Components")] 
    [SerializeField] private PiecesManager piecesManager;
    [SerializeField] private UIManager uiManager;
    
    // List of MonoBehaviour classes inheriting from the ITurnPerformListener interface
    private List<ITurnPerformListener> turnPerformListeners = new List<ITurnPerformListener>();
    
    void Start()
    {
        GetComponents();
        RegisterTurnPerformListeners();
    }

    #region Initializations
    private void GetComponents()
    {
        if (piecesManager == null) piecesManager = GetComponent<PiecesManager>();
        if (uiManager == null) uiManager = GetComponent<UIManager>();
    }

    private void RegisterTurnPerformListeners()
    {
        MonoBehaviour[] allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        turnPerformListeners = allMonoBehaviours.OfType<ITurnPerformListener>().ToList();
    }
    #endregion

    #region Session Joining
    private bool hostJoined = false;
    
    public void OnClientJoinSession()
    {
        StartCoroutine(JoinSessionCoroutine());
    }

    // Small delay to ensure the NetworkObject has
    // been spawned before triggering Rpc methods
    private IEnumerator JoinSessionCoroutine()
    {
        yield return new WaitUntil(() => IsSpawned);
        OnClientJoinSessionServerRpc();
    }
    
    // Runs on server whenever a client joins the session
    // (Including on session creation)
    [ServerRpc(RequireOwnership = false)]
    private void OnClientJoinSessionServerRpc()
    {
        Debug.Log("A client has joined the session!");

        if (hostJoined)
        {
            Debug.Log("We can start the game now!");
        } else hostJoined = true;
    }
    #endregion

    [ClientRpc, UsedImplicitly]
    public void StartGameClientRpc()
    {
        piecesManager.DealPieces(IsHost);
        Debug.Log($"[{OwnerClientId}] Dealing pieces Client RPC...");
        Debug.Log($"[{OwnerClientId}] I am{(IsHost ? "" : " not")} host!");
    }

    #region Moves
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
    #endregion
    
    private void InvokeTurnPerformInterfaceCall()
    {
        foreach (var listener in turnPerformListeners)
        {
            listener?.TurnPerformed();
        }
    }
}
