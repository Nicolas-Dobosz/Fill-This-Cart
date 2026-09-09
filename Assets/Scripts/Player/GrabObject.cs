using UnityEngine;
using UnityEngine.InputSystem;

public class GrabObject : MonoBehaviour
{
    [Header("Grab")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float maxDistance;
    [SerializeField] private float followSpeed;
    [SerializeField] private float grabStrength;

    [Header("Throw")]
    [SerializeField] private float throwForce;
    [SerializeField] private float yBoost;

    private Rigidbody _targetRb;
    private float _holdDistance;
    private bool _isHolding;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_isHolding)
        {
            Vector3 targetPosition = targetCamera.transform.position + (targetCamera.transform.forward * _holdDistance);
            Vector3 movement = targetPosition - _targetRb.position;

            Vector3 force = (movement * grabStrength) - (_targetRb.linearVelocity * followSpeed);
            _targetRb.AddForce(force);
        }
    }
    
    public void OnGrab(InputValue ctx)
    {
        if (ctx.isPressed)
        {
            Vector3 centerScreen = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
            Ray ray = targetCamera.ScreenPointToRay(centerScreen);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
            {
                Hold(hit);
            }
        }
        else 
        {
            if (_targetRb != null)
            {
                Release();
            }
        }
    }

    private void Hold(RaycastHit hit)
    {
        _targetRb = hit.collider.attachedRigidbody;
                    
        _holdDistance = hit.distance; 

        _isHolding = true;
    }

    private void Release()
    {
        _targetRb = null;
        _isHolding = false;
    }

    public void OnThrow()
    {
        if(_isHolding)
        {
            Vector3 direction = (_targetRb.transform.position - targetCamera.transform.position).normalized + new Vector3(0f, yBoost, 0f);
            
            _targetRb.linearVelocity = Vector3.zero;
            _targetRb.AddForce(direction * throwForce, ForceMode.Impulse);

            Release();
        }
    }
}
