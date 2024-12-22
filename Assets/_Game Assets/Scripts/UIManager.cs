using System;
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

    public void UpdateEnemyLockState(LockState lockState)
    {
        enemyLockStateImage.color = enemyLockStateColors[(int) lockState];
    }

    public void OnBoardStateUpdated(BoardState boardState)
    {
        lockButton.interactable = boardState != initialBoardState;
    }

    #region Button Click Listeners
    public void OnLockButtonPressed()
    {
        lockButton.interactable = false;
        GameManager.Instance.LocalPlayerLockMove();
        dragAndDropHandler.ChangeInputState(false);
    }

    public void OnUndoButtonPressed()
    {
        BoardManager.Instance.RestoreLastBoardState();
        dragAndDropHandler.ChangeInputState(true);
    }
    #endregion
}