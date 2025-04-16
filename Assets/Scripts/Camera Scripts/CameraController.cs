using UnityEngine;

public class CameraController : MonoBehaviour
{
    float rotX; // stores rotation in the X axis
    float rotY; // stores rotation in the Y axis
    public float mouseSmoothness = 2.5f;
    public float mouseRotAcceleration = 20;
    public float followSpeed = 100;
    public float maxPitch = 80;
    public float fovAim;
    public float fovNorm;
    public Transform yaw;
    public Transform pitch;
    public Transform shake;
    public Transform target;
    public Transform globalLookTarget;
    public Vector3 camOffset = new Vector3(0,0,0);
    public Vector3 aimOffset = new Vector3(0, 0, 0);
    private Vector3 currCamOffset;
    Camera cam;
    public LayerMask layerMask;
    bool aiming = false;

    // camera recoil parameters
    public float incrementRate = 0.1f;
    public float increment = 1;
    public float currTimeRecoil = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotX = pitch.rotation.eulerAngles.x;
        rotY = yaw.rotation.eulerAngles.y;
        cam = GetComponent<Camera>();
    }

    void getMouseParam()
    {
        // gets delta mouse change
        float mouseX = Input.GetAxis(ControllerStatics.MouseX);
        float mouseY = Input.GetAxis(ControllerStatics.MouseY);

        // multiple delta mouse parameters with the time taken per frame and
        // amplify it with the mouse smoothness and add to it to the existing
        // pitch angle stored in rotX
        rotX += mouseY * mouseSmoothness * Time.deltaTime;
        rotY += mouseX * mouseSmoothness * Time.deltaTime;
        rotX = Mathf.Clamp(rotX, - maxPitch, maxPitch);

        if(Mathf.Abs(rotY) > 360)
        {
            rotY = 0;
        }
    }

    void cameraRotation()
    {
        // smoothly rotate the Yaw in the Y axis with the new updated angles for Y
        Quaternion yAngle = Quaternion.Euler(yaw.rotation.eulerAngles.x, rotY, yaw.rotation.eulerAngles.z);
        yaw.rotation = Quaternion.Slerp(yaw.rotation, yAngle, Time.deltaTime * mouseRotAcceleration);

        // smoothly rotate the Yaw in the X axis with the new updated angles for X
        Quaternion xAngle = Quaternion.Euler(rotX, yaw.rotation.eulerAngles.y, yaw.rotation.eulerAngles.z);
        pitch.rotation = Quaternion.Slerp(pitch.rotation, xAngle, Time.deltaTime * mouseRotAcceleration);
    }

    void cameraTranslation()
    {
        // follow player
        yaw.transform.position = Vector3.Lerp(yaw.transform.position, target.transform.position, Time.deltaTime * followSpeed);

        if (aiming)
        {
            currCamOffset = aimOffset;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovNorm, Time.deltaTime * 10);
        }
        else
        {
            currCamOffset = camOffset;
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fovNorm, Time.deltaTime * 10);
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, currCamOffset, Time.deltaTime * 5);

        cameraCollision();
    }

    void cameraCollision()
    {
        Vector3 origin = target.position;
        Ray r = new Ray(origin, -(origin - transform.position).normalized);
        RaycastHit hit;
        if (Physics.Linecast(origin, transform.position, out hit, layerMask))
        {
            transform.position = origin + r.direction * hit.distance;
        }

        // set the far point with raycast
        if (Physics.Raycast(transform.position, transform.forward, out hit, 200, layerMask))
        {
            // if obstructed then set the point of the obstruction as the look position
            globalLookTarget.transform.position = hit.point;
        }
        else
        {
            // if nothing is obstructed thhen set the look position to a large distance
            globalLookTarget.localPosition = new Vector3(0, 0, 100);
        }
    }

    // Update is called once per frame
    void Update()
    {
        getMouseParam();
    }

    private void FixedUpdate()
    {
        cameraRotation();
        cameraTranslation();
    }
}
