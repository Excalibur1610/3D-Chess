using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {
    private int count = 200;
    private Vector3 axis;

    // Use this for initialization
    void Start () {
        axis = new Vector3(0, -.2f,0);
	}
	
	// Update is called once per frame
	void Update () {
        if (count % 400 == 0)
            axis.y *= -1;
        transform.Rotate(axis);
        count++;
	}
}
