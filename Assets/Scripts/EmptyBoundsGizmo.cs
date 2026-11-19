using UnityEngine;

public class EmptyBoundsGizmo : MonoBehaviour
{
    [SerializeField] private Color wireColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = wireColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
    }
}