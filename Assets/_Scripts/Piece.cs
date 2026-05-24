using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum PieceType { Pawn, Bishop, King, Queen, Knight, Rook, Pope }
    enum MoveType { Directional, Positional }

    public Team CurrentTeam;
    public PieceType CurrentPieceType;    
    public BoardTile CurrentTile;
    MoveType CurrentMoveType;

    Vector2Int[] Directions;
    Vector2Int[] Offsets;

    int Steps = 1, Width = 1;

    private void Awake()
    {
        SetMoves();
    }

    private void Start()
    {
        if(CurrentTile != null)
        {
            CurrentTile.OccupyingPiece = this;
        }
    }

    void SetMoves()
    {
        //DIRECTIONAL
        if (CurrentPieceType == PieceType.Pawn)
        {
            if(CurrentTeam == Team.White)
            {
                Directions = new Vector2Int[]
                {
                    new Vector2Int(0, 1)
                };
            }
            else
            {
                Directions = new Vector2Int[]
                {
                    new Vector2Int(0, -1)
                };
            }
            

            Steps = 1;
            CurrentMoveType = MoveType.Directional;
        }
        else if(CurrentPieceType == PieceType.Bishop)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
            };

            Steps = 100;
            CurrentMoveType = MoveType.Directional;
        }
        else if(CurrentPieceType == PieceType.King)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
                new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
            };

            Steps = 1;
            CurrentMoveType = MoveType.Directional;
        }
        else if(CurrentPieceType == PieceType.Queen)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
                new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
            };

            Steps = 100;
            CurrentMoveType = MoveType.Directional;
        }
        else if(CurrentPieceType == PieceType.Rook)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(-1,0), new Vector2Int(0,-1)
            };

            Steps = 100;
            CurrentMoveType = MoveType.Directional;
        }
        else if(CurrentPieceType == PieceType.Pope)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
            };

            Steps = 100;
            Width = 3;
            CurrentMoveType = MoveType.Directional;
        }

        //POSITIONAL
        else if(CurrentPieceType == PieceType.Knight)
        {
            Offsets = new Vector2Int[]
            {
                new Vector2Int(1, 2), new Vector2Int(1, -2), new Vector2Int(-1, 2), new Vector2Int(-1, -2),
                new Vector2Int(2,1), new Vector2Int(2, -1), new Vector2Int(-2,1), new Vector2Int(-2,-1)
            };

            CurrentMoveType = MoveType.Positional;
        }
    }

    public Vector2Int[] GetValidPositions(BoardTile startTile)
    {
        BoardManager boardManager = BoardManager.Instance;
        List<Vector2Int> validPositions = new List<Vector2Int>();

        if(CurrentMoveType == MoveType.Directional)
        {
            foreach (Vector2Int dir in Directions)
            {
                for (int i = 1; i <= Steps; i++)
                {
                    Vector2Int pos = startTile.GridPos + (dir * i);
                    if (pos.x < 0 || pos.x >= boardManager.Tiles.GetLength(0) || pos.y < 0 || pos.y >= boardManager.Tiles.GetLength(1))
                    {
                        break;
                    }

                    if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece == null)
                    {
                        validPositions.Add(pos);
                    }
                    else
                    {
                        //check if enemy, then add and break
                        if(boardManager.Tiles[pos.x, pos.y].OccupyingPiece.CurrentTeam != CurrentTeam)
                        {
                            //add logic for capture
                            validPositions.Add(pos);
                        }

                        //breaks for this direction, so you cant pass occupied tiles
                        break;
                    }
                }
            }
        }
        else if(CurrentMoveType == MoveType.Positional)
        {
            foreach(Vector2Int offset in Offsets)
            {
                Vector2Int pos = startTile.GridPos + offset;
                if (pos.x < 0 || pos.x >= boardManager.Tiles.GetLength(0) || pos.y < 0 || pos.y >= boardManager.Tiles.GetLength(1))
                {
                    continue;
                }

                if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece == null || boardManager.Tiles[pos.x, pos.y].OccupyingPiece.CurrentTeam != CurrentTeam)
                {
                    validPositions.Add(pos);
                }
            }
        }
        
        
        return validPositions.ToArray();
    }

    public void CaptureOtherPiece(Piece pieceToCapture, BoardTile captureTile)
    {
        Vector2Int[] validPositions = GetValidPositions(CurrentTile);
        foreach(Vector2Int pos in validPositions)
        {
            if(pos == captureTile.GridPos)
            {
                Debug.Log("Piece captured!");
                Destroy(pieceToCapture.gameObject);
                return;
            }
        }
    }
}
