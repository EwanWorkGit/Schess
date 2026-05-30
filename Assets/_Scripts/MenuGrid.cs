using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGrid : MonoBehaviour
{
    [SerializeField] GameObject TilePrefab;
    [SerializeField] Color EvenColor = Color.white, OddColor = Color.black;
    [SerializeField] int Width = 10, Height = 10;

    private void Awake()
    {
        for(int y = 0; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                Vector2 offset = new Vector2(Width / 2f, Height / 2f);
                GameObject obj = Instantiate(TilePrefab, new Vector2(x, y) - offset, Quaternion.identity, transform);
                SpriteRenderer rend = obj.GetComponent<SpriteRenderer>();
                rend.color = (x+y) % 2 == 0 ? EvenColor : OddColor;
            }
        }
    }
}
