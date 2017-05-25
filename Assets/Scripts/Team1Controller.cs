using UnityEngine;
using System.Collections;

public class Team1Controller : MonoBehaviour {

    public Team1[] players;
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

    void SelectPlayerToGetFlag()
    {
        selectedPlayer = Random.Range(0, range);
        players[selectedPlayer].targetFlag = true;
        Debug.Log("Player assigned to get flag");
    }

    void SaveFrozenTeammate()
    {
        for(int i = 0; i < players.Length; i++)
        {
            if(players[i].frozen == true)
            {
                for(int j = 0; j < players.Length; j++)
                {
                    if(players[j].frozen == false && (players[j].targetFlag == false && players[j].hasFlag == false))
                    {
                        players[j].SaveTeamMate(players[i].gameObject);
                        return;
                    }
                }
            }
        }
    }
}
