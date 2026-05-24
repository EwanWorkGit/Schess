using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public GameObject SeeThroughTile;
    public Vector2Int GridPos;
    public SpriteRenderer Renderer;
    public Piece OccupyingPiece;

    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        SeeThroughTile.transform.position = transform.position;
        SeeThroughTile.SetActive(false);
    }
}
