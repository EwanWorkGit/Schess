using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowManager : MonoBehaviour
{
    [SerializeField] BoardManager BoardManager;
    [SerializeField] WinManager WinManager;

    private void Start()
    {
        BoardManager.SetBoard();
        Piece[] allPieces = FindObjectsOfType<Piece>();
        foreach(Piece piece in allPieces)
        {
            piece.AssignClosestTileToCurrent();
            piece.transform.position = piece.CurrentTile.transform.position;
            piece.CurrentTile.OccupyingPiece = piece;
            piece.StartingTile = piece.CurrentTile;
        }
        BoardManager.CachePieces(allPieces);
        WinManager.SetKings();
    }
}
