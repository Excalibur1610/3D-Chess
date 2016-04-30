using UnityEngine;

public class BasicMoveScript : MonoBehaviour {

    private Vector3 endPos, startPos;    //determine the bounds of motion
    private Rigidbody piece;    //for physics with piece
    private float distance, tanAngle, maxHeight;
    private float DELTA_TIME = 40;

    // Use this for initialization
    void Start() {
        DELTA_TIME *= Time.deltaTime;
        piece = GetComponent<Rigidbody>();
        endPos = GameObject.Find("EndBlock").transform.position;
        endPos.y = transform.position.y;
        startPos = transform.position;
        piece.AddForce(ForceCalculation(), ForceMode.Acceleration);
    }

    Vector3 ForceCalculation () {
        Vector3 initVelocity = new Vector3(XVelocity(), YVelocity(), ZVelocity());
        Vector3 force = piece.mass * initVelocity / Time.fixedDeltaTime;
        return force;
    }

    float XVelocity () {
        float distance = endPos.x - startPos.x;
        return distance / (DELTA_TIME * 2);
    }

    float YVelocity () {
        return Physics.gravity.magnitude * (DELTA_TIME);
    }

    float ZVelocity () {
        float distance = endPos.z - startPos.z;
        return distance / (DELTA_TIME * 2);
    }

    void Update () {
        Vector3 position = transform.position;
        position.y += .2f;
        if (position == endPos)
            piece.velocity = new Vector3(0, 0, 0);
    }
}
