using UnityEngine;
using System.Collections;

public class BasicMoveScript : MonoBehaviour {

    public Vector3 startPos, endPos;    //determine the bounds of motion
    //public bool yHeight = true, xHeight = false, zHeight = false; //specify the axis to act as height.  Pieces arc within the height axis, but stay linear in the others
    //public float speed; //number of frames it takes for piece to move from start to end
    private float deltaX, deltaY, deltaZ, maxX, maxY, maxZ;   //variables for move equation

    // Use this for initialization
    void Start() {
        //failsafe: if more than one height option is selected by tester, the booleans are prioritized: Y > X > Z
        //failsafe: if no height option is selected, Y is made default
        /*if (yHeight && xHeight) {
            xHeight = false;
            Debug.Log("xHeight/yHeight conflict.  Resolved to yHeight.");
        } if (yHeight && zHeight) {
            zHeight = false;
            Debug.Log("zHeight/yHeight conflict.  Resolved to yHeight.");
        } if (xHeight && zHeight) {
            zHeight = false;
            Debug.Log("xHeight/zHeight conflict.  Resolved to xHeight.");
        } if (!xHeight && !yHeight && !zHeight) { 
            yHeight = true;
            Debug.Log("Height opt not set.  Resolved to yHeight.");
        }*/
        deltaX = endPos.x - startPos.x;
        Debug.Log("deltaX solved.  Value = " + deltaX);
        deltaY = endPos.y - startPos.y;
        Debug.Log("deltaY solved.  Value = " + deltaY);
        deltaZ = endPos.z - startPos.z;
        Debug.Log("deltaZ solved.  Value = " + deltaZ);
        /*if (xHeight)
            maxX = (deltaX / 2) + 0.75f;
        else*/
            maxX = endPos.x;
        Debug.Log("maxX solved.  Value = " + maxX);
        //if (yHeight)
            maxY = (deltaY / 2) + 0.75f;
        /*else
            maxY = endPos.y;*/
        Debug.Log("maxY solved.  Value = " + maxY);
        /*if (zHeight)
            maxZ = (deltaZ / 2) + 0.75f;
        else*/
            maxZ = endPos.z;
        Debug.Log("maxZ solved.  Value = " + maxZ);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        Debug.Log("Current position: (" + position.x + ", " + position.y + ", " + position.z + ")");
        bool doMove = false;
        if (doMove)
        {
            if (position.y < maxY)
                transform.Translate(new Vector3(deltaX, (4 * maxY), deltaZ) * Time.deltaTime);
            else
                transform.Translate(new Vector3(deltaX, ((0 - 1) * (4 * maxY)), deltaZ) * Time.deltaTime);
        }
	}

    /*float equation (float height, float length) {   //height is the axis specified as height

        return 0;
    }*/
}
