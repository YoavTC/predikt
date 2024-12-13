using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Camera mainCamera;
    private int circleLayerMask;
    private int cellLayerMask;
    
    private Vector2 mousePosition;

    [SerializeField] private Circle selectedCircle;

    private ValidMoveCellsProjector moveCellsProjector;
    
    private void Start()
    {
        GetComponents();
        InitializeLayerMasks();
    }

    private void GetComponents()
    {
        mainCamera = Camera.main;
        moveCellsProjector = GetComponent<ValidMoveCellsProjector>();
    }

    private void InitializeLayerMasks()
    {
        circleLayerMask = LayerMask.GetMask("Circle");
        cellLayerMask = LayerMask.GetMask("Cell");
    }
    
    // TODO: Replace this whole stupid moving system with drag n drop
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Click");
            mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            if (selectedCircle == null)
            {
                Debug.Log("sc: " + selectedCircle);
                Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition, circleLayerMask);
                if (hitCollider)
                {
                    Debug.Log("Found circle: " + selectedCircle);
                    selectedCircle = hitCollider.GetComponent<Circle>();
                }
            }
            else
            {
                Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition, cellLayerMask);
                if (hitCollider)
                {
                    Debug.Log("hc: " + hitCollider);
                    if (TryMoveCircleToCell(hitCollider.GetComponent<Cell>()))
                    {
                        selectedCircle = null;
                    }
                }
            }
        }
    }

    private bool TryMoveCircleToCell(Cell selectedCell)
    {
        List<Cell> validCells = moveCellsProjector.GetValidMoveCells(selectedCircle.GetCurrentCell());
        if (validCells.Contains(selectedCell))
        {
            selectedCircle.MoveToCell(selectedCell);
            return true;
        }

        return false;
    }
}
