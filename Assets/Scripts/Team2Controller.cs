using UnityEngine;
using System.Collections;

public class Team2Controller : MonoBehaviour {

    public Team2[] players;
    public int range;

    private int selectedPlayer;

    // Use this for initialization
    void Start () {
        range = players.Length;
        SelectPlayerToGetFlag();
    }
	
	// Update is called once per frame
	void Update () {
        SaveFrozenTeammate();
    }

    //Select at random which member of the team will go get the flag
    void SelectPlayerToGetFlag()
    {
        selectedPlayer = Random.Range(0, range);
        players[selectedPlayer].targetFlag = true;
        Debug.Log("enemy assigned to get flag");
    }

    void SaveFrozenTeammate()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].frozen == true) //if a team member is frozen
            {
                for (int j = 0; j < players.Length; j++)
                {
                    if (players[j].frozen == false && (players[j].targetFlag == false && players[j].hasFlag == false)) //if the other members are not going for the flag and are not frozen
                    {
                        players[j].SaveTeamMate(players[i].gameObject ); //go save the frozen team member, function being called is in Team Script
                        return;
                    }
                }
            }
        }
    }
}
