using UnityEngine;

public abstract class SnapToGridBase : MonoBehaviour
{
    public virtual void MoveTo(Vector3 pos)
    {
        transform.position = GetSnappedPosition(pos);
    }

    protected Vector3 GetSnappedPosition(Vector3 pos)
    {
        return new Vector3(
            Mathf.Clamp(Mathf.Round(pos.x), 0, 8),
            Mathf.Clamp(Mathf.Round(pos.y), 0, 8),
            Mathf.Clamp(Mathf.Round(pos.z), 0, 8)
        );
    }
}