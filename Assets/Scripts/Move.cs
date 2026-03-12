using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Move : MonoBehaviour
{
    [Header("移动")]
    [Tooltip("行走速度")]
    public float walkSpeed = 5f;
    [Tooltip("重力")]
    public float gravity = -18f;

    [Header("视角")]
    [Tooltip("鼠标水平灵敏度")]
    public float mouseSensitivityX = 2f;
    [Tooltip("鼠标垂直灵敏度")]
    public float mouseSensitivityY = 1.5f;
    [Tooltip("垂直视角下限（度）")]
    public float pitchMin = -85f;
    [Tooltip("垂直视角上限（度）")]
    public float pitchMax = 85f;

    CharacterController _controller;
    float _velocityY;
    float _pitch; // 上下视角（欧拉角 X）

    void Start()
    {
        _controller = GetComponent<CharacterController>();

        // 第一人称：锁定并隐藏鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // 初始化垂直视角为当前 X 欧拉角，避免开局猛抬头/低头
        _pitch = transform.eulerAngles.x;
        if (_pitch > 180f) _pitch -= 360f;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY;

        // 水平：绕世界 Y 轴旋转（左右转）
        transform.Rotate(0f, mouseX, 0f);

        // 垂直：只改俯仰，限制范围
        _pitch -= mouseY;
        _pitch = Mathf.Clamp(_pitch, pitchMin, pitchMax);
        transform.localEulerAngles = new Vector3(_pitch, transform.eulerAngles.y, 0f);
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D → -1/1
        float v = Input.GetAxisRaw("Vertical");   // S/W → -1/1

        // 相对摄像机朝向的移动方向（只取 XZ，不往天上走）
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();
        Vector3 right = transform.right;

        Vector3 moveDir = (forward * v + right * h).normalized;
        Vector3 velocity = moveDir * walkSpeed;

        // 重力
        if (_controller.isGrounded && _velocityY < 0f)
            _velocityY = -2f; // 小向下速度贴地
        _velocityY += gravity * Time.deltaTime;
        velocity.y = _velocityY;

        _controller.Move(velocity * Time.deltaTime);
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
