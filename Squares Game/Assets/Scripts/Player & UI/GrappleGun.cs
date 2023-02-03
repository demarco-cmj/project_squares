using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GrappleGun : MonoBehaviour
{
    public Rigidbody rb;
    public PhotonView PV;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private Vector3 currentGrapplePosition;
    public LayerMask isGrapplable;
    public Transform grappleSpawn, playerCam;
    private float maxDistance = 25f;
    private SpringJoint joint;
    private bool isGrappling = false;
    private Vector3 gx, gy;

    private Dictionary < int, Vector3 > playersGrappling = new Dictionary<int, Vector3>();

    void Awake() {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();
        lr = GetComponent<LineRenderer>();
    }

    public void Grapple() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            isGrappling = startGrapple();
            if (isGrappling) {
                PV.RPC("toggleEnemyGrappleON", RpcTarget.Others, PV.ViewID, grapplePoint);
                
            }
        } 
        else if (Input.GetKeyUp(KeyCode.Q)) {
            stopGrapple();
            if (isGrappling) {
                isGrappling = false;
                PV.RPC("toggleEnemyGrappleOFF", RpcTarget.Others, PV.ViewID);
            }
        }
    }

    //local player calculation
    bool startGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(origin: rb.position, direction: playerCam.forward, out hit, maxDistance)) {

            grapplePoint = hit.point;
            joint = rb.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(rb.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.4f;
            joint.minDistance = distanceFromPoint * 0.3f;

            joint.spring = 50f;
            joint.damper = 10f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = grappleSpawn.position;

            return true;
        }
        return false; //if the same pos, not grappling
    }

    void stopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }

    public void DrawRopeLocal() {
        // If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f); //adds "animation"
        
        lr.SetPosition(0, grappleSpawn.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    // [PunRPC]
    // void Test(int id, Vector3 grapplePoint) {
    //     Debug.Log(PhotonView.Find(id).Owner.NickName + " is grappling \n " + "ID: " + id);
        
    // }


    [PunRPC]
    void toggleEnemyGrappleON(int id, Vector3 grapplePoint) {
        // Debug.Log(PhotonView.Find(id).Owner.NickName + "ON");
        playersGrappling.Add(id, grapplePoint);  
    }

    [PunRPC]
    void toggleEnemyGrappleOFF(int id) {
        // Debug.Log(PhotonView.Find(id).Owner.NickName + "OFF");
        //Debug.Log(PhotonView.Find(id).Owner.NickName + " is grappling \n " + "ID: " + id);
       
        PhotonView.Find(id).gameObject.GetComponent<LineRenderer>().positionCount = 0;
        playersGrappling.Remove(id);
    }

    public void DrawAllRopes() {
        foreach (KeyValuePair<int, Vector3> entry in playersGrappling) {
            // Debug.Log(PhotonView.Find(entry.Key).Owner.NickName + " is grappling \n " + "ID: " + entry.Key + "Pos: " + entry.Value);
            PhotonView.Find(entry.Key).gameObject.GetComponent<LineRenderer>().positionCount = 2;
            PhotonView.Find(entry.Key).gameObject.GetComponent<LineRenderer>().SetPositions(new Vector3[]{PhotonView.Find(entry.Key).gameObject.transform.position, entry.Value});
        }
    }
}
