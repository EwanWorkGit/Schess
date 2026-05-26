using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PieceType { Pawn, Bishop, King, Queen, Knight, Rook, Assassin }

public class Piece : MonoBehaviour
{
    enum MoveType { Directional, Positional }

    public Team CurrentTeam;
    public PieceType CurrentPieceType;    
    public BoardTile CurrentTile;
    public SpriteRenderer Renderer;

    [SerializeField] Sprite[] SpriteArray;
    [SerializeField] Sprite DefaultSprite;

    MoveType CurrentMoveType;
    Vector2Int[] Directions;
    Vector2Int[] Offsets;

    int Steps = 1;

    private void Awake()
    {
        PieceConfig();
    }

    private void Start()
    {
        if(CurrentTile != null)
        {
            CurrentTile.OccupyingPiece = this;
        }

        transform.name = $"{CurrentTeam.ToString()} : {CurrentPieceType.ToString()} : {transform.position.x}x {transform.position.y}y";
    }

    public void PieceConfig()
    {
        //DIRECTIONAL
        if(CurrentPieceType == PieceType.Bishop)
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
        else if(CurrentPieceType == PieceType.Assassin)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
            };

            Steps = 2;
            CurrentMoveType = MoveType.Directional;
        }

        //POSITIONAL
        if(CurrentPieceType == PieceType.Pawn)
        {
            if(CurrentTeam == Team.White)
            {
                Offsets = new Vector2Int[] { new Vector2Int(0, 1) };
            }
            else
            {
                Offsets = new Vector2Int[] { new Vector2Int(0, -1) };
            }

            Steps = 1;
            CurrentMoveType = MoveType.Positional;
        }
        else if(CurrentPieceType == PieceType.Knight)
        {
            Offsets = new Vector2Int[]
            {
                new Vector2Int(1, 2), new Vector2Int(1, -2), new Vector2Int(-1, 2), new Vector2Int(-1, -2),
                new Vector2Int(2,1), new Vector2Int(2, -1), new Vector2Int(-2,1), new Vector2Int(-2,-1)
            };

            CurrentMoveType = MoveType.Positional;
        }

        int index = (int)CurrentPieceType;
        if (index < 0 || index >= SpriteArray.Length)
        {
            Renderer.sprite = DefaultSprite;   
        }
        else
        {
            Renderer.sprite = SpriteArray[index];
        }
    }

    public Vector2Int[] GetValidPositions(BoardTile startTile)
    {
        BoardManager boardManager = BoardManager.Instance;
        List<Vector2Int> validPositions = new List<Vector2Int>();

        switch (CurrentPieceType)
        {
            case PieceType.Pawn:
                {
                    PawnMoves(startTile, boardManager, validPositions);
                    break;
                }
            case PieceType.Assassin:
                {
                    AssasinMoves(startTile, boardManager, validPositions);
                    break;
                }
            default:
                {
                    BaseMoves(startTile, boardManager, validPositions);
                    break;
                }
        }

        return validPositions.ToArray();
    }

    void BaseMoves(BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
    {
        if (CurrentMoveType == MoveType.Directional)
        {
            foreach (Vector2Int dir in Directions)
            {
                for (int i = 1; i <= Steps; i++)
                {
                    Vector2Int pos = startTile.GridPos + (dir * i);

                    //out of bounds check
                    if (!IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos))
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
                        if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece.CurrentTeam != CurrentTeam)
                        {
                            validPositions.Add(pos);
                        }

                        //breaks for this direction, so you cant pass occupied tiles
                        break;
                    }
                }
            }
        }
        else if (CurrentMoveType == MoveType.Positional)
        {
            foreach (Vector2Int offset in Offsets)
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
    }

    void PawnMoves(BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
    {
        foreach (Vector2Int offset in Offsets)
        {
            //out of bounds
            Vector2Int pos = startTile.GridPos + offset;
            if (pos.x < 0 || pos.x >= boardManager.Tiles.GetLength(0) || pos.y < 0 || pos.y >= boardManager.Tiles.GetLength(1))
            {
                continue;
            }

            //movable pieces
            if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece == null)
            {
                validPositions.Add(pos);
            }
        }

        //sideways moves
        Vector2Int[] captureOffsets = new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(-1, 1) };

        foreach (Vector2Int offset in captureOffsets)
        {
            Vector2Int pos = startTile.GridPos + (CurrentTeam == Team.White ? offset : -offset);
            if (IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos) && boardManager.Tiles[pos.x, pos.y].OccupyingPiece != null)
            {
                validPositions.Add(pos);
            }
        }
    }

    void AssasinMoves(BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
    {
        foreach (Vector2Int dir in Directions)
        {
            bool passedPiece = false;
            for (int i = 1; i <= Steps; i++)
            {
                Vector2Int pos = startTile.GridPos + (dir * i);

                //out of bounds check
                if (!IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos))
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
                    if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece.CurrentTeam != CurrentTeam)
                    {
                        //piece capture position
                        if (passedPiece)
                        {
                            validPositions.Add(pos);
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

    public void CaptureOtherPiece(Piece pieceToCapture, BoardTile captureTile)
    {
        Vector2Int[] validPositions = GetValidPositions(CurrentTile);
        foreach(Vector2Int pos in validPositions)
        {
            if(pos == captureTile.GridPos)
            {
                Destroy(pieceToCapture.gameObject);
                return;
            }
        }
    }

    bool IsInsideBounds(int width, int height, Vector2Int vec)
    {
        bool insideBounds = vec.x >= 0 && vec.x < width && vec.y >= 0 && vec.y < height;
        return insideBounds;
    }
}
