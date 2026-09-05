using UnityEngine;
using UnityEngine.InputSystem;

public class GrabObject : MonoBehaviour
{
    private Camera _camera;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private float followSpeed;
    [SerializeField] private float throwForce;
    [SerializeField] private float velocityMultiplier;
    [SerializeField] private float grabStrength;
    

    private Rigidbody _targetRb;
    private float _holdDistance;
    private bool _isHolding;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_isHolding)
        {
            // Vector3 targetPosition = _camera.transform.position + (_camera.transform.forward * _holdDistance);

            // Vector3 nextPosition = Vector3.Lerp(_targetRb.position, targetPosition, Time.fixedDeltaTime * followSpeed);
            // _targetRb.MovePosition(nextPosition);

            Vector3 targetPosition = _camera.transform.position + (_camera.transform.forward * _holdDistance);
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
            Ray ray = _camera.ScreenPointToRay(centerScreen);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
            {
                Debug.Log($"Touché : {hit.collider.name}");
                Hold(hit);
            }
            else
            {
                Debug.Log($"Rien touché");
            }
        }
        else 
        {
            if (_targetRb != null)
            {
                Debug.Log("Lâché !");
                Release();
            }
        }
    }

    private void Hold(RaycastHit hit)
    {
        _targetRb = hit.collider.attachedRigidbody;
                    
        _holdDistance = hit.distance; 

        // _targetRb.useGravity = false;
        _isHolding = true;
    }

    private void Release()
    {
        // _targetRb.useGravity = true;


        _targetRb = null;
        _isHolding = false;
    }
}
