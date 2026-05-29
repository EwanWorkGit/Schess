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

    private void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            if(SelectedPiece != null)
            {
                UnDisplayMoves();
                SelectedPiece.IsInActionMode = !SelectedPiece.IsInActionMode;
                DisplayMoves();
            }
        }
    }

    public void SelectPiece(Piece piece)
    {
        SelectedPiece = piece;
        DisplayMoves();
    }
    public void DeselectPiece()
    {
        UnDisplayMoves();
        SelectedPiece = null;
    }
    public void UnDisplayMoves()
    {
        BoardTile[,] Tiles = BoardManager.Instance.Tiles;
        foreach (BoardTile tile in Tiles)
        {
            tile.SeeThroughTile.SetActive(false);
        }
    }
    public void DisplayMoves()
    {
        if (SelectedPiece != null)
        {
            Vector2Int[] validPositions = SelectedPiece.GetValidTargets(SelectedPiece.CurrentTile);
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
