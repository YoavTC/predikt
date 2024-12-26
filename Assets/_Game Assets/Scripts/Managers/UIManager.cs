using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour, ITurnPerformListener
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
        lockButton.interactable = false;
        GameManager.Instance.LockMove();
        dragAndDropHandler.ChangeInputState(false);
    }

    public void TurnPerformed()
    {
        UpdateLockButtonState(false);
        UpdateEnemyLockState(LockState.PLAYING);
    }
}