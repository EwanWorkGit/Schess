using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    Dictionary<Team, List<Piece>> KingsInTeam = new Dictionary<Team, List<Piece>>();

    private void Start()
    {
        Piece[] allPieces = FindObjectsOfType<Piece>();
        Piece[] kings = allPieces.Where(k => k.CurrentPieceType == PieceType.King).ToArray();
        
        foreach(Team team in System.Enum.GetValues(typeof(Team)))
        {
            KingsInTeam.Add(team, new List<Piece>(allPieces.Where(k => k.CurrentTeam == team && k.CurrentPieceType == PieceType.King).ToList()));
        }

        foreach(var keyValue in KingsInTeam)
        {
            foreach(Piece piece in keyValue.Value)
            {
                Debug.Log($"{keyValue.Key} : {piece.transform.name}");
            }
        }
    }
}
