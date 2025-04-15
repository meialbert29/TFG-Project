using UnityEngine;

public class twoDimensionalAnimationStateController : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;
    private Transform cameraTransform;
    
    //Camera Rotation
    public CameraChange cameraChange;
    public float mouseSensitivity = 2f;
    private float verticalRotation = 0f;

    float velocityZ = 0.0f;
    float velocityX = 0.0f;
    public float accelaration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;
    public float maximumRotationVelocity = 250f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // search the gameobject this script is attached to and get the animator component & the character controller
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        cameraChange = FindAnyObjectByType<CameraChange>();

        if(cameraChange != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // hides the mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // get key input from player
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);
        bool runPressed = Input.GetKey("left shift");

        // set current maxVelocity
        // if runPressed is true --> currentMaxVelocity = maximumRunVelocity
        // if runPressed is false --> currentMaxVelocity = maximumWalkVelocity
        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        changeVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
        lockOrResetVelocity(forwardPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
        changeRotation(leftPressed, rightPressed, runPressed, maximumRotationVelocity);

        // set parameters to the local variable values
        animator.SetFloat("Velocity Z", velocityZ);
        animator.SetFloat("Velocity X", velocityX);

        transform.Translate(0, 0, velocityZ * Time.deltaTime * currentMaxVelocity);
    }

    // handle acceleration and deceleration
    private void changeVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // if player presses W, increase velocity in z direction
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * accelaration;
        }

        // if player presses A, increase velocity in left direction
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * accelaration;
        }

        // if player presses D, increase velocity in left direction
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * accelaration;
        }

        // decrease velocityZ
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }

        // increase velocityX if left is not pressed and velocityX < 0
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }

        // decrease velocityX if right is not pressed and velocityX > 0
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
    }

    // handle reset and locking of velocity
    private void lockOrResetVelocity(bool forwardPressed, bool leftPressed, bool rightPressed, bool runPressed, float currentMaxVelocity)
    {
        // reset velocityZ
        if (!forwardPressed && velocityZ <= 0.0f)
        {
            velocityZ = 0.0f;
        }

        // reset velocityX
        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
        {
            velocityX = 0.0f;
        }

        // FIX FORWARD VELOCITY
        /*-----------------------------------------------------*/

        // lock forward
        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            // round to the currentMaxVelocity if within offset
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        // round to the currentMaxVelocity if within offset
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
        {
            velocityZ = currentMaxVelocity;
        }

        // FIX LEFT VELOCITY
        /*-----------------------------------------------------*/
        // lock left
        if (leftPressed && runPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
            // round to the currentMaxVelocity if within offset
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - 0.05f))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        // round to the currentMaxVelocity if within offset
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + 0.05f))
        {
            velocityX = -currentMaxVelocity;
        }

        // FIX RIGHT VELOCITY
        /*-----------------------------------------------------*/
        // lock right
        if (rightPressed && runPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        // decelerate to the maximum walk velocity
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
            // round to the currentMaxVelocity if within offset
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + 0.05f))
            {
                velocityX = currentMaxVelocity;
            }
        }
        // round to the currentMaxVelocity if within offset
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - 0.05f))
        {
            velocityX = currentMaxVelocity;
        }
    }

    private void changeRotation(bool leftPressed, bool rightPressed, bool runPressed, float maximumRotationVelocity)
    {
        float rotationDirection = 0f;

        if (leftPressed)
        {
            rotationDirection = -1f;
        }
        else if (rightPressed)
        {
            rotationDirection = 1f;
        }

        if (rotationDirection != 0f)
        {
            float rotationAmount = rotationDirection * maximumRotationVelocity * Time.deltaTime;
            transform.Rotate(0, rotationAmount, 0);
        }
    }
}
