using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(0, 0, -1);
        }
        KeepOnMap();
	}
    void KeepOnMap()
    {
        if (transform.position.z > 36.3f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -35f);
        }
        else if (transform.position.z < -36.3f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 35f);
        }
        else if (transform.position.x > 16)
        {
            transform.position = new Vector3(-15.5f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -16)
        {
            transform.position = new Vector3(15f, transform.position.y, transform.position.z);
        }
    }

}
