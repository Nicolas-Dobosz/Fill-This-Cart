using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float Speed;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float topClamp;
    [SerializeField] private float bottomClamp;
    private Vector2 _movement;
    private Vector2 _look;
    private Rigidbody _rigidbody;
    private float _xRotation;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }
    private void Update()
    {
        HandleLook();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_movement.magnitude >= 0.01f)
        {
            Vector3 moveDirection = transform.TransformDirection(new Vector3(_movement.x, 0f, _movement.y));
            _rigidbody.AddForce(moveDirection * Speed, ForceMode.Force);
        }
    }
    private void HandleLook()
    {
        float mouseX = _look.x * mouseSensitivity * Time.deltaTime;
        float mouseY = _look.y * mouseSensitivity * Time.deltaTime;

        // Vertical pitch (Look up/down - affects Camera only)
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, bottomClamp, topClamp);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }

        // Horizontal yaw (Look left/right - rotates Player Body)
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
}
