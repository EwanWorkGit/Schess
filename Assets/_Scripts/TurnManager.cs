using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team { White, Black }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public Team ActiveTeam;

    int TeamIndex = 0;

    private void Awake()
    {
        Instance = this;
        ActiveTeam = (Team)TeamIndex;
    }

    //should switch upon move
    public void ChangeTeam()
    {
        TeamIndex++;
        if(TeamIndex > System.Enum.GetValues(typeof(Team)).Length - 1)
        {
            TeamIndex = 0;
        }

        ActiveTeam = (Team)TeamIndex;
    }
}
