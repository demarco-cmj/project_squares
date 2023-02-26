using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilCam : MonoBehaviour
{

    public float rotationSpeed = 6;
    public float returnSpeed = 25;
    [SerializeField] Camera cam;

    public Vector3 recoilRotation = new Vector3(10f, 5f, 8f); //aim from hip (straight back, horizontal, vertical)
    // public Vector3 recoilRotationAiming = new Vector3(2f, 2f, 2f); //for ads

    // public bool isAiming;

    private Vector3 currentRotation;
    private Vector3 rot;

    void FixedUpdate()
    {   
        if (cam) {
            currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
            rot = Vector3.Slerp(rot, currentRotation, rotationSpeed * Time.deltaTime);
            cam.transform.localRotation = Quaternion.Euler(rot);       
        }
                             //TODO creates null err
    }

    public void CamRecoil() {
        currentRotation += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
    }
}
