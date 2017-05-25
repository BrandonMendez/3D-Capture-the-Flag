using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

    Team1[] teamMember1;
    Team2[] teamMember2;
	// Use this for initialization
	void Start () {
        teamMember1 = FindObjectsOfType<Team1>();
        teamMember2 = FindObjectsOfType<Team2>();
	}

    // Update is called once per frame
    void Update() {


    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject obj = collision.gameObject;

        if(gameObject.tag == "Flag1")
        {
            for(int i = 0; i < teamMember2.Length; i++)
            {
                if(obj == teamMember2[i].gameObject)
                {
                    gameObject.SetActive(false);
                 }
            }
        }
        if (gameObject.tag == "Flag2")
        {
            for (int i = 0; i < teamMember1.Length; i++)
            {
                if (obj == teamMember1[i].gameObject)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
