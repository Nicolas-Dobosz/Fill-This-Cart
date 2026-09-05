using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float topClamp;
    [SerializeField] private float bottomClamp;
    [SerializeField] private GameObject caddie;
    private Vector2 _movement;
    private Vector2 _look;
    private Rigidbody _rigidbody;
    private float _xRotation;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        HandleLook();
    }

    private void FixedUpdate()
    {
        if (_movement.magnitude >= 0.01f)
        {
            Vector3 moveDirection = transform.TransformDirection(new Vector3(_movement.x, 0f, _movement.y));
            _rigidbody.AddForce(moveDirection * speed);
        }
    }
    private void HandleLook()
    {
        float mouseX = _look.x * mouseSensitivity * Time.deltaTime;
        float mouseY = _look.y * mouseSensitivity * Time.deltaTime;

        // Look up/down - affects Camera only
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, bottomClamp, topClamp);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }

        // Look left/right - rotates Player Body
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCaddie()
    {

    }

    public void OnInteract(InputValue ctx)
    {
        if (ctx.isPressed)
        {
            HandleCaddie();
        }
    }

    public void OnMove(InputValue ctx)
    {
        _movement = ctx.Get<Vector2>();
    }
    public void OnLook(InputValue ctx)
    {
        _look = ctx.Get<Vector2>();
    }
}
