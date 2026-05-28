using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public BoardTile[,] Tiles;

    [SerializeField] GameObject PiecePrefab;
    [SerializeField] Transform TileHolder;
    [SerializeField] Color EvenColor, OddColor;

    PieceCache[] PieceCache; //piece info to load upon reload

    private void Awake()
    {
        Instance = this;
    }

    public void SetBoard()
    {
        Tiles = new BoardTile[8, 8];
        BoardTile[] tiles = TileHolder.GetComponentsInChildren<BoardTile>();

        int width = Tiles.GetLength(0);
        int height = Tiles.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < height; x++)
            {
                BoardTile currentTile = tiles[y * width + x];
                currentTile.GridPos = new Vector2Int(x, y);
                Tiles[x, y] = currentTile;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                //color
                BoardTile tile = Tiles[x, y];
                if ((x + y) % 2 == 0)
                {
                    tile.Renderer.color = EvenColor;
                }
                else
                {
                    tile.Renderer.color = OddColor;
                }
            }
        }
    }
    public void CachePieces(Piece[] piecesToCache)
    {
        PieceCache = new PieceCache[piecesToCache.Length];
        for (int i = 0; i < piecesToCache.Length; i++)
        {
            PieceCache[i].Team = piecesToCache[i].CurrentTeam;
            PieceCache[i].PieceType = piecesToCache[i].CurrentPieceType;
            PieceCache[i].CurrentTile = piecesToCache[i].CurrentTile;
            PieceCache[i].Color = piecesToCache[i].GetComponent<SpriteRenderer>().color;

            if (PieceCache[i].CurrentTile == null)
            {
                Debug.Log("Null current tile found");
            }
        }
    }
    public void MovePieceToTile(BoardTile newTile, Piece piece)
    {
        if(piece.CurrentTeam != TurnManager.Instance.ActiveTeam)
        {
            return;
        }

        //get valid tiles, check if place to move is inside list, move if true, end turn
        Vector2Int[] validPositions = piece.GetValidTargets(piece.CurrentTile);
        foreach(Vector2Int pos in validPositions)
        {
            if(pos == newTile.GridPos)
            {
                piece.CurrentTile.OccupyingPiece = null;
                newTile.OccupyingPiece = piece;
                piece.CurrentTile = newTile;
                piece.transform.position = newTile.transform.position;
                return;
            }
        }
    }
    public void CaptureOtherPiece(Piece attackPiece, Piece pieceToCapture, BoardTile captureTile)
    {
        Vector2Int[] validPositions = attackPiece.GetValidTargets(attackPiece.CurrentTile);
        foreach (Vector2Int pos in validPositions)
        {
            if (pos == captureTile.GridPos)
            {
                Destroy(pieceToCapture.gameObject);
                return;
            }
        }
    }
    public void ResetPieces()
    {
        //Destroys all pieces
        foreach(Piece piece in FindObjectsOfType<Piece>())
        {
            DestroyImmediate(piece.gameObject);
        }

        TurnManager.Instance.ResetTurn();

        //Makes new copies
        for(int i = 0; i < PieceCache.Length; i++)
        {
            GameObject pieceObj = Instantiate(PiecePrefab, PieceCache[i].CurrentTile.transform.position, Quaternion.identity);
            Piece piece = pieceObj.GetComponent<Piece>();
            piece.CurrentPieceType = PieceCache[i].PieceType;
            piece.CurrentTile = PieceCache[i].CurrentTile;
            piece.CurrentTeam = PieceCache[i].Team;
            piece.Renderer.color = PieceCache[i].Color;

            piece.MoveAndSpriteConfig();

            piece.CurrentTile.OccupyingPiece = piece;
        }

        PieceManager.Instance.DeselectPiece();
    }
}
