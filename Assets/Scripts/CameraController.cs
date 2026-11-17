using Unity.Cinemachine;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_OFFSET = 2f;
    private const float MAX_FOLLOW_OFFSET = 10f;

    [SerializeField] private CinemachineCamera cinemachineCamera;
    private CinemachineCameraOffset cinemachineCameraOffset;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private float zoomIncreaseAmount = 1f;
    private Vector3 followOffsetTarget;
    [SerializeField] private float zoomSpeed = 5f;

    private void Awake()
    {
        cinemachineCameraOffset = cinemachineCamera.GetComponent<CinemachineCameraOffset>();
        followOffsetTarget = cinemachineCameraOffset.Offset;
    }

    private void Update()
    {
        HandleMoveAndRotate();

        HandleZoom();

    }

    private void HandleMoveAndRotate()
    {
        // 移动（WASD）
        Vector2 inputDir = InputManager.Instance.GetCameraMoveVector();

        Vector3 moveVector = transform.forward * inputDir.y + transform.right * inputDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;

        // 旋转（Q/E）
        Vector3 rotateDir = new Vector3(0, InputManager.Instance.GetCameraRotate(), 0);

        transform.eulerAngles += rotateDir * rotateSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        //鼠标滚轮
        followOffsetTarget.y += InputManager.Instance.GetCameraZoomAmount() * zoomIncreaseAmount;

        followOffsetTarget.y = Mathf.Clamp(followOffsetTarget.y, MIN_FOLLOW_OFFSET, MAX_FOLLOW_OFFSET);
        cinemachineCameraOffset.Offset = Vector3.Lerp(cinemachineCameraOffset.Offset, followOffsetTarget, Time.deltaTime * zoomSpeed);
    }
}
