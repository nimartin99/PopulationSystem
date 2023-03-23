using Cinemachine;
using UnityEngine;

public class CameraSystem : MonoBehaviour {
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float rotateSpeed = 300f;
    [SerializeField] private float dragPanSpeed = 2f;
    [SerializeField] private bool useEdgeScrolling = true;
    [SerializeField] private bool useDragPan = false;
    [SerializeField] private float followOffsetMin = 20f;
    [SerializeField] private float followOffsetMax = 110f;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float zoomAmount = 3f;
    
    private bool _dragPanMoveActive;
    private Vector2 _lastMousePosition;

    private Vector3 _followOffset;

    private void Awake() {
        _followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }

    // Update is called once per frame
    void Update() {
        HandleCameraMovementButtons();
        if (useEdgeScrolling) {
            HandleCameraMovementEdgeScrolling();HandleCameraMovementEdgeScrolling();
        }
        if (useDragPan) {
            HandleCameraMovementDragPan();
        }
        HandleCameraRotation();
        HandleCameraZoom();
    }

    private void HandleCameraMovementButtons() {
        Vector3 inputDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDirection.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDirection.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDirection.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDirection.x = +1f;

        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
    }

    private void HandleCameraMovementEdgeScrolling() {
        Vector3 inputDirection = new Vector3(0, 0, 0);
        
        int edgeScrollSize = 20;
        if (Input.mousePosition.x < edgeScrollSize) inputDirection.x = -1f;
        if (Input.mousePosition.y < edgeScrollSize) inputDirection.z = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDirection.x = +1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDirection.z = +1f;
        
        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
    }

    private void HandleCameraMovementDragPan() {
        Vector3 inputDirection = new Vector3(0, 0, 0);
        
        if (Input.GetMouseButtonDown(1)) {
            _dragPanMoveActive = true;
        }

        if (Input.GetMouseButtonUp(1)) {
            _dragPanMoveActive = false;
        }

        if (_dragPanMoveActive) {
            Vector2 mouseMovementDelta = (Vector2) Input.mousePosition - _lastMousePosition;
            inputDirection.x = mouseMovementDelta.x * dragPanSpeed;
            inputDirection.z = mouseMovementDelta.y * dragPanSpeed;
            _lastMousePosition = Input.mousePosition;
        }
        
        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        transform.position += moveDirection * (moveSpeed * Time.deltaTime);
    }

    private void HandleCameraRotation() {
        float rotateDirection = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateDirection = +1f;
        if (Input.GetKey(KeyCode.E)) rotateDirection = -1f;

        transform.eulerAngles += new Vector3(0, rotateDirection * rotateSpeed * Time.deltaTime, 0);
    }

    private void HandleCameraZoom() {
        if (Input.mouseScrollDelta.y > 0) {
            _followOffset.y -= zoomAmount;
        }
        
        if (Input.mouseScrollDelta.y < 0) {
            _followOffset.y += zoomAmount;
        }

        _followOffset.y = Mathf.Clamp(_followOffset.y, followOffsetMin, followOffsetMax);
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = 
            Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset,
            _followOffset, Time.deltaTime * zoomSpeed);
    }
}
