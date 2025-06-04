using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // declare reference variables
    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _animator;

    // variables to store optimized setter/getter parameters IDs
    int _isWalkingHash;
    int _isWalkingBackwardHash;
    int _isTurningLeftHash;
    int _isTurningRightHash;

    int _isRunningHash;
    int _isJumpingHash;

    // variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isTurningLeft;
    bool _isTurningRight;
    bool _isMovingForward;
    bool _isMovingBackward;

    // constants
    float _rotationFactorPerFrame = 15.0f;
    float _runMultiplier = 5.0f;
    int _zero = 0;

    //gravity variables
    float _gravity = -9.8f;
    float _groundedGravity = -0.5f;

    // jumping variables
    bool _isJumpPressed = false;
    float _initialJumpVelocity;
    float _maxJumpHeight = 4.0f;
    float _maxJumpTime = 0.75f;
    bool _isJumping = false;
    bool _requireNewJumpPress = false;

    // state variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Animator Animator { get { return _animator; } }
    public CharacterController CharacterController { get { return _characterController; } set{ _characterController = value; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public Transform Transform { get { return transform; } }

    public bool IsJumpPressed {  get { return _isJumpPressed; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public bool RequireNewJumpPress { get { return  _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
    public bool isJumping { get { return _isJumping; } set { _isJumping = value; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    public float InitialJumpVelocity { get { return _initialJumpVelocity; } set { _initialJumpVelocity = value; } }
    public float GroundedGravity { get { return _groundedGravity; } set { _groundedGravity = value; } }
    public float Gravity { get { return _gravity; } set { _gravity = value; } }

    public bool IsMovementPressed { get  { return _isMovementPressed; } set { _isMovementPressed = value; } }
    public bool IsRunPressed { get { return _isRunPressed; } set {_isRunPressed = value; } }
    public bool IsTurningLeft { get {  return _isTurningLeft; } set { _isTurningLeft = value; } }
    public bool IsTurningRight { get { return _isTurningRight; } set { _isTurningRight = value; } }
    public bool IsMovingForward { get { return _isMovingForward; } set { _isMovingForward = value; } }
    public bool IsMovingBackward { get { return _isMovingBackward; } set { _isMovingBackward = value; } }

    public int IsWalkingHash { get { return _isWalkingHash; } set { _isWalkingHash = value; } }
    public int IsWalkingBackwardHash { get { return _isWalkingBackwardHash; } set { _isWalkingBackwardHash = value; } }
    public int IsTurningLeftHash { get { return _isTurningLeftHash; } set { _isTurningLeftHash = value; } }
    public int IsTurningRightHash { get { return _isTurningRightHash; } set { _isTurningRightHash = value; } }
    public int IsRunningHash { get { return _isRunningHash; } set { _isRunningHash = value; } }
    public float RunMultiplier { get { return _runMultiplier; } }


/********************************************************************************************************************/

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        _isWalkingHash = Animator.StringToHash("isWalking");
        _isWalkingBackwardHash = Animator.StringToHash("isWalkingBackward");
        _isTurningLeftHash = Animator.StringToHash("isTurningLeft");
        _isTurningRightHash = Animator.StringToHash("isTurningRight");

        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");

        // reads when the buttons are pressed or not
        _playerInput.CharacterControls.Move.started += onMovementInput;
        _playerInput.CharacterControls.Move.canceled += onMovementInput;
        _playerInput.CharacterControls.Move.performed += onMovementInput;

        _playerInput.CharacterControls.Run.started += onRun;
        _playerInput.CharacterControls.Run.canceled += onRun;

        _playerInput.CharacterControls.Jump.started += onJump;
        _playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
    }

    void setupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        _currentState.UpdateStates();

        _characterController.Move(_appliedMovement * Time.deltaTime);
    }

    void handleRotation()
    {
        float turnSpeed = _rotationFactorPerFrame * Time.deltaTime;

        if (_isTurningLeft)
        {
            transform.Rotate(0, -turnSpeed * 10, 0);
        }
        else if (_isTurningRight)
        {
            transform.Rotate(0, turnSpeed * 10, 0);
        }
    }

    void onMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();

        _isTurningLeft = false;
        _isTurningRight = false;
        _isMovingForward = false;
        _isMovingBackward = false;

        // luego evaluamos el nuevo input
        if (_currentMovementInput.x < 0)
        {
            _isTurningLeft = true;
        }
        else if (_currentMovementInput.x > 0)
        {
            _isTurningRight = true;
        }

        if (_currentMovementInput.y > 0)
        {
            _isMovingForward = true;
        }
        else if (_currentMovementInput.y < 0)
        {
            _isMovingBackward = true;
        }

        _isMovementPressed = _isTurningLeft || _isTurningRight || _isMovingForward || _isMovingBackward;
    }
    void onJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }

    void onRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }

    public void SetInputEnabled(bool value)
    {
        if (value)
            _playerInput.CharacterControls.Enable();
        else
            _playerInput.CharacterControls.Disable();
    }
}
