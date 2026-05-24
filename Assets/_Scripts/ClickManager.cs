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
        if(Input.GetMouseButtonDown(0))
        {
            //clicking piece
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D pieceHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, PieceMask);

            if(pieceHit.transform != null)
            {
                if (pieceHit.transform.TryGetComponent(out Piece piece))
                {
                    //to prevent multiple different pieces' clicks from stacking
                    if (piece.CurrentTeam == TurnManager.Instance.ActiveTeam)
                    {
                        //selecting new piece
                        PieceManager.DeselectPiece();
                        PieceManager.SelectPiece(piece);
                    }
                    else
                    {
                        //pressing enemy piece
                        MoveAndCapturePiece(mousePos, piece);
                    }
                }
            }
            else
            {
                MovePiece(mousePos);   
            }
        }
    }
    void MovePiece(Vector2 mousePos)
    {
        //clicking tile
        RaycastHit2D tileHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, TileMask);
        if (tileHit.transform != null)
        {
            if (tileHit.transform.TryGetComponent(out BoardTile tile))
            {
                if (PieceManager.SelectedPiece != null)
                {
                    BoardManager.MovePieceToTile(tile, PieceManager.SelectedPiece);
                    PieceManager.DeselectPiece();
                }
            }
        }
    }

    void MoveAndCapturePiece(Vector2 mousePos, Piece pieceToCapture)
    {
        //clicking tile
        RaycastHit2D tileHit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, TileMask);
        if (tileHit.transform != null)
        {
            if (tileHit.transform.TryGetComponent(out BoardTile tile))
            {
                if (PieceManager.SelectedPiece != null)
                {
                    PieceManager.SelectedPiece.CaptureOtherPiece(pieceToCapture, tile);
                    BoardManager.MovePieceToTile(tile, PieceManager.SelectedPiece);
                    PieceManager.DeselectPiece();
                }
            }
        }
    }
}
