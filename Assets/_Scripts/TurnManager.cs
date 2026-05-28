using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Team { White, Black }

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public Action OnTurnChange;
    public Team ActiveTeam;

    [SerializeField] bool AutoChangeTurns;

    int TeamIndex = 0;

    private void Awake()
    {
        Instance = this;
        ActiveTeam = (Team)TeamIndex;
    }

    private void Update()
    {
        //FOR DEBUGGING
        if(Input.GetKeyDown(KeyCode.R))
        {
            ChangeTeam(true);
        }
    }

    //should switch upon move
    public void ChangeTeam(bool overrideRestrictions)
    {
        if(!AutoChangeTurns && !overrideRestrictions)
        {
            return;
        }

        TeamIndex++;
        if(TeamIndex > System.Enum.GetValues(typeof(Team)).Length - 1)
        {
            TeamIndex = 0;
        }

        ActiveTeam = (Team)TeamIndex;
        OnTurnChange?.Invoke();
    }

    public void ResetTurn()
    {
        TeamIndex = 0;
        ActiveTeam = (Team)TeamIndex;
    }
}
