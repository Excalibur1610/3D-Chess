using UnityEngine;
using System.Collections;

public class BasicMoveScript : MonoBehaviour {

    public Vector3 startPos, endPos;    //determine the bounds of motion
    public float speed; //number of frames it takes for piece to move from start to end, values must be in bounds (0, 1]
    private float deltaX, deltaZ, maxX, maxY, maxZ, Xpivot, Zpivot;   //variables for move equation

    // Use this for initialization
    void Start() {
        deltaX = endPos.x - startPos.x;
        Debug.Log("deltaX solved.  Value = " + deltaX);
        float deltaY = endPos.y - startPos.y;
        Debug.Log("deltaY solved.  Value = " + deltaY);
        deltaZ = endPos.z - startPos.z;
        Debug.Log("deltaZ solved.  Value = " + deltaZ);
        maxX = endPos.x;
        Debug.Log("maxX solved.  Value = " + maxX);
        maxY = (deltaY / 2) + 0.75f + startPos.y;
        Debug.Log("maxY solved.  Value = " + maxY);
        maxZ = endPos.z;
        Debug.Log("maxZ solved.  Value = " + maxZ);
        Xpivot = (endPos.x + startPos.x) / 2;
        Debug.Log("Xpivot solved.  Value = " + Xpivot);
        Zpivot = (endPos.z + startPos.z) / 2;
        Debug.Log("Zpivot solved.  Value = " + Zpivot);
        if (speed <= 0)
            speed = 0.01f;
        else if (speed > 1)
            speed = 1.0f;
        Debug.Log("Speed solved.  Value = " + speed);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        Debug.Log("Current position: (" + position.x + ", " + position.y + ", " + position.z + ")");
        bool doMove = true;
        if (deltaX > 0 && position.x > endPos.x)
            doMove = false;
        else if (deltaX < 0 && position.x < endPos.x)
            doMove = false;
        else if (deltaX == 0 && position.x != endPos.x)
            doMove = false;
        if (deltaZ > 0 && position.z > endPos.z)
            doMove = false;
        else if (deltaZ < 0 && position.z < endPos.z)
            doMove = false;
        else if (deltaZ == 0 && position.z != endPos.z)
            doMove = false;
        if (position == endPos)
            doMove = false;
        Debug.Log("Is moving: " + doMove);
        
        bool doSwitch = false;
        if (deltaX > 0 && position.x >= Xpivot) {
            if (deltaZ > 0 && position.z >= Zpivot)
                doSwitch = true;
            else if (deltaZ < 0 && position.z <= Zpivot)
                doSwitch = true;
            else if (deltaZ == 0)
                doSwitch = true;
        } else if (deltaX < 0 && position.x <= Xpivot) {
            if (deltaZ > 0 && position.z >= Zpivot)
                doSwitch = true;
            else if (deltaZ < 0 && position.z <= Zpivot)
                doSwitch = true;
            else if (deltaZ == 0)
                doSwitch = true;
        } else if (deltaX == 0) {
            if (deltaZ > 0 && position.z >= Zpivot)
                doSwitch = true;
            else if (deltaZ < 0 && position.z <= Zpivot)
                doSwitch = true;
            else if (deltaZ == 0)
                doSwitch = true;
        }
        Debug.Log("Past pivot: " + doSwitch);

        if (doMove)
        {
            if (!doSwitch)
                transform.Translate(new Vector3(deltaX, (4 * maxY), deltaZ) * Time.deltaTime * speed);
            else
                transform.Translate(new Vector3(deltaX, ((0 - 1) * (4 * maxY)), deltaZ) * Time.deltaTime * speed);
        }
        else
            transform.position = endPos;
	}

    float equation (float maxHeight, float length) {

        return 0;
    }
}
