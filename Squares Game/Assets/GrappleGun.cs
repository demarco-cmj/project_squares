using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleGun : MonoBehaviour
{

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;
    public LayerMask isGrapplable;
    public Transform grappleSpawn, playerCamera, player;

    private float maxDistance = 100f;
    private SpringJoint joint;

    void Awake() {
        lr = GetComponent<LineRenderer>();
    }

    void Update() {
        if (Input.GetKey(KeyCode.Q)) {
            Debug.Log("player pressed Q");
            startGrapple();
        } else {
            stopGrapple();
        }
    }

    void LateUpdate() {
        DrawRope();
    }

    void startGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(origin: playerCamera.position, direction: playerCamera.forward, out hit, maxDistance)) {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = grappleSpawn.position;
        }
    }

    void stopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }

    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, grappleSpawn.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
