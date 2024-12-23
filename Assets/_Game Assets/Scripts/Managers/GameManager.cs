using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using External_Packages;
using JetBrains.Annotations;
using NaughtyAttributes;
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
    
    [Header("Rpc")]
    [SerializeField] [ReadOnly] private ulong localClientId;
    [SerializeField] [ReadOnly] private ulong opponentClientId;
    private ClientRpcParams opponentRpcParams; // Default Rpc Send params to send only to other player
    
    private bool hostJoined = false;
    
    void Start()
    {
        GetComponents();
        RegisterTurnPerformListeners();
        
        NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
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

    #region Network Joining & Initialization
    private void OnClientConnectedCallback(ulong obj)
    {
        Debug.Log($"[{NetworkManager.LocalClientId}]: OnClientConnectedCallback: {obj}");
        StartCoroutine(JoinSessionCoroutine());
    }
    
    private IEnumerator JoinSessionCoroutine()
    {
        yield return new WaitUntil(() => IsSpawned); // Small delay to ensure the NetworkObject has
                                                     // been spawned before triggering any Rpc methods
        
        localClientId = NetworkManager.Singleton.LocalClientId;
        RequestOpponentIdClientRpc(localClientId);
        
        Debug.Log($"[{localClientId}]: I joined the session!");
        
        OnClientJoinSessionServerRpc();
    }
    
    // Gets the opponent's client ID,
    // and sets the default Rpc params
    [ClientRpc]
    private void RequestOpponentIdClientRpc(ulong _opponentClientId)
    {
        Debug.Log($"[{localClientId}]: RequestOpponentIdClientRpc");
        // Ensure it doesn't run on the same client that called it
        if (localClientId == _opponentClientId) return;
        
        Debug.Log($"[{localClientId}]: Proceeding RequestOpponentIdClientRpc");
        
        opponentClientId = _opponentClientId;
        opponentRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new []{opponentClientId} }
        };
    }
    
    // Runs when a client joins the session
    // (Including on session creation)
    [ServerRpc(RequireOwnership = false)]
    private void OnClientJoinSessionServerRpc()
    {
        Debug.Log($"[{localClientId}]: A client has joined the session!");

        if (hostJoined)
        {
            Debug.Log($"[{localClientId}]: we can start now!");
        } else hostJoined = true;
    }
    #endregion

    [ClientRpc, UsedImplicitly]
    public void StartGameClientRpc()
    {
        piecesManager.DealPieces(IsHost);
    }

    #region Moves
    //TODO: Remake lock state checking logic
    
    private LockState localLockState;
    private LockState opponentLockState;
    
    public void LocalPlayerLockMove()
    {
        Debug.Log($"[{localClientId}]: Move locked! Notifying opponent..");
        localLockState = LockState.LOCKED;
        
        OpponentLockMoveClientRpc(opponentRpcParams);
        TryNextTurn();
    }

    [ClientRpc]
    private void OpponentLockMoveClientRpc(ClientRpcParams rpcParams = default)
    {
        Debug.Log($"[{localClientId}]: Got lock notification!");
        opponentLockState = LockState.LOCKED;
        
        uiManager.UpdateEnemyLockState(opponentLockState);
        TryNextTurn();
    }

    private void TryNextTurn()
    {
        if (opponentLockState == LockState.LOCKED && 
            localLockState == LockState.LOCKED) 
        {
            StartCoroutine(NextTurnCountdown());
        }
    }

    private IEnumerator NextTurnCountdown()
    {
        for (int i = 0; i < 2; i++)
        {
            Debug.Log($"{i}..");
            yield return new WaitForSeconds(1);
        }
        
        // BoardManager.Instance.LoadBoardState();
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
