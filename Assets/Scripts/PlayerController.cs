using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using static Configs;
public class PlayerController : MonoBehaviour
{
    // 필요한 컴포넌트
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] float crouchSpeed;
    [SerializeField] float lookSensitivity;
    [SerializeField] float cameraRotationLimitX; // 고개가 회전하는 정도 제한
    [SerializeField] Camera playerCamera; // 1인칭 카메라
    GunController theGunController;
    Crosshair crosshair;
    StatusController statusController;
    Vector3 _moveDirection;
    Rigidbody _myRigidBody;
    CapsuleCollider _myCapsuleCollider;
    InputSystem_Actions _inputAction;
    PlayerState _currentState = PlayerState.None;
    float _currentCameraRotationX;
    float _applySpeed;

    // 앉기 관련 변수
    [SerializeField] float CrouchPosY;
    float _originPosY;
    float _applyCrouchPosY;
    Coroutine _CrouchCoroutine;

    // 상태 변수
    bool _isRun;
    bool _isCrouch;
    bool _isWalk;
    bool _isGround;
    bool pauseCameraRotation;
    Vector3 lashPos; // 움직임 체크 변수수
    void Awake()
    {
        _inputAction = new InputSystem_Actions();
        _myRigidBody = GetComponent<Rigidbody>();
        _myCapsuleCollider = GetComponent<CapsuleCollider>();
        theGunController = FindAnyObjectByType<GunController>();
        crosshair = FindAnyObjectByType<Crosshair>();
        statusController = FindAnyObjectByType<StatusController>();
    }
    void Start()
    {
        _applySpeed = walkSpeed;
        _applyCrouchPosY = _originPosY = playerCamera.transform.localPosition.y;
    }
    void OnEnable()
    {
        _inputAction.Player.Enable();
        _inputAction.Player.Move.performed += OnMoved;
        _inputAction.Player.Move.canceled += OnStopped;
        _inputAction.Player.Run.started += RunStart;
        _inputAction.Player.Run.canceled += RunCancel;
        _inputAction.Player.Crouch.started += CrouchStart;
        _inputAction.Player.Crouch.canceled += CrouchCancel;
        _inputAction.Player.Jump.started += OnJump;
    }
    void OnDisable()
    {
        _inputAction.Player.Move.performed -= OnMoved;
        _inputAction.Player.Move.canceled -= OnStopped;
        _inputAction.Player.Run.started -= RunStart;
        _inputAction.Player.Run.canceled -= RunStart;
        _inputAction.Player.Jump.started -= OnJump;
        _inputAction.Player.Crouch.started -= CrouchStart;
        _inputAction.Player.Crouch.canceled -= CrouchStart;
        _inputAction.Player.Disable();        
    }
    void Update()
    {
        if(_currentState == PlayerState.Move)
        {
            Move();
        }
        RotateCameraVertical();
        RotatePlayer();
    }
    void FixedUpdate()
    {
        _isGround = IsGround();
    }
    void OnMoved(InputAction.CallbackContext context)
    {
        if(context.ReadValue<Vector2>().magnitude > Mathf.Epsilon && _currentState == PlayerState.None)
        {
            _currentState = PlayerState.Move;
            if(!_isRun && !_isCrouch && _isGround)
            {
                _isWalk = true;
                crosshair.WalkingAnimation(_isWalk);
            }
        }
    }

    void OnStopped(InputAction.CallbackContext context)
    {
        if(_currentState == PlayerState.Move)
        {
            _currentState = PlayerState.None;
            if(!_isRun && !_isCrouch && _isGround)
            {
                _isWalk = false;
                crosshair.WalkingAnimation(_isWalk);
            }
        }
    }

    void RunStart(InputAction.CallbackContext context)
    {
        SetRun(true);

        theGunController.CancelFineSight();
        SetCrouch(false); // 앉기 취소
    }

    void RunCancel(InputAction.CallbackContext context)
    {
        SetRun(false);
    }

    void SetRun(bool isRun)
    {
        if(_isRun == isRun && statusController.GetCurrentSP() <= 0) return;
        _isRun = isRun;
        crosshair.RunningAnimation(_isRun);
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if(_isGround && statusController.GetCurrentSP() > 0)
        {
            statusController.DecreaseStamina(100);
            _myRigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    void CrouchStart(InputAction.CallbackContext context)
    {
        SetCrouch(true);
    }

    void CrouchCancel(InputAction.CallbackContext context)
    {
        SetCrouch(false);
    }

    void SetCrouch(bool isCrouch)
    {
        if(_isCrouch == isCrouch) return;
        _isCrouch = isCrouch;
        crosshair.CrouchingAnimation(_isCrouch);

        _applyCrouchPosY = _isCrouch ? CrouchPosY : _originPosY;
        if(_CrouchCoroutine != null)
        {
            StopCoroutine(_CrouchCoroutine);
        }
        _CrouchCoroutine = StartCoroutine(CrouchCoroutine());
    }

    bool IsGround()
    {
        bool isGround = Physics.Raycast(transform.position, Vector3.down, _myCapsuleCollider.bounds.extents.y + 0.3f);
        crosshair.JumpAnimation(!isGround);
        return isGround;
    }

    void Move()
    {
        Vector2 inputVector = _inputAction.Player.Move.ReadValue<Vector2>();
        Vector3 directionVertical = transform.forward * inputVector.y;
        Vector3 directionHorizontal = transform.right * inputVector.x;
        _moveDirection = (directionVertical + directionHorizontal).normalized;

        _applySpeed = _isCrouch || _isRun? (_isCrouch? crouchSpeed : runSpeed) : walkSpeed;
        if(_isRun)
        {
            statusController.DecreaseStamina(1);
            if(statusController.GetCurrentSP() <= 0)
            {
                SetRun(false);
            }
        }

        _myRigidBody.MovePosition(transform.position + _moveDirection * _applySpeed * Time.deltaTime);
    }   

    // 플레이어 수평 회전 - 시선도 수평으로 회전하게 된다
    void RotatePlayer()
    {
        float yRotation = _inputAction.Player.Look.ReadValue<Vector2>().x * lookSensitivity;
        Vector3 PlayerRotation = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        _myRigidBody.MoveRotation(_myRigidBody.rotation * Quaternion.Euler(PlayerRotation)); 
    }

    // 플레이어 시점 상하 회전 - 마우스 위아래로 움직일 때 카메라도 위아래로 움직여야 한다
    void RotateCameraVertical()
    {   
        if(!pauseCameraRotation)
        {
            // X축 기준으로 회전시키면 상하로 움직이므로, 인풋을 x축 회전에 적용한다
            float _xRotation = _inputAction.Player.Look.ReadValue<Vector2>().y * lookSensitivity;

            // 회전이 양수면 아래를 바라보므로 값을 빼준다.
            _currentCameraRotationX -= _xRotation;
            _currentCameraRotationX = Mathf.Clamp(_currentCameraRotationX, -cameraRotationLimitX, cameraRotationLimitX);
            
            playerCamera.transform.localEulerAngles = new Vector3(_currentCameraRotationX, 0f, 0f);
        }
    }

    public IEnumerator TreeLookCoroutine(Vector3 target)
    {
        pauseCameraRotation = true;
        
        Quaternion direction = Quaternion.LookRotation(target - playerCamera.transform.position);
        Vector3 eulerValue = direction.eulerAngles;
        float destinationX = eulerValue.x;

        while(Mathf.Abs(destinationX - _currentCameraRotationX) >= 0.5f)
        {
            eulerValue = Quaternion.Lerp(playerCamera.transform.rotation, direction, 0.2f).eulerAngles;
            playerCamera.transform.localRotation = Quaternion.Euler(eulerValue.x, 0f, 0f);
            _currentCameraRotationX = playerCamera.transform.localEulerAngles.x;
            yield return null;
        }

        pauseCameraRotation = false;
    }
    IEnumerator CrouchCoroutine()
    {
        float posY = playerCamera.transform.localPosition.y;

        while(Mathf.Abs(posY - _applyCrouchPosY) > 0.1f)
        {
            posY = Mathf.MoveTowards(posY, _applyCrouchPosY, 5f * Time.deltaTime);
            playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, posY,
                                                                playerCamera.transform.localPosition.z);
            yield return null;
        }
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, _applyCrouchPosY,
                                                            playerCamera.transform.localPosition.z);        
    }
}
