using System.Collections;
using System.Linq;
using External_Packages;
using Unity.Netcode;
using UnityEngine;
using WebSocketSharp;

// public class GameManager : NetworkSingleton<GameManager>
public class GameManager : NetworkSingleton<GameManager>, ITurnPerformListener
{
    [Header("Components")] 
    [SerializeField] private PiecesManager piecesManager;
    [SerializeField] private UIManager uiManager;
    
    [Header("Rpc")]
    private ulong localClientId;
    private ulong opponentClientId;
    private ClientRpcParams opponentRpcParams; // Default Rpc Send params to send only to other player
    
    void Start()
    {
        GetComponents();
        NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
    }
    
    #region Initializations
    private void GetComponents()
    {
        if (piecesManager == null) piecesManager = GetComponent<PiecesManager>();
        if (uiManager == null) uiManager = GetComponent<UIManager>();
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
        uiManager.UpdateEnemyLockState(LockState.PLAYING);
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

    private MoveInformation localMoveInformation;
    private MoveInformation opponentMoveInformation;
    
    public void UpdateMoveInformation(MoveInformation newMoveInformation)
    {
        localMoveInformation = newMoveInformation;
    }
    
    // Called when the local client uses the "Lock" button
    // (Called from UIManager's OnLockButtonPressed)
    public void LockMove()
    {
        Debug.Log($"[{localClientId}]: Move locked! Notifying opponent..");

        MoveInformationRPC moveInformationRPC = new MoveInformationRPC(localMoveInformation, localClientId);
        
        localLockState = LockState.LOCKED;
        ClientLockedMoveServerRpc(localClientId, moveInformationRPC);
    }

    // Received on server when any client locks their move
    // and calls the client RPC method below
    [ServerRpc(RequireOwnership = false)]
    private void ClientLockedMoveServerRpc(ulong clientId, MoveInformationRPC clientMoveInformationRpc)
    {
        Debug.Log("Server: Received ServerRpc, notifying clients..");
        ClientLockedMoveClientRpc(clientId, clientMoveInformationRpc);
    }
    
    // Called on every client from the server RPC method above
    [ClientRpc]
    private void ClientLockedMoveClientRpc(ulong clientId, MoveInformationRPC clientMoveInformationRpc)
    {
        if (clientMoveInformationRpc.clientId != localClientId)
        {
            opponentMoveInformation = new MoveInformation(clientMoveInformationRpc);
        }
        
        if (localClientId != clientId)
        {
            Debug.Log($"[{localClientId}]: Got lock notification!");
            opponentLockState = LockState.LOCKED;
            uiManager.UpdateEnemyLockState(opponentLockState);
        } 
        
        if (opponentLockState == LockState.LOCKED && localLockState == LockState.LOCKED) 
        {
            Debug.Log("Both locked, starting next turn sequence");
            
            Debug.Log($"Opp: {opponentMoveInformation.circle.id} moved {opponentMoveInformation.originCell.name} -> {opponentMoveInformation.targetCell.name}");
            
            
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
        
        OperationResult operationResult = ApplyMovesOperation();
        InvokeTurnPerformInterfaceCall(); 
        
        Debug.Log(">> TURN OVER");
        
        Debug.Log($"operation: {operationResult.message}");
    }

    private OperationResult ApplyMovesOperation()
    {
        Cell opponentTargetCell = opponentMoveInformation.targetCell;
        Cell localTargetCell = localMoveInformation.targetCell;

        if (opponentTargetCell == localTargetCell)
        {
            return new OperationResult(false, "Both players moved to the same cell!");
        }

        string operationResultMessage = "";

        Circle opponentTargetCircle = opponentTargetCell.GetOccupyingCircle;
        Circle localTargetCircle = localTargetCell.GetOccupyingCircle;
        
        Debug.Log("-----------");
        Debug.Log(opponentTargetCell);
        Debug.Log(localTargetCell);
        Debug.Log(opponentTargetCircle);
        Debug.Log(localTargetCircle);
        Debug.Log(opponentTargetCircle != localMoveInformation.circle);
        Debug.Log(localTargetCircle != opponentMoveInformation.circle);
        Debug.Log("-----------");
        
        // Opponent kill circle
        if (opponentTargetCircle != null && opponentTargetCircle != localMoveInformation.circle)
        {
            operationResultMessage += "// Opponent killed your piece!";
            // Debug.Log($">> {opponentMoveInformation.circle} KILL {opponentTargetCircle.id}");
            Destroy(opponentTargetCircle.gameObject);
        }
        
        // Local kill circle
        if (localTargetCircle != null && localTargetCircle != opponentMoveInformation.circle)
        {
            operationResultMessage += "// You killed your opponent's piece!";
            // Debug.Log($">> {localMoveInformation.circle} KILL {localTargetCircle.id}");
            Destroy(localTargetCircle.gameObject);
        }

        Debug.Log($">> {localMoveInformation.circle} MOVED {localMoveInformation.originCell} -> {localMoveInformation.targetCell}");
        Debug.Log($">> {opponentMoveInformation.circle} MOVED {opponentMoveInformation.originCell} -> {opponentMoveInformation.targetCell}");
        
        if (operationResultMessage.IsNullOrEmpty())
        {
            operationResultMessage = "// Nothing happened!";
        }
        
        opponentMoveInformation.circle.MoveToCell(opponentMoveInformation.targetCell);
        localMoveInformation.circle.MoveToCell(localMoveInformation.targetCell);
        
        return new OperationResult(true, operationResultMessage);
    }
    #endregion

    #region Turn Calls
    private void InvokeTurnPerformInterfaceCall()
    {
        MonoBehaviour[] allMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);

        // Loop through them and invoke the TurnPerformed method if they implement ITurnPerformListener
        foreach (var monoBehaviour in allMonoBehaviours)
        {
            if (monoBehaviour is ITurnPerformListener listener)
            {
                listener.TurnPerformed();
            }
        }
    }

    public void TurnPerformed()
    {
        localLockState = LockState.PLAYING;
        opponentLockState = LockState.PLAYING;
    }
    #endregion
}