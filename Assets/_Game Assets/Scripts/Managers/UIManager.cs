using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private DragAndDropHandler dragAndDropHandler;
    
    [Header("Toolbar UI Elements")]
    [SerializeField] private Button lockButton;
    [Space]
    [SerializeField] private Image enemyLockStateImage;
    [SerializeField] private Color[] enemyLockStateColors;

    [Header("Board State")]
    private BoardState initialBoardState; 

    private void Start()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        if (dragAndDropHandler == null) dragAndDropHandler = DragAndDropHandler.Instance;
    }

    // Called when both the players connect to the session
    public void PrepareUI()
    {
        UpdateEnemyLockState(LockState.PLAYING);
    } 
        
        
    #region UI Updating
    public void UpdateEnemyLockState(LockState lockState)
    {
        enemyLockStateImage.color = enemyLockStateColors[(int) lockState];
    }

    // Called when the player moves a piece & causes a BoardState update
    public void UpdateBoardState(BoardState boardState)
    {
        lockButton.interactable = boardState != initialBoardState;
    }
    #endregion

    #region Button Click Listeners
    public void OnLockButtonPressed()
    {
        // lockButton.interactable = false;
        GameManager.Instance.LockMove();
        dragAndDropHandler.ChangeInputState(false);
    }

    public void OnUndoButtonPressed()
    {
        BoardManager.Instance.RestoreLastBoard();
        dragAndDropHandler.ChangeInputState(true);
    }
    #endregion
}