using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraChange : MonoBehaviour
{

    public GameObject TPSCam;
    public GameObject FPSCam;
    public int CamMode; //0 --> ThirdCam and 1 --> FirstCam

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Camera"))
        {
            if (CamMode == 1) //Change camera
            {
                CamMode = 0;
            }
            else
            {
                CamMode += 1;
            }
            //call routine
            StartCoroutine(CamChange());
        }
    }

    IEnumerator CamChange()
    {
        yield return new WaitForSeconds(0.01f);
        if(CamMode == 0)
        {
            TPSCam.SetActive(true);
            FPSCam.SetActive(false);
        }
        if (CamMode == 1)
        {
            FPSCam.SetActive(true);
            TPSCam.SetActive(false);  
        }
    }
}
