using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PieceType { Pawn, Bishop, King, Queen, Knight, Rook, Assassin, Artillery }

public class Piece : MonoBehaviour
{
    //how pieces can move, NOT ACTUAL STATE CHANGES

    //only for base pieces
    enum MoveType { Directional, Positional }

    public Team CurrentTeam;
    public PieceType CurrentPieceType;    
    public BoardTile CurrentTile;
    public SpriteRenderer Renderer;
    public bool IsInActionMode = false;
    public float TurnsUntilFire;

    [SerializeField] Sprite[] SpriteArray;
    [SerializeField] Sprite DefaultSprite;

    MoveType CurrentMoveType;
    Vector2Int[] Directions;
    Vector2Int[] Offsets;

    int Steps = 1, TurnsToFire = 1;

    private void Awake()
    {
        //only does internal stuff
        MoveAndSpriteConfig();
        TurnsUntilFire = TurnsToFire;
        transform.name = $"{CurrentTeam.ToString()} : {CurrentPieceType.ToString()} : {transform.position.x}x {transform.position.y}y";
    }

    bool IsInsideBounds(int width, int height, Vector2Int vec)
    {
        bool insideBounds = vec.x >= 0 && vec.x < width && vec.y >= 0 && vec.y < height;
        return insideBounds;
    }

    //moves and sprites
    public void HandleClickInteraction(ClickType clickType, Piece clickedPiece, BoardTile clickedTile)
    {
        BaseClickInteraction(clickType, clickedPiece, clickedTile);
    }
    void BaseClickInteraction(ClickType clickType, Piece clickedPiece, BoardTile clickedTile)
    {
        PieceManager pieceManager = PieceManager.Instance;
        BoardManager boardManager = BoardManager.Instance;
        TurnManager turnManager = TurnManager.Instance;

        //when something happens, not before

        if (clickType == ClickType.Select)
        {
            pieceManager.DeselectPiece();
            pieceManager.SelectPiece(clickedPiece);
        }
        else if(clickType == ClickType.Move)
        {
            boardManager.MovePieceToTile(clickedTile, pieceManager.SelectedPiece);
            turnManager.ChangeTeam(false);
            pieceManager.DeselectPiece();
        }
        else if(clickType == ClickType.Capture)
        {
            boardManager.CaptureOtherPiece(pieceManager.SelectedPiece, clickedPiece, clickedTile);
            boardManager.MovePieceToTile(clickedTile, pieceManager.SelectedPiece);
            turnManager.ChangeTeam(false);
            pieceManager.DeselectPiece();
        }
    }

    public void MoveAndSpriteConfig()
    {
        //DIRECTIONAL
        if (CurrentPieceType == PieceType.Bishop)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
            };

            Steps = 100;
            CurrentMoveType = MoveType.Directional;
        }
        else if (CurrentPieceType == PieceType.King)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
                new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
            };

            Steps = 1;
            CurrentMoveType = MoveType.Directional;
        }
        else if (CurrentPieceType == PieceType.Queen)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1),
                new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1)
            };

            Steps = 100;
            CurrentMoveType = MoveType.Directional;
        }
        else if (CurrentPieceType == PieceType.Rook)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(-1,0), new Vector2Int(0,-1)
            };

            Steps = 100;
            CurrentMoveType = MoveType.Directional;
        }
        else if (CurrentPieceType == PieceType.Assassin)
        {
            Directions = new Vector2Int[]
            {
                new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
            };

            Steps = 4;
            CurrentMoveType = MoveType.Directional;
        }

        //POSITIONAL
        if (CurrentPieceType == PieceType.Pawn)
        {
            if (CurrentTeam == Team.White)
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
        else if (CurrentPieceType == PieceType.Knight)
        {
            Offsets = new Vector2Int[]
            {
                new Vector2Int(1, 2), new Vector2Int(1, -2), new Vector2Int(-1, 2), new Vector2Int(-1, -2),
                new Vector2Int(2,1), new Vector2Int(2, -1), new Vector2Int(-2,1), new Vector2Int(-2,-1)
            };

            CurrentMoveType = MoveType.Positional;
        }

        //MIX OF BOTH
        else if(CurrentPieceType == PieceType.Artillery)
        {
            Offsets = new Vector2Int[]
            {
                new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1)
            };

            Directions = new Vector2Int[]
            {
                new Vector2Int(0,1) * (CurrentTeam == Team.White ? 1 : -1)
            };

            Steps = 100;
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
    public void AssignClosestTileToCurrent()
    {
        BoardTile closestTile = null;
        float closestDistance = Mathf.Infinity;

        foreach (BoardTile tile in BoardManager.Instance.Tiles)
        {
            float dist = Vector2.Distance(transform.position, tile.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestTile = tile;
            }
        }

        CurrentTile = closestTile;
    }
    
    //puts valid movesets into a list
    public Vector2Int[] GetValidTargets(BoardTile startTile)
    {
        BoardManager boardManager = BoardManager.Instance;
        List<Vector2Int> validMoves = new List<Vector2Int>();

        switch (CurrentPieceType)
        {
            case PieceType.Pawn:
                {
                    PawnMoves(startTile, boardManager, validMoves);
                    break;
                }
            case PieceType.Assassin:
                {
                    AssassinMoves(startTile, boardManager, validMoves);
                    break;
                }
            case PieceType.Artillery:
                {
                    ArtilleryMoves(startTile, boardManager, validMoves);
                    break;
                }
            default:
                {
                    BaseMoves(startTile, boardManager, validMoves);
                    break;
                }
        }

        return validMoves.ToArray();
    }

    //These manage the pieces individual movesets
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
            if (!IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos))
            {
                continue;
            }

            //movable pieces
            if (boardManager.Tiles[pos.x, pos.y].OccupyingPiece == null)
            {
                validPositions.Add(pos);
            }
        }
        
        //captures
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
    void AssassinMoves(BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
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
    void ArtilleryMoves(BoardTile startTile, BoardManager boardManager, List<Vector2Int> validPositions)
    {
        if(!IsInActionMode)
        {
            foreach (Vector2Int offset in Offsets)
            {
                Vector2Int pos = startTile.GridPos + offset;
                //out of bounds
                if (!IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(0), pos) || boardManager.Tiles[pos.x,pos.y].OccupyingPiece != null)
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
            foreach(Vector2Int dir in Directions)
            {
                for (int i = 1; i <= Steps; i++)
                {
                    Vector2Int pos = startTile.GridPos + (dir * i);
                    if(!IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), pos))
                    {
                        break;
                    }
                    validPositions.Add(pos);
                    float dist = Vector2Int.Distance(startTile.GridPos, pos);
                    float distPerWidth = 3f; //how large the distance has to be for width to increase by 1
                    int width = (int)Mathf.Floor(dist / distPerWidth);
                    
                    Vector2Int[] sideOffsets = new Vector2Int[] { new Vector2Int(1,0), new Vector2Int(-1, 0) };
                    foreach(Vector2Int sideOffset in sideOffsets)
                    {
                        for (int j = 1; j <= width; j++)
                        {
                            int x = pos.x + (sideOffset.x * j);
                            Vector2Int sidePos = new Vector2Int(x, pos.y);
                            if (!IsInsideBounds(boardManager.Tiles.GetLength(0), boardManager.Tiles.GetLength(1), sidePos))
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
