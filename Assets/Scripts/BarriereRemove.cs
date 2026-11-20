using UnityEngine;

public class BarriereRemove : MonoBehaviour
{
[SerializeField] private Material newMaterial;

    private Transform wallTransform;

    private void Start()
    {
        wallTransform = transform.Find("Wall");
    }
    public void Remove()
    {
        Transform[] descendants = wallTransform.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in descendants)
        {
            if (child == wallTransform) continue;

            if (child.TryGetComponent(out BoxCollider boxCollider))
            {
                Destroy(boxCollider);
            }

            if (newMaterial != null && child.TryGetComponent(out Renderer rend))
            {
                rend.material = newMaterial;
            }
        }
    }
}
