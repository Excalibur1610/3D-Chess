using UnityEngine;

public class BasicMoveScript : MonoBehaviour {

    private Vector3 endPos, startPos;    //determine the bounds of motion
    private Rigidbody piece;    //for physics with piece
    float DELTA_TIME = 80;

    // Use this for initialization
    void Start() {
        DELTA_TIME *= Time.deltaTime;
        piece = GetComponent<Rigidbody>();
        endPos = new Vector3(-1.5f, 0f, -1.5f);
        endPos.y = transform.position.y;
        startPos = transform.position;
        piece.isKinematic = false;
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
        if (Mathf.Abs(position.x - endPos.x) < .1f && Mathf.Abs(position.z - endPos.z) < .1f)
        {
            piece.isKinematic = true;
            piece.velocity = new Vector3(0, 0, 0);
            transform.position = endPos;
        }
    }
}
