using UnityEngine;
using System.Collections;

public class BasicMoveScript : MonoBehaviour {

    public Vector3 startPos, endPos;    //determine the bounds of motion
    public bool yHeight, xHeight, zHeight; //specify the axis to act as height.  Pieces arc within the height axis, but stay linear in the others
    public float speed; //number of frames it takes for piece to move from start to end
    private float deltaX, deltaY, deltaZ, maxX, maxY, maxZ;   //variables for move equation

	// Use this for initialization
	void Start () {
        //failsafe: if more than one height option is selected by tester, the booleans are prioritized: Y > X > Z
        if (yHeight && xHeight)
            xHeight = false;
        if (yHeight && zHeight)
            zHeight = false;
        if (xHeight && zHeight)
            zHeight = false;
        deltaX = endPos.x - startPos.x;
        deltaY = endPos.y - startPos.y;
        deltaZ = endPos.z - startPos.z;
        if (xHeight)
            maxX = (deltaX / 2) + 0.75f;
        else
            maxX = endPos.x;
        if (yHeight)
            maxY = (deltaY / 2) + 0.75f;
        else
            maxY = endPos.y;
        if (zHeight)
            maxZ = (deltaZ / 2) + 0.75f;
        else
            maxZ = endPos.z;
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 position = transform.position;
        while (position != endPos)
        {
            if (xHeight)
            {
                if (position.x != maxX)
                    transform.Translate(new Vector3(((2 * maxX) / speed), (deltaY / speed), (deltaZ / speed)) * Time.deltaTime);
                else
                    transform.Translate(new Vector3(((0 - 1) *((2 * maxX) / speed)), (deltaY / speed), (deltaZ / speed)) * Time.deltaTime);
            }
            else if (yHeight)
            {
                if (position.y != maxY)
                    transform.Translate(new Vector3((deltaX / speed), ((2 * maxY) / speed), (deltaZ / speed)) * Time.deltaTime);
                else
                    transform.Translate(new Vector3((deltaX / speed), ((0 - 1) * ((2 * maxY) / speed)), (deltaZ / speed)) * Time.deltaTime);
            }
            else if (zHeight)
            {
                if (position.x != maxX)
                    transform.Translate(new Vector3((deltaX / speed), (deltaY / speed), ((2 * maxZ) / speed)) * Time.deltaTime);
                else
                    transform.Translate(new Vector3((deltaX / speed), (deltaY / speed), ((0 - 1) * ((2 * maxZ) / speed))) * Time.deltaTime);
            }
        }
	}

    /*float equation (float height, float length) {   //height is the axis specified as height

        return 0;
    }*/
}
