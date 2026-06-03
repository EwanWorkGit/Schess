using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class WinManager : MonoBehaviour
{
    public static WinManager Instance;
    public bool GameActive = true;

    [SerializeField] GameObject Winscreen;
    [SerializeField] TMP_Text TeamText;

    Dictionary<Team, List<Piece>> KingsInEachTeam = new Dictionary<Team, List<Piece>>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Winscreen.SetActive(false);
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
            
            //team which has lost
            if (keyValue.Value.Count <= 0)
            {
                teamsToRemove.Add(keyValue.Key);
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
                TeamText.text = $"{keyValue.Key} team has won!";
                Winscreen.SetActive(true);
                GameActive = false;
            }
        }
    }

    public void SetKings()
    {
        KingsInEachTeam.Clear();

        Piece[] allPieces = FindObjectsOfType<Piece>();
        Piece[] kings = allPieces.Where(k => k.CurrentPieceType == PieceType.King).ToArray();

        foreach (Team team in System.Enum.GetValues(typeof(Team)))
        {
            KingsInEachTeam.Add(team, new List<Piece>(allPieces.Where(k => k.CurrentTeam == team && k.CurrentPieceType == PieceType.King).ToList()));
        }
    }

    public void Surrender()
    {
        Team activeTeam = TurnManager.Instance.ActiveTeam;
        foreach(var kvp in KingsInEachTeam.ToArray())
        {
            if(kvp.Key == activeTeam)
            {
                foreach(Piece piece in kvp.Value)
                {
                    Destroy(piece.gameObject);
                    KingsInEachTeam.Remove(activeTeam);
                }
            }
        }
    }
    public void Rematch()
    {
        BoardManager.Instance.ResetPieces();
        SetKings();
        Winscreen.SetActive(false);
        GameActive = true;
    }
}
