using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    [SerializeField] GameObject Winscreen;

    Dictionary<Team, List<Piece>> KingsInEachTeam = new Dictionary<Team, List<Piece>>();

    private void Start()
    {
        Piece[] allPieces = FindObjectsOfType<Piece>();
        Piece[] kings = allPieces.Where(k => k.CurrentPieceType == PieceType.King).ToArray();
        
        foreach(Team team in System.Enum.GetValues(typeof(Team)))
        {
            KingsInEachTeam.Add(team, new List<Piece>(allPieces.Where(k => k.CurrentTeam == team && k.CurrentPieceType == PieceType.King).ToList()));
        }
    }

    private void Update()
    {
        //checking lost teams
        List<Team> teamsToRemove = new();
        foreach (var keyValue in KingsInEachTeam)
        {
            for(int i = keyValue.Value.Count - 1; i >= 0; i--)
            {
                if (keyValue.Value[i] == null)
                {
                    keyValue.Value.RemoveAt(i);
                    Debug.Log(keyValue.Value.Count);
                }
            }
            
            if (keyValue.Value.Count <= 0)
            {
                teamsToRemove.Add(keyValue.Key);
                //this team has lost
                //count active teams, when it reaches one, display that team
            }
        }

        //removal
        foreach(Team team in teamsToRemove)
        {
            KingsInEachTeam.Remove(team);
            Debug.Log(team + " removed");
        }

        if(KingsInEachTeam.Count <= 1)
        {
            //one team has won, display it.
            foreach(var keyValue in KingsInEachTeam)
            {
                Debug.Log($"{keyValue.Key} has won!");
            }
        }
    }
}
