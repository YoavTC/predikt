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
    
    // UI updating
    public void UpdateEnemyLockState(LockState lockState)
    {
        enemyLockStateImage.color = enemyLockStateColors[(int) lockState];
    }
    
    public void UpdateLockButtonState(bool state)
    {
        lockButton.interactable = state;
    }
    
    // UI listening
    public void OnLockButtonPressed()
    {
        // lockButton.interactable = false;
        GameManager.Instance.LockMove();
        dragAndDropHandler.ChangeInputState(false);
    }
}