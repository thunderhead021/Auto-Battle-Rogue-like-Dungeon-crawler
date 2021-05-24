using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public SpriteRenderer highlightSprite;
    public Color validColor;
    public Color wrongColor;
    public bool EnemyTile;
    public int SortNumber;
    public void SetHighlight(bool active, bool valid)
    {
        highlightSprite.gameObject.SetActive(active);

        highlightSprite.color = valid ? validColor : wrongColor;
    }
}
