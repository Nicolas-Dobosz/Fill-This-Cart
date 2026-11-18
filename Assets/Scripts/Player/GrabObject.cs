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
    private Vector3 _grabPointOffset;
    private float _holdDistance;
    private bool _isHolding = false;
    private bool _isCaddie = false;
    private float _yAngleOffset;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_isHolding)
        {
            Vector3 targetGrabPoint = targetCamera.transform.position + (targetCamera.transform.forward * _holdDistance);
            Vector3 targetCenterPosition = targetGrabPoint - targetCamera.transform.TransformVector(_grabPointOffset);

            Vector3 movement = targetCenterPosition - _targetRb.position;
            Vector3 force = (movement * grabStrength) - (_targetRb.linearVelocity * followSpeed);

            _targetRb.AddForce(force);

            if (_isCaddie)
            {
                // Conserve les angles actuels sur X et Z
                Vector3 currentEuler = _targetRb.rotation.eulerAngles;

                // Calcule le nouvel angle Y en appliquant le décalage initial
                float targetYAngle = targetCamera.transform.eulerAngles.y + _yAngleOffset;

                // Applique la rotation mise à jour uniquement sur Y
                Quaternion targetRotation = Quaternion.Euler(currentEuler.x, targetYAngle, currentEuler.z);
                _targetRb.MoveRotation(targetRotation);
            }
        }
    }
    
    public void OnGrab(InputValue ctx)
    {
        if (ctx.isPressed)
        {
            // Recup le milieu de chaque camera
            Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

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

        Vector3 offsetInWorld = hit.point - _targetRb.position;
        _grabPointOffset = targetCamera.transform.InverseTransformVector(offsetInWorld);

        if(_targetRb.CompareTag("Caddie"))
        {
            _isCaddie = true;
            _yAngleOffset = _targetRb.rotation.eulerAngles.y - targetCamera.transform.eulerAngles.y;
        }

        _isHolding = true;
    }

    private void Release()
    {
        _targetRb = null;
        _isHolding = false;
        _isCaddie = false;
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
