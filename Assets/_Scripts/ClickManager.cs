using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClickType { Move, Capture, Select }

public class ClickManager : MonoBehaviour
{
    public LayerMask PieceMask, TileMask;

    PieceManager PieceManager;
    BoardManager BoardManager;

    private void Start()
    {
        PieceManager = PieceManager.Instance;
        BoardManager = BoardManager.Instance;
    }

    private void Update()
    {
        if (!WinManager.Instance.GameActive)
        {
            return;
        }

        if (PieceManager == null)
        {
            Debug.LogWarning("Piece Manager is null!");
        }
        if (BoardManager == null)
        {
            Debug.LogWarning("Board Manager is null!");
        }

        //ideal move, checks both piece and tile, registers both does piece first. capture, move, deselect
        if (Input.GetMouseButtonDown(0))
        {
            //clicking
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D pieceHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, PieceMask);
            RaycastHit2D tileHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, TileMask);

            Piece clickedPiece = null;
            if (pieceHit.transform != null)
            {
                pieceHit.transform.TryGetComponent(out clickedPiece);
            }

            BoardTile clickedTile = null;
            if (tileHit.transform != null)
            {
                tileHit.transform.TryGetComponent(out clickedTile);
            }

            //capture or select
            if (clickedPiece != null && clickedTile != null)
            {
                //select
                if (clickedPiece.CurrentTeam == TurnManager.Instance.ActiveTeam)
                {
                    clickedPiece.HandleClickInteraction(ClickType.Select, clickedPiece, clickedTile);
                }
                //capture
                else if (PieceManager.SelectedPiece != null)
                {
                    if (clickedPiece.CurrentTeam != PieceManager.SelectedPiece.CurrentTeam)
                    {
                        PieceManager.SelectedPiece.HandleClickInteraction(ClickType.Capture, clickedPiece, clickedTile);   
                    }
                }
            }
            //move
            else if (clickedPiece == null && clickedTile != null)
            {
                if (PieceManager.SelectedPiece != null)
                {
                    PieceManager.SelectedPiece.HandleClickInteraction(ClickType.Move, PieceManager.SelectedPiece, clickedTile);
                }
            }
        }
    }
}