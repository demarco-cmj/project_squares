using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    [SerializeField] private float smooth;
    [SerializeField] private float swayMuliplier;
    

    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.isPaused) {
            float mouseX = Input.GetAxisRaw("Mouse X") * swayMuliplier;
            float mouseY = Input.GetAxisRaw("Mouse Y") * swayMuliplier;

            Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        }

    }
}
