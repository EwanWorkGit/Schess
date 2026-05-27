using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    //to prevent multiple different pieces' clicks from stacking
                    PieceManager.DeselectPiece();
                    PieceManager.SelectPiece(clickedPiece);
                }
                //capture
                else if (PieceManager.SelectedPiece != null)
                {
                    if (clickedPiece.CurrentTeam != PieceManager.SelectedPiece.CurrentTeam)
                    {
                        BoardManager.CaptureOtherPiece(PieceManager.SelectedPiece, clickedPiece, clickedTile);
                        BoardManager.MovePieceToTile(clickedTile, PieceManager.SelectedPiece);
                        PieceManager.DeselectPiece();
                    }
                }
            }
            //move
            else if (clickedPiece == null && clickedTile != null)
            {
                if (PieceManager.SelectedPiece != null)
                {
                    //move selected piece to clicked tile
                    BoardManager.MovePieceToTile(clickedTile, PieceManager.SelectedPiece);
                    PieceManager.DeselectPiece();
                }
            }
        }
    }
}