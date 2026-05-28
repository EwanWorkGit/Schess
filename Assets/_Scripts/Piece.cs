using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PieceType { Pawn, Bishop, King, Queen, Knight, Rook, Assassin, Artillery }

//only for base pieces
public enum MoveType { Directional, Positional }

public class Piece : MonoBehaviour
{
    //how pieces can move, NOT ACTUAL STATE CHANGES

    public Team CurrentTeam;
    public PieceType CurrentPieceType;    
    public BoardTile CurrentTile;
    public SpriteRenderer Renderer;
    public bool IsInActionMode = false;
    public float TurnsUntilFire;
    public MoveType CurrentMoveType;
    public Vector2Int[] Directions;
    public Vector2Int[] Offsets;

    [SerializeField] Sprite[] SpriteArray;
    [SerializeField] Sprite DefaultSprite;

    public int Steps = 1;
    float TurnsToFire = 1;

    private void Awake()
    {
        //only does internal stuff
        MoveAndSpriteConfig();
        TurnsUntilFire = TurnsToFire;
        transform.name = $"{CurrentTeam.ToString()} : {CurrentPieceType.ToString()} : {transform.position.x}x {transform.position.y}y";
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
        else if(clickType == ClickType.Move && TileIsValid(pieceManager.SelectedPiece.CurrentTile, clickedTile))
        {
            boardManager.MovePieceToTile(clickedTile, pieceManager.SelectedPiece);
            turnManager.ChangeTeam(false);
            pieceManager.DeselectPiece();
        }
        else if(clickType == ClickType.Capture && TileIsValid(pieceManager.SelectedPiece.CurrentTile, clickedTile))
        {
            boardManager.CaptureOtherPiece(pieceManager.SelectedPiece, clickedPiece, clickedTile);
            boardManager.MovePieceToTile(clickedTile, pieceManager.SelectedPiece);
            turnManager.ChangeTeam(false);
            pieceManager.DeselectPiece();
        }
    }

    public void MoveAndSpriteConfig()
    {
        MovesetLibrary.Instance.DefinePiece(this);

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
        MovesetLibrary movesets = MovesetLibrary.Instance;
        List<Vector2Int> validMoves = new List<Vector2Int>();

        switch (CurrentPieceType)
        {
            case PieceType.Pawn:
                {
                    movesets.PawnMoves(startTile.OccupyingPiece, startTile, boardManager, validMoves);
                    break;
                }
            case PieceType.Assassin:
                {
                    movesets.AssassinMoves(startTile.OccupyingPiece, startTile, boardManager, validMoves);
                    break;
                }
            case PieceType.Artillery:
                {
                    movesets.ArtilleryMoves(startTile.OccupyingPiece, startTile, boardManager, validMoves);
                    break;
                }
            default:
                {
                    movesets.BaseMoves(startTile.OccupyingPiece, startTile, boardManager, validMoves);
                    break;
                }
        }

        return validMoves.ToArray();
    }

    public bool TileIsValid(BoardTile startTile, BoardTile endTile)
    {
        if(startTile == null || endTile == null)
        {
            Debug.LogWarning($"Start or end tile null! (from {transform.name})");
            return false;
        }

        Vector2Int[] validTiles = GetValidTargets(startTile);
        foreach(Vector2Int tile in validTiles)
        {
            if(tile == endTile.GridPos)
            {
                return true;
            }
        }

        return false;
    }
    public bool IsInsideBounds(int width, int height, Vector2Int vec)
    {
        bool insideBounds = vec.x >= 0 && vec.x < width && vec.y >= 0 && vec.y < height;
        return insideBounds;
    }
}
