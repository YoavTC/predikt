using System.Collections.Generic;
using External_Packages;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class DragAndDropHandler : Singleton<DragAndDropHandler>
{
    [Header("Components")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private int circleLayerMask;

    [Header("Dragging")] 
    [SerializeField] private ProjectionCircle projectionCirclePrefab;
    private ProjectionCircle projectionCircle;
    
    [SerializeField] [ReadOnly] private bool canDrag = true;
    [SerializeField] [ReadOnly] private bool isDragging = false;
    
    [SerializeField] [ReadOnly] private MoveInformation currentMoveInformation;
    [SerializeField] [ReadOnly] private Circle currentCircle;
    [SerializeField] [ReadOnly] private Vector2 mousePosition;
    
    [Header("Events")]
    public UnityEvent<Cell> PickCircleUnityEvent;
    public UnityEvent StopDragCircleUnityEvent;
    public UnityEvent<MoveInformation> DropCircleUnityEvent;
    
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
        else if (currentCircle != null && projectionCircle != null)
        {
            if (Input.GetMouseButton(0) && isDragging) MoveObject();
            else if (Input.GetMouseButtonUp(0)) DropObject();
        }
    }
    
    #region Drag & Dropping
    private void TryPickUp()
    {
        mousePosition = GetMousePosition();

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, circleLayerMask);
        if (hit.collider?.TryGetComponent(out Circle hitCircle) == true)
        {
            if (currentCircle != null)
            {
                ResetDrag();
                if (currentCircle == hitCircle) return;
            }

            isDragging = true;
            currentCircle = hitCircle;

            projectionCircle = Instantiate(projectionCirclePrefab, currentCircle.transform.position, Quaternion.identity);
            
            PickCircleUnityEvent?.Invoke(currentCircle.currentCell);
        }
    }

    private void MoveObject()
    {
        mousePosition = GetMousePosition();
        projectionCircle.MoveTo(mousePosition);
    }

    private void DropObject()
    {
        Vector2Int startCoords = projectionCircle.GetOriginalCoords();
        Vector2Int dropCoords = projectionCircle.GetCurrentCoords();
        
        Cell originCell = BoardManager.Instance.GetCellFromCoords(startCoords.x, startCoords.y);
        Cell targetCell = BoardManager.Instance.GetCellFromCoords(dropCoords.x, dropCoords.y);

        currentMoveInformation = new MoveInformation(currentCircle, originCell, targetCell);
        StopDragCircleUnityEvent?.Invoke();

        List<Cell> validCells = BoardManager.Instance.GetValidCellMoves(originCell);
        if (validCells.Contains(targetCell))
        {
            DropCircleUnityEvent?.Invoke(currentMoveInformation);
        }
        else
        {
            ResetDrag();
        }

        isDragging = false;
    }

    private void ResetDrag()
    {
        Destroy(projectionCircle.gameObject);
        currentCircle = null;
    }
    #endregion
    
    public void ChangeInputState(bool state)
    {
        canDrag = state;
    }
    
    private Vector2 GetMousePosition()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}