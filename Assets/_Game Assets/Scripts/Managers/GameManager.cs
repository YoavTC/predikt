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
    
    [Header("Rpc")]
    private ulong localClientId;
    private ulong opponentClientId;
    private ClientRpcParams opponentRpcParams; // Default Rpc Send params to send only to other player
    
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
    private void OnClientConnectedCallback(ulong connectedClientId)
    {
        StartCoroutine(JoinSessionCoroutine(connectedClientId));
    }
    
    private IEnumerator JoinSessionCoroutine(ulong connectedClientId)
    {
        // Small delay to ensure the NetworkObject has
        // been spawned before triggering any Rpc methods
        yield return new WaitUntil(() => IsSpawned); 
        localClientId = NetworkManager.Singleton.LocalClientId;
        
        Debug.Log($"Client {connectedClientId} has connected!");

        if (connectedClientId != localClientId)
        {
            AllClientsAreConnectedClientRpc();
        }
    }

    [ClientRpc]
    private void AllClientsAreConnectedClientRpc()
    {
        GetOpponentClientIdAndRpc();
        
        Debug.Log("both have connected");
        // Enable button and prepare board
    }

    private void GetOpponentClientIdAndRpc()
    {
        ulong[] connectedClientIds = NetworkManager.Singleton.ConnectedClientsIds
            .Where(clientId => clientId != localClientId)
            .ToArray();
        
        if (connectedClientIds.Length > 0)
        {
            opponentClientId = connectedClientIds[0];
        }
        
        opponentRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new []{opponentClientId} }
        };
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
