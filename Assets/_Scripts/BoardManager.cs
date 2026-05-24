using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public BoardTile[,] Tiles;

    [SerializeField] Transform TileHolder;
    [SerializeField] Color EvenColor, OddColor;
    

    private void Awake()
    {
        Instance = this;
        SetBoard();
    }

    void SetBoard()
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
                Tiles[x,y] = currentTile;
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
    public void MovePieceToTile(BoardTile newTile, Piece piece)
    {
        if(piece.CurrentTeam != TurnManager.Instance.ActiveTeam)
        {
            return;
        }

        //get valid tiles, check if place to move is inside list, move if true, end turn
        Vector2Int[] validPositions = piece.GetValidPositions(piece.CurrentTile);
        foreach(Vector2Int pos in validPositions)
        {
            if(pos == newTile.GridPos)
            {
                piece.CurrentTile.OccupyingPiece = null;
                newTile.OccupyingPiece = piece;
                piece.CurrentTile = newTile;
                piece.transform.position = newTile.transform.position;
                TurnManager.Instance.ChangeTeam();
                return;
            }
        }
    }
}
