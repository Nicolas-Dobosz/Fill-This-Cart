using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float sprintMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravityMultiplier;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float topClamp;
    [SerializeField] private float bottomClamp;
    
    private Vector2 _movement;
    private Vector2 _look;
    private Rigidbody _rigidbody;
    private float _xRotation;
    private float _playerHeight;
    private bool _isSprinting;
    private float _currentSpeed;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _playerHeight = transform.localScale.y;
    }
    private void Update()
    {
        HandleLook();
    }

    private void FixedUpdate()
    {
        _currentSpeed = speed * (_isSprinting ? sprintMultiplier : 1f);

        if (_movement.magnitude >= 0.01f)
        {
            Vector3 moveDirection = transform.TransformDirection(new Vector3(_movement.x, 0f, _movement.y));
            _rigidbody.linearVelocity = new Vector3(moveDirection.x * _currentSpeed, _rigidbody.linearVelocity.y, moveDirection.z * _currentSpeed);
        }
        else
        {
            if(IsGrounded())
            {
                _rigidbody.linearVelocity = new Vector3(0f, _rigidbody.linearVelocity.y, 0f);
            }
        }

        // custom gravity
        if (!IsGrounded())
        {
            _rigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (gravityMultiplier - 1) * Time.fixedDeltaTime;
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

    public void OnMove(InputValue ctx)
    {
        _movement = ctx.Get<Vector2>();
    }
    public void OnLook(InputValue ctx)
    {
        _look = ctx.Get<Vector2>();
    }
    public void OnJump(InputValue ctx)
    {
        if (ctx.isPressed && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public void OnSprint(InputValue ctx)
    {
        _isSprinting = ctx.isPressed;
    }
    private bool IsGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _playerHeight + 0.1f))
        {
            return hit.collider.gameObject != gameObject;
        }
        return false;
    }
    
}
