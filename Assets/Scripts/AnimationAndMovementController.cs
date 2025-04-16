using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationAndMovementController : MonoBehaviour
{
    // declare reference variables
    PlayerInput playerInput;
    CharacterController characterController;
    Animator animator;

    // variables to store optimized setter/getter parameters IDs
    int isWalkingHash;
    int isRunningHash;
    int isJumpingHash;
    int isWalkingBackwardHash;

    // variables to store player input values
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovement;
    bool isMovementPressed;
    bool isRunPressed;
    bool isTurningLeft;
    bool isTurningRight;
    bool isMovingForward;
    bool isMovingBackward;

    // constants
    float rotationFactorPerFrame = 15.0f;
    float runMultiplier = 3.0f;
    int zero = 0;
    public float turnSpeed = 100f; // grados por segundo

    //gravity variables
    float gravity = -9.8f;
    float groundedGravity = -0.5f;

    // jumpling variables
    bool isJumpPressed = false;
    float initialJumpVelocity;
    float maxJumpHeight = 4.0f;
    float maxJumpTime = 0.75f;
    bool isJumping = false;
    bool isJumpAnimating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isJumpingHash = Animator.StringToHash("isJumping");
        isWalkingBackwardHash = Animator.StringToHash("isWalkingBackward");

        // reads when the buttons are pressed or not
        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;

        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;

        playerInput.CharacterControls.Jump.started += onJump;
        playerInput.CharacterControls.Jump.canceled += onJump;

        setupJumpVariables();
    }

    void setupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    void handleJump()
    {
        if (!isJumping && characterController.isGrounded && isJumpPressed)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpAnimating = true;
            isJumping = true;
            currentMovement.y = initialJumpVelocity * 0.5f;
            currentRunMovement.y = initialJumpVelocity * 0.5f;
        }
        else if(!isJumpPressed && characterController.isGrounded)
        {
            isJumping = false;
        }
    }

    void onJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();

    }

    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void handleRotation()
    {
        //Vector3 positionToLookAt;
        //// the change in position the character should point to
        //positionToLookAt.x = currentMovement.x;
        //positionToLookAt.y = 0.0f;
        //positionToLookAt.z = 0.0f;

        //// the current rotation of the character
        //Quaternion currentRotation = transform.rotation;

        //if (isMovementPressed)
        //{
        //    // creates a new rotation based on where the player is currently pressing
        //    Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
        //    // rotate the character to face the positionToLookAt
        //    transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        //}

        float rotationThisFrame = turnSpeed * Time.deltaTime;

        if (isTurningLeft)
        {
            transform.Rotate(0, -rotationThisFrame, 0);
        }
        else if (isTurningRight)
        {
            transform.Rotate(0, rotationThisFrame, 0);
        }
    }
    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        currentMovement.x = currentMovementInput.x * runMultiplier;
        currentMovement.z = currentMovementInput.y * runMultiplier;
        
        isTurningLeft = currentMovementInput.x < 0;
        isTurningRight = currentMovementInput.x > 0;
        isMovingForward = currentMovementInput.y > 0;
        isMovingBackward = currentMovementInput.y < 0;

        isMovementPressed = currentMovementInput.x != zero || currentMovementInput.y != zero || isTurningLeft || isTurningRight;
    }

    void handleAnimation()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalkingBackward = animator.GetBool(isWalkingBackwardHash);

        if ((isMovementPressed && isMovingForward) && !isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool(isWalkingHash, false);
        }
        if((isMovementPressed && isRunPressed) && !isRunning)
        {
            animator.SetBool(isRunningHash, true);
        }
        else if((!isMovementPressed || !isRunPressed) && isRunning)
        {
            animator.SetBool(isRunningHash, false);
        }
        if((isMovementPressed && isMovingBackward) && !isWalkingBackward)
        {
            animator.SetBool(isWalkingBackwardHash, true);
        }
        else if(!isMovementPressed && isWalkingBackward)
        {
            animator.SetBool(isWalkingBackwardHash, false);
        }
    }

    void handleGravity()
    {
        bool isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;

        // apply proper gravity depending on if the character is grounded or not
        if (characterController.isGrounded)
        {
            if (isJumpAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpAnimating = false;
            }
            
            currentMovement.y = groundedGravity;
            currentRunMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            float nextYVelocity = Mathf.Max((previousYVelocity + newYVelocity) * 0.5f, -20.0f);
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
        else
        {
            float previousYVelocity = currentMovement.y;
            float newYVelocity = currentMovement.y + (gravity * Time.deltaTime);
            float nextYVelocity = (previousYVelocity + newYVelocity) * 0.5f;
            currentMovement.y = nextYVelocity;
            currentRunMovement.y = nextYVelocity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleRotation();
        handleAnimation();

        if (isMovingForward)
        {
            Vector3 moveDirection = transform.forward;
            characterController.Move(moveDirection * Time.deltaTime * (isRunPressed ? runMultiplier : 1f));
        }
        else if(isMovingBackward)
        {
            Vector3 moveDirection = transform.forward;
            characterController.Move( -moveDirection * Time.deltaTime);
        }

        if (isRunPressed)
        {
            characterController.Move(currentRunMovement * Time.deltaTime);
        }
        else
        {
            characterController.Move(currentMovement * Time.deltaTime);
        }

        handleGravity();
        handleJump();
    }

    private void OnEnable()
    {
        playerInput.CharacterControls.Enable();
    }

    private void OnDisable()
    {
        playerInput.CharacterControls.Disable();
    }
}
