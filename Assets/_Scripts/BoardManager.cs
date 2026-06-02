using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    public BoardTile[,] Tiles;

    [SerializeField] GameObject PiecePrefab, PreviewPrefab, TilePrefab;
    [SerializeField] Transform TileHolder;
    [SerializeField] Color EvenColor, OddColor;
    [SerializeField] int Width = 2, Height = 2;
    [SerializeField] float AnimationSpeed = 5;

    Dictionary<BoardTile, int> TurnsUntilEffect = new Dictionary<BoardTile, int>();

    PieceCache[] PieceCache; //piece info to load upon reload

    GameObject[,] PreviewGrid;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnChange += DecrementTurns;
    }

    //add delayed attack
    public void SetBoard()
    {
        DestroyPreviewGrid();

        Tiles = new BoardTile[Width, Height];
        Vector2 offset = new Vector2((Width / 2f)-0.5f, (Height / 2f)-0.5f);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                GameObject obj = Instantiate(TilePrefab, new Vector2(x, y) - offset, Quaternion.identity, transform);
                BoardTile currentTile = obj.GetComponent<BoardTile>();
                currentTile.GridPos = new Vector2Int(x, y);
                Tiles[x, y] = currentTile;
            }
        }

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
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

        Vector2Int[] validPositions = piece.GetValidTargets(piece.CurrentTile);
        foreach(Vector2Int pos in validPositions)
        {
            if(pos == newTile.GridPos)
            {
                Vector2 oldTilePos = piece.CurrentTile.transform.position;
                piece.CurrentTile.OccupyingPiece = null;
                piece.CurrentTile = newTile;
                newTile.OccupyingPiece = piece;

                //a clean seperation of visuals and position is needed in order for this to be functional, otherwise it will be buggy
                piece.transform.position = newTile.transform.position;
                //StartCoroutine(MoveAnimation(piece.transform, oldTilePos, newTile.transform.position));

                piece.HasMoved = true;
                return;
            }
        }
    }
    IEnumerator MoveAnimation(Transform transToUpdate, Vector2 oldPos, Vector2 newPos)
    {
        float timeElapsed = 0f;
        float distToNew = Vector2.Distance(transToUpdate.position, newPos);
        while (Mathf.Abs(distToNew) > 0.01f)
        {
            float duration = (Vector2.Distance(newPos, oldPos) / AnimationSpeed);
            if(transToUpdate != null)
            {
                transToUpdate.position = Vector2.Lerp(oldPos, newPos, timeElapsed / duration);
            }
            timeElapsed += Time.deltaTime;
            Debug.Log("moving");
            yield return null;
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
    public void AddDelayedEffect(BoardTile effectTile, int turnsUntilEffect)
    {
        effectTile.Warning.SetActive(true);
        if(!TurnsUntilEffect.ContainsKey(effectTile))
        {
            TurnsUntilEffect.Add(effectTile, turnsUntilEffect);
        }
    }
    void ActivateEffect(BoardTile tile)
    {
        Debug.Log("Effect triggered");
        if (tile.OccupyingPiece != null)
        {
            Destroy(tile.OccupyingPiece.gameObject);
        }
        
        tile.Warning.SetActive(false);
    }

    //triggered by turnmanager's action
    void DecrementTurns()
    {
        List<BoardTile> toRemove = new List<BoardTile>();
        foreach(var kvp in TurnsUntilEffect.ToList())
        {
            int newValue = kvp.Value - 1;
            TurnsUntilEffect[kvp.Key] = newValue;
            if(newValue <= 0)
            {
                ActivateEffect(kvp.Key);
                toRemove.Add(kvp.Key);
            }
        }

        foreach(BoardTile tile in toRemove)
        {
            TurnsUntilEffect.Remove(tile);
        }
    }
    public void ResetPieces()
    {
        //Destroys all pieces
        foreach(Piece piece in FindObjectsOfType<Piece>())
        {
            DestroyImmediate(piece.gameObject);
        }
        TurnsUntilEffect.Clear();
        foreach(BoardTile tile in FindObjectsOfType<BoardTile>())
        {
            tile.Warning.SetActive(false);
        }

        TurnManager.Instance.ResetTurn();

        //Makes new copies
        for(int i = 0; i < PieceCache.Length; i++)
        {
            GameObject pieceObj = Instantiate(PiecePrefab, PieceCache[i].CurrentTile.transform.position, Quaternion.identity);
            Piece piece = pieceObj.GetComponent<Piece>();
            piece.CurrentPieceType = PieceCache[i].PieceType;
            piece.CurrentTile = PieceCache[i].CurrentTile;
            piece.HasMoved = false;
            //piece.StartingTile = PieceCache[i].CurrentTile;
            piece.CurrentTeam = PieceCache[i].Team;
            piece.Renderer.color = PieceCache[i].Color;

            piece.MoveAndSpriteConfig();

            piece.CurrentTile.OccupyingPiece = piece;
        }

        PieceManager.Instance.DeselectPiece();
    }

    [ContextMenu("Generate Preview Grid")]
    void GeneratePreviewGrid()
    {
        //destroy and clear
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        
        PreviewGrid = new GameObject[Width, Height];
        Vector2 offset = new Vector2((Width / 2f) - 0.5f, (Height / 2f) - 0.5f);

        if (Height > 0 && Width > 0)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    //spawn prefab
                    PreviewGrid[x, y] = Instantiate(PreviewPrefab, new Vector2(x, y) - offset, Quaternion.identity, transform);
                }
            }
        }
        else
        {
            Debug.Log("x or y is <= 0");
        }
    }
    [ContextMenu("Destroy Preview Grid")]
    void DestroyPreviewGrid()
    {
        //destroy and clear
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
