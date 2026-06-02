using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


public class MovesetLibrary : MonoBehaviour
{
    public static MovesetLibrary Instance;

    private void Awake()
    {
        Instance = this;
    }

    //MOVE OUT OF BOUNDS CHECKS TO PIECE OR BOARD

    public void DefinePiece(Piece piece)
    {
        //DIRECTIONAL
        if (piece.CurrentPieceType == PieceType.Pawn)
        {
            if (piece.CurrentTeam == Team.White)
            {
                piece.Directions = new Vector2Int[] { new Vector2Int(0, 1) };
            }
            else
            {
                piece.Directions = new Vector2Int[] { new Vector2Int(0, -1) };
            }

            piece.Steps = 1;
            piece.CurrentMoveType = MoveType.Directional;
        }
        else if (piece.CurrentPieceType == PieceType.Bishop)
        {
            piece.Directions = new Vector2Int[]
            {
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
            };

            piece.Steps = 100;
            piece.CurrentMoveType = MoveType.Directional;
        }
        else if (piece.CurrentPieceType == PieceType.King)
        {
            piece.Directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
                new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
            };

            piece.Steps = 1;
            piece.CurrentMoveType = MoveType.Directional;
        }
        else if (piece.CurrentPieceType == PieceType.Queen)
        {
            piece.Directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
                new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
            };

            piece.Steps = 100;
            piece.CurrentMoveType = MoveType.Directional;
        }
        else if (piece.CurrentPieceType == PieceType.Rook)
        {
            piece.Directions = new Vector2Int[]
            {
                new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(-1,0), new Vector2Int(0,-1)
            };

            piece.Steps = 100;
            piece.CurrentMoveType = MoveType.Directional;
        }
        else if (piece.CurrentPieceType == PieceType.Assassin)
        {
            piece.Directions = new Vector2Int[]
            {
                //diagonals
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1),
                //straights
                new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1)
            };

            piece.Steps = 3;
            piece.CurrentMoveType = MoveType.Directional;
        }

        //POSITIONAL
        else if (piece.CurrentPieceType == PieceType.Knight)
        {
            piece.Offsets = new Vector2Int[]
            {
                new Vector2Int(1, 2), new Vector2Int(1, -2), new Vector2Int(-1, 2), new Vector2Int(-1, -2),
                new Vector2Int(2,1), new Vector2Int(2, -1), new Vector2Int(-2,1), new Vector2Int(-2,-1)
            };

            piece.CurrentMoveType = MoveType.Positional;
        }

        //MIX OF BOTH
        else if (piece.CurrentPieceType == PieceType.Artillery)
        {
            piece.Offsets = new Vector2Int[]
            {
                new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1)
            };

            piece.Directions = new Vector2Int[]
            {
                new Vector2Int(0,1) * (piece.CurrentTeam == Team.White ? 1 : -1)
            };

            piece.Steps = 100;
        }
    }

    //moves dont change piece variables, only utilize them to display and give moves
    public void BaseMoves(Piece piece, BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
    {
        if (piece.CurrentMoveType == MoveType.Directional)
        {
            foreach (Vector2Int dir in piece.Directions)
            {
                for (int i = 1; i <= piece.Steps; i++)
                {
                    Vector2Int pos = startTile.GridPos + (dir * i);

                    //out of bounds check
                    if (!piece.IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos))
                    {
                        Debug.Log("breaked for this direction.");
                        break;
                    }

                    if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece == null)
                    {
                        validPositions.Add(pos);
                    }
                    else
                    {
                        //check if enemy, then add and break
                        if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece.CurrentTeam != piece.CurrentTeam)
                        {
                            validPositions.Add(pos);
                        }

                        //breaks for this direction, so you cant pass occupied tiles
                        break;
                    }
                }
            }
        }
        else if (piece.CurrentMoveType == MoveType.Positional)
        {
            foreach (Vector2Int offset in piece.Offsets)
            {
                Vector2Int pos = startTile.GridPos + offset;
                if (pos.x < 0 || pos.x >= boardManager.Tiles.GetLength(0) || pos.y < 0 || pos.y >= boardManager.Tiles.GetLength(1))
                {
                    continue;
                }

                if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece == null || boardManager.Tiles[pos.x, pos.y].OccupyingPiece.CurrentTeam != piece.CurrentTeam)
                {
                    validPositions.Add(pos);
                }
            }
        }
    }
    public void PawnMoves(Piece piece, BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
    {
        foreach (Vector2Int dir in piece.Directions)
        {
            for(int i = 1; i <= piece.Steps; i++)
            {
                Vector2Int pos = startTile.GridPos + (dir * i);
                if (!piece.IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos))
                {
                    break;
                }

                //movable pieces
                if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece == null)
                {
                    validPositions.Add(pos);
                }
                else
                {
                    break;
                }
            }   
        }

        //captures
        //only y should be inverted but lets do that after double move
        Vector2Int[] captureOffsets = new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(-1, 1) };
        foreach (Vector2Int offset in captureOffsets)
        {
            Vector2Int pos = startTile.GridPos + (piece.CurrentTeam == Team.White ? offset : new Vector2Int(offset.x, -offset.y));
            if (piece.IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos) && boardManager.Tiles[pos.x, pos.y].OccupyingPiece != null)
            {
                validPositions.Add(pos);
            }
        }
    }
    public void AssassinMoves(Piece piece, BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
    {
        foreach (Vector2Int dir in piece.Directions)
        {
            bool passedPiece = false;
            bool isStraightDir = dir.x != 0 && dir.y != 0;
            int currentSteps = (isStraightDir) ? piece.Steps : 1;
            for (int i = 1; i <= currentSteps; i++)
            {
                Vector2Int pos = startTile.GridPos + (dir * i);

                //out of bounds check
                if (!piece.IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos))
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
                    if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece.CurrentTeam != piece.CurrentTeam)
                    {
                        //piece capture position
                        if (passedPiece)
                        {
                            validPositions.Add(pos);
                            passedPiece = false;
                        }
                        else
                        {
                            passedPiece = true;
                        }
                    }
                }
            }
        }

    }
    public void ArtilleryMoves(Piece piece, BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
    {
        if (!piece.IsInActionMode)
        {
            foreach (Vector2Int offset in piece.Offsets)
            {
                Vector2Int pos = startTile.GridPos + offset;
                //out of bounds
                if (!piece.IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos) || boardManager.Tiles[pos.x, pos.y].OccupyingPiece != null)
                {
                    continue;
                }

                //movable pieces
                if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece == null)
                {
                    validPositions.Add(pos);
                }
            }
        }
        else
        {
            foreach (Vector2Int dir in piece.Directions)
            {
                for (int i = 1; i <= piece.Steps; i++)
                {
                    Vector2Int pos = startTile.GridPos + (dir * i);
                    if (!piece.IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos))
                    {
                        continue;
                    }
                    validPositions.Add(pos);
                    float dist = Vector2Int.Distance(startTile.GridPos, pos);
                    float distPerWidth = 3f; //how large the distance has to be for width to increase by 1
                    int width = (int)Mathf.Floor(dist / distPerWidth);

                    Vector2Int[] sideOffsets = new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(-1, 0) };
                    foreach (Vector2Int sideOffset in sideOffsets)
                    {
                        for (int j = 1; j <= width; j++)
                        {
                            int x = pos.x + (sideOffset.x * j);
                            Vector2Int sidePos = new Vector2Int(x, pos.y);
                            if (!piece.IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), sidePos))
                            {
                                continue;
                            }
                            validPositions.Add(sidePos);
                        }
                    }
                }
            }
        }
    }
}
