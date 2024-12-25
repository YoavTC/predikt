using UnityEngine;

public class ProjectionCircle : SnapToGridBase
{
    [SerializeField] private LineRenderer lineRenderer;
    private Vector2 originalCoords;

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);

        originalCoords = GetSnappedPosition(transform.position);
    }

    public override void MoveTo(Vector3 pos)
    {
        if (!lineRenderer.enabled) lineRenderer.enabled = true;
        
        base.MoveTo(pos);
        lineRenderer.SetPosition(1, transform.position);
    }

    public Vector2Int GetCurrentCoords()
    {
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }
    
    public Vector2Int GetOriginalCoords()
    {
        return new Vector2Int((int) originalCoords.x, (int) originalCoords.y);
    }
}