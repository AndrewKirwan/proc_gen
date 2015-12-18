using UnityEngine;
using System.Collections;

public class lorenz : MonoBehaviour {

    public float dx, dy, dz;
    public float h = 0.01f;
    public float a = 10.0f;
    public float b = 28.0f;
    public float c = 8.0f / 3.0f;
    Vector3 move;
    

	// Use this for initialization
	void Start () {
        move.x = 0.01f;
    }
	
	// Update is called once per frame
	void Update () {

        dx = move.x + h * a * (move.y - move.x);
        dy = move.y + h * (move.x * (b - move.z) - move.y);
        dz = move.z + h * (move.x * move.y - c * move.z);

        move.x = dx;
        move.y = dy;
        move.z = dz;

        transform.position = move;
	}
}
