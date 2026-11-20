using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("Grab Line)")]
    [SerializeField] private Transform lineOrigin;
    [SerializeField] private int curveResolution;
    [SerializeField] private float curveSag;
    [SerializeField] private float velocityLag;
    private LineRenderer _lineRenderer;

    private Rigidbody _targetRb;
    private Vector3 _grabPointOffset;
    private float _holdDistance;
    private bool _isHolding = false;
    private bool _isCaddie = false;
    private float _yAngleOffset;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = curveResolution;
        _lineRenderer.enabled = false;
    }

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

            DrawGrabCurve();
        }
    }

    private void DrawGrabCurve()
    {
        _lineRenderer.enabled = true;

        Vector3 startPoint = lineOrigin != null 
            ? lineOrigin.position 
            : targetCamera.transform.position + targetCamera.transform.right * 0.3f - targetCamera.transform.up * 0.3f;

        Vector3 endPoint = _targetRb.position + targetCamera.transform.TransformVector(_grabPointOffset);

        // 3. Point de contrôle au milieu avec un effet de gravité et d'inertie
        Vector3 midPoint = Vector3.Lerp(startPoint, endPoint, 0.5f);
        midPoint += Vector3.down * curveSag;
        midPoint -= _targetRb.linearVelocity * velocityLag;

        // 4. Calcul des points de la courbe de Bézier
        for (int i = 0; i < curveResolution; i++)
        {
            float t = i / (float)(curveResolution - 1);
            Vector3 point = CalculateQuadraticBezierPoint(t, startPoint, midPoint, endPoint);
            _lineRenderer.SetPosition(i, point);
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }
    
    public void OnGrab(InputValue ctx)
    {
        if (ctx.isPressed)
        {
            // Recup le milieu de chaque camera
            Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, hitLayers))
            {
                if (hit.transform.CompareTag("InteractableButton"))
                {
                    if (hit.collider.TryGetComponent(out InteractableButton button))
                    {
                        button.OnPress();
                    }
                }
                else
                {
                    Hold(hit);
                }
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
        _lineRenderer.enabled = false;
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
