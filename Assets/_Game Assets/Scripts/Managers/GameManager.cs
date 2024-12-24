using System.Collections;
using System.Collections.Generic;
using System.Linq;
using External_Packages;
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

        // Called on the host when the opponent connects
        if (IsHost && connectedClientId != localClientId)
        {
            AllClientsAreConnectedClientRpc();
        }
    }

    [ClientRpc]
    private void AllClientsAreConnectedClientRpc()
    {
        GetOpponentIdAndRpcParams();
        
        // Enable button and prepare board
        uiManager.PrepareUI();
        piecesManager.DealPieces(IsHost);
        BoardManager.Instance.ResetBoard();
    }

    private void GetOpponentIdAndRpcParams()
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
        
        Debug.Log($"[{localClientId}]: got opponent's ID: {opponentClientId}, and set Rpc params");
    }
    #endregion

    #region Moves
    private LockState localLockState;
    private LockState opponentLockState;
    
    // Called when the local client uses the "Lock" button
    // (Called from UIManager's OnLockButtonPressed)
    public void LockMove()
    {
        Debug.Log($"[{localClientId}]: Move locked! Notifying opponent..");
        
        localLockState = LockState.LOCKED;
        ClientLockedMoveServerRpc(localClientId);
    }

    // Received on server when any client locks their move
    // and calls the client RPC method below
    [ServerRpc(RequireOwnership = false)]
    private void ClientLockedMoveServerRpc(ulong clientId)
    {
        Debug.Log("Server: Received ServerRpc, notifying clients..");
        ClientLockedMoveClientRpc(clientId);
    }
    
    // Called on every client from the server RPC method above
    [ClientRpc]
    private void ClientLockedMoveClientRpc(ulong clientId)
    {
        if (localClientId != clientId)
        {
            Debug.Log($"[{localClientId}]: Got lock notification!");
            opponentLockState = LockState.LOCKED;
            uiManager.UpdateEnemyLockState(opponentLockState);
        } 
        
        if (opponentLockState == LockState.LOCKED && localLockState == LockState.LOCKED) 
        {
            Debug.Log("Both locked, starting next turn sequence");
            StartCoroutine(NextTurnSequence());
        }
    }

    private IEnumerator NextTurnSequence()
    {
        for (int i = 3; i > 0; i--)
        {
            Debug.Log($"{i}..");
            yield return new WaitForSeconds(1);
        }
        
        // BoardManager.Instance.LoadBoard();
        InvokeTurnPerformInterfaceCall();
        
        Debug.Log("Next turn sequence finished!");
    }
    #endregion
    
    // Invokes the TurnPerformed method on all ITurnPerformedListener inheritors
    private void InvokeTurnPerformInterfaceCall()
    {
        for (int i = 0; i < turnPerformListeners.Count; i++)
        {
            turnPerformListeners[i]?.TurnPerformed();
        }
    }
}
