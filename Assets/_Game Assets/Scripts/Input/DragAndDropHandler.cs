using External_Packages;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class DragAndDropHandler : Singleton<DragAndDropHandler>
{
    [Header("Components")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] [ReadOnly] private int circleLayerMask;

    [Header("Dragging")] 
    [SerializeField] [ReadOnly] private bool canDrag = true;
    [SerializeField] [ReadOnly] private Circle currentCircle;
    [SerializeField] [ReadOnly] private DragInformation dragInformation;
    [SerializeField] [ReadOnly] private Vector2 mousePosition;
    
    [Header("Events")]
    public UnityEvent<Cell> PickUpUnityEvent;
    public UnityEvent<DragInformation> MoveDragUnityEvent;
    public UnityEvent<DragInformation> DropValidUnityEvent;
    public UnityEvent<DragInformation> DropInvalidUnityEvent;
    
    [Header("Consts")]
    private const string VALID_DROP_LOCATION_MESSAGE = "Dropped at valid location. Moving.";
    private const string INVALID_DROP_LOCATION_MESSAGE = "Dropped at invalid location. Reverting.";
    
    private void Start() => GetComponents();
    private void Update() => HandleInput();
    
    private void GetComponents()
    {
        circleLayerMask = LayerMask.GetMask("Circle");
        if (mainCamera == null) mainCamera = Camera.main;
    }
    
    private void HandleInput()
    {
        if (!canDrag) return;
        
        if (Input.GetMouseButtonDown(0)) TryPickUp();
        else if (currentCircle != null)
        {
            if (Input.GetMouseButton(0)) MoveObject();
            else if (Input.GetMouseButtonUp(0)) DropObject(dragInformation);
        }
    }

    public void ChangeInputState(bool state)
    {
        canDrag = state;
    }

    #region Drag & Dropping
    private void TryPickUp()
    {
        mousePosition = GetMousePosition();

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, circleLayerMask);
        if (hit.collider?.TryGetComponent(out Circle hitCircle) == true)
        {
            currentCircle = hitCircle;

            dragInformation = new DragInformation(currentCircle);
            PickUpUnityEvent?.Invoke(dragInformation._originalCell);
        }
    }

    private void MoveObject()
    {
        mousePosition = GetMousePosition();
        currentCircle.transform.position = mousePosition;
        
        dragInformation.UpdateTargetedCell();
        MoveDragUnityEvent?.Invoke(dragInformation);
    }

    private void DropObject(DragInformation eventData)
    {
        bool validMove = eventData.IsValidMoveCell();
        if (validMove)
        {
            currentCircle.MoveToCell(eventData.targetedCell);
            DropValidUnityEvent?.Invoke(eventData);
        } else {
            currentCircle.MoveToCell(eventData._originalCell);
            DropInvalidUnityEvent?.Invoke(eventData);
        }
        
        Debug.Log(validMove ? VALID_DROP_LOCATION_MESSAGE : INVALID_DROP_LOCATION_MESSAGE);
        currentCircle = null;
    }
    #endregion

    #region Utility
    private Vector2 GetMousePosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
    #endregion
}