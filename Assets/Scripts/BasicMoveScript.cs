using UnityEngine;
using System.Collections;

public class BasicMoveScript : MonoBehaviour {

    private Vector3 vectorDistance, endPos;    //determine the bounds of motion
    private Rigidbody piece;    //for physics with piece
    private float distance, tanAngle, maxHeight;
    private bool apexPassed;

    // Use this for initialization
    void Start() {
        piece = GetComponent<Rigidbody>();
        endPos = GameObject.Find("EndBlock").transform.position;
        endPos.y = transform.position.y;
        Vector3 startPos = transform.position;
        maxHeight = 3.0f;
        vectorDistance = endPos - startPos;
        vectorDistance.y = 0;
        distance = vectorDistance.magnitude;
        tanAngle = (maxHeight * 2) / Mathf.Abs(distance);
        apexPassed = false;
    }

    Vector3 VelocityCalculation () {
        return new Vector3(XVelocity(), YVelocity(), ZVelocity());
    }

    float XVelocity () {
        return endPos.x - transform.position.x + 0.4f;  //0.4f constant modifier to eliminate skipping effect
    }

    float YVelocity () {
        if (transform.position.y < maxHeight && !apexPassed)
        {
            float y = tanAngle * (Mathf.Abs(distance) / 2);
            y -= transform.position.y - 0.1f;
            y *= Physics.gravity.magnitude / 2;
            return y;
        }
        if (apexPassed)
        {
            float y = tanAngle * (Mathf.Abs(distance) / 2);
            y -= transform.position.y - 0.1f;
            return (0.0f - y);
        }
        apexPassed = true;
        return 0.0f - 0.1f;
    }

    float ZVelocity () {
        return endPos.z - transform.position.z + 0.4f;  //0.4f constant modifier to eliminate skipping effect
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
