using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
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
    Vector3 _currentRunMovement;
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isTurningLeft;
    bool _isTurningRight;
    bool _isMovingForward;
    bool _isMovingBackward;

    // constants
    float _rotationFactorPerFrame = 15.0f;
    float _runMultiplier = 3.0f;
    int _zero = 0;

    //gravity variables
    float _gravity = -9.8f;
    float _groundedGravity = -0.5f;

    // jumpling variables
    bool _isJumpPressed = false;
    float _initialJumpVelocity;
    float _maxJumpHeight = 4.0f;
    float _maxJumpTime = 0.75f;
    bool _isJumping = false;
    bool _isJumpAnimating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

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

    void handleGravity()
    {

    }

    void handleJump()
    {
        if (!_isJumping && _characterController.isGrounded && _isJumpPressed)
        {
            _animator.SetBool(_isJumpingHash, true);
            _isJumpAnimating = true;
            _isJumping = true;
            _currentMovement.y = _initialJumpVelocity * 0.5f;
            _currentRunMovement.y = _initialJumpVelocity * 0.5f;
        }
        else if(!_isJumpPressed && _characterController.isGrounded)
        {
            _isJumping = false;
        }
    }
   
    void onMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();

        _isTurningLeft = _currentMovementInput.x < 0;
        _isTurningRight = _currentMovementInput.x > 0;
        _isMovingForward = _currentMovementInput.y > 0;
        _isMovingBackward = _currentMovementInput.y < 0;

        _isMovementPressed = _isTurningLeft || _isTurningRight || _isMovingForward || _isMovingBackward;
    }
    void onJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
    }

    void onRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void handleAnimation()
    {
        bool isWalking = _animator.GetBool(_isWalkingHash);
        bool isWalkingBackward = _animator.GetBool(_isWalkingBackwardHash);
        bool isRotatingLeft = _animator.GetBool(_isTurningLeftHash);
        bool isRotatingRight = _animator.GetBool(_isTurningRightHash);
        bool isRunning = _animator.GetBool(_isRunningHash);

        // check if player is moving forward
        if ((_isMovementPressed && _isMovingForward) && !isWalking)
        {
            _animator.SetBool(_isWalkingHash, true);
        }
        // check if player is not moving forward
        else if (!_isMovementPressed && isWalking)
        {
            _animator.SetBool(_isWalkingHash, false);
        }
        // check if player is running
        if ((_isMovementPressed && _isRunPressed) && !isRunning)
        {
            _animator.SetBool(_isRunningHash, true);
        }
        // check if player stop pressing run key
        else if ((!_isMovementPressed || !_isRunPressed) && isRunning)
        {
            _animator.SetBool(_isRunningHash, false);
        }
        // check if the player is walking backward
        if((_isMovementPressed && _isMovingBackward) && !isWalking)
        {
            _animator.SetBool(_isWalkingBackwardHash, true);
        }
        // check if player is not moving backward
        else if (!_isMovementPressed && isWalkingBackward)
        {
            _animator.SetBool(_isWalkingBackwardHash, false);
        }
        // Turning left animation
        if ((_isMovementPressed && _isTurningLeft) && !isRotatingLeft)
        {
            _animator.SetBool(_isTurningLeftHash, true);
        }
        else if(!_isMovementPressed && isRotatingLeft)
        {
            _animator.SetBool(_isTurningLeftHash, false);
        }
        // Turning right animation
        if ((_isMovementPressed && _isTurningRight) && !isRotatingRight)
        {
            _animator.SetBool(_isTurningRightHash, true);
        }
        else if (!_isMovementPressed && isRotatingRight)
        {
            _animator.SetBool(_isTurningRightHash, false);
        }
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

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();

        Vector3 moveDirection = Vector3.zero;

        if (_isMovingForward)
        {
            moveDirection = transform.forward;
        }
        else if (_isMovingBackward)
        {
            moveDirection = -transform.forward;
        }

        Vector3 finalMovement = _isRunPressed ? moveDirection * _runMultiplier : moveDirection;
        finalMovement.y = _currentMovement.y;
        _characterController.Move(finalMovement * Time.deltaTime);

        handleGravity();
        handleJump();
    }

    private void OnEnable()
    {
        _playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        _playerInput.CharacterControls.Disable();
    }
}
