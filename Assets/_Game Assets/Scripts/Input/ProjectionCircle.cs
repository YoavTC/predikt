using System;
using UnityEngine;

public class ProjectionCircle : SnapToGridBase
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float lineProjectionAnimationSpeed;
    private Vector2 originalCoords;
    private Material lineMaterial;

    private void Start()
    {
        lineMaterial = lineRenderer.material;
        
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);

        originalCoords = GetSnappedPosition(transform.position);
    }

    private void Update()
    {
        lineRenderer.material.mainTextureOffset -= new Vector2(lineProjectionAnimationSpeed * Time.deltaTime, 0f);
        lineRenderer.material = lineMaterial;
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