using UnityEngine;
using System.Collections;

public class BasicMoveScript : MonoBehaviour {

    private Vector3 vectorDistance, endPos, startPos;    //determine the bounds of motion
    private Rigidbody piece;    //for physics with piece
    private float distance, tanAngle, maxHeight;
    private float DELTA_TIME = (40 * Time.deltaTime);
    private bool apexPassed;

    // Use this for initialization
    void Start() {
        piece = GetComponent<Rigidbody>();
        endPos = GameObject.Find("EndBlock").transform.position;
        endPos.y = transform.position.y;
        startPos = transform.position;
        maxHeight = 3.0f;
        vectorDistance = endPos - startPos;
        vectorDistance.y = 0;
        distance = vectorDistance.magnitude;
        tanAngle = (maxHeight * 2) / Mathf.Abs(distance);
        apexPassed = false;
    }

    Vector3 ForceCalculation () {
        Vector3 initVelocity = new Vector3(XVelocity(), YVelocity(), ZVelocity());

        return new Vector3(XVelocity(), YVelocity(), ZVelocity());
    }

    float XVelocity () {
        float distance = endPos.x - startPos.x;
        return distance / DELTA_TIME;
    }

    float YVelocity () {
        return Physics.gravity.magnitude * (DELTA_TIME / 2);
    }

    float ZVelocity () {
        float distance = endPos.z - startPos.z;
        return distance / DELTA_TIME;
    }

    void FixedUpdate() {
        if (transform.position.y >= endPos.y)
            piece.velocity = VelocityCalculation();
        else {
            piece.velocity = new Vector3(0, 0, 0);
            if (transform.position != endPos)
                transform.position = endPos;
        }
    }
}
