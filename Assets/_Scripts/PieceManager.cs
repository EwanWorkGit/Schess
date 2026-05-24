using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public static PieceManager Instance;
    public Piece SelectedPiece;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectPiece(Piece piece)
    {
        SelectedPiece = piece;
        Debug.Log($"Selected {SelectedPiece.transform.name}");
        DisplayMoves();
    }
    
    public void DeselectPiece()
    {
        BoardTile[,] Tiles = BoardManager.Instance.Tiles;
        foreach(BoardTile tile in Tiles)
        {
            tile.SeeThroughTile.SetActive(false);
        }

        SelectedPiece = null;
    }

    public void DisplayMoves()
    {
        if (SelectedPiece != null)
        {
            Vector2Int[] validPositions = SelectedPiece.GetValidPositions(SelectedPiece.CurrentTile);
            BoardTile[,] tiles = BoardManager.Instance.Tiles;

            foreach (Vector2Int pos in validPositions)
            {
                BoardTile targetTile = tiles[pos.x, pos.y];
                if (targetTile.OccupyingPiece == null || targetTile.OccupyingPiece.CurrentTeam != TurnManager.Instance.ActiveTeam)
                {
                    targetTile.SeeThroughTile.SetActive(true);
                }
            }
        }
    }
}
