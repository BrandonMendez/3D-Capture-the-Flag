using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
	void Start ()
	{
		// Optional...
	}
	
	float counter = 10.0f;
	
	void Update ()
	{
		// Obtain input information (See "Horizontal" and "Vertical" in the Input Manager)
		float horizontal = Input.GetAxis ("Horizontal");
		float vertical = Input.GetAxis ("Vertical");
		float speed = 10;
		float step;
		
		
		// Check for inputs
		if(!Mathf.Approximately(vertical, 0.0f) || !Mathf.Approximately(horizontal, 0.0f))
		{
			counter = 0;
			transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
			
			Vector3 direction = new Vector3(horizontal, 0.0f, vertical);
			direction = Vector3.ClampMagnitude(direction, 1.0f);

           
            transform.Translate(direction * 0.1f , Space.World);

            
            Vector3 relativePos = (transform.position);
			Quaternion rotation = Quaternion.LookRotation(relativePos);
			transform.rotation = rotation;

            
            step = speed * Time.deltaTime;
			transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, step);
		}

        
        if (counter >= 100.0f) 
        transform.localScale += new Vector3 (0.1f, 0.1f, 0.1f);
		else
			counter += 1.0f;

        // TODO: Translate the game object in world space

        // TODO: Rotate the game object

        // TODO: Reset idle timer to zero 
    }



    // ALTERNATIVE
    /*
        if(Input.GetKey (KeyCode.W))
        {

        }
        if(Input.GetKey (KeyCode.S))
        {

        }
        if(Input.GetKey (KeyCode.A))
        {

        }
        if(Input.GetKey (KeyCode.D))
        {

        }
        */
}

