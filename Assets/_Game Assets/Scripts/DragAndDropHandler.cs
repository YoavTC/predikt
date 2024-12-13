using UnityEngine;

public class DragAndDropHandler : MonoBehaviour
{
    private GameObject currentObject;
    private Vector3 originalPosition;
    
    private Camera mainCamera;
    private int circleLayerMask;

    private void Start()
    {
        GetComponents();
    }

    private void GetComponents()
    {
        circleLayerMask = LayerMask.GetMask("Circle");
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) TryPickUp();
        else if (currentObject != null)
        {
            if (Input.GetMouseButton(0)) MoveObject();
            else if (Input.GetMouseButtonUp(0)) DropObject();
        }
    }

    #region Drag & Dropping
    private void TryPickUp()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, circleLayerMask);
        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.GetComponent<Circle>() != null)
            {
                currentObject = hitObject;
                originalPosition = currentObject.transform.position;
                
                OnPickUp(currentObject);
            }
        }
    }

    private void MoveObject()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        currentObject.transform.position = mousePosition;
        
        OnMove(currentObject, mousePosition);
    }

    private void DropObject()
    {
        bool isValid = IsValidDropLocation(currentObject);
        if (!isValid)
        {
            currentObject.transform.position = originalPosition;
        }

        OnDrop(currentObject, isValid);
        currentObject = null;
    }
    
    private bool IsValidDropLocation(GameObject obj)
    {
        return true;
    }
    #endregion

    #region Callbacks
    private void OnPickUp(GameObject obj)
    {
        
    }

    private void OnMove(GameObject obj, Vector2 pos)
    {
        
    }

    private void OnDrop(GameObject obj, bool isValid)
    {
        if (isValid)
        {
            Debug.Log($"Dropped {obj.name} at a valid location.");
        }
        else
        {
            Debug.Log($"Dropped {obj.name} at an invalid location. Reverting.");
        }
    }
    #endregion
}
