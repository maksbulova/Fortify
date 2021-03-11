using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Image infoImg;
    public Text infoDescription;


    private void Start()
    {
        StartCoroutine(CheckObject());
    }

    private IEnumerator CheckObject()
    {
        while (true)
        {
            DisplayInfo(Object.GetTerrain(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            yield return null;
        }
    }

    private void DisplayInfo(TerrainTile tile)
    {
        if (tile.currentStructure != null)
        {
            
        }
        else if (tile.currentUnit != null)
        {
            Unit unit = tile.currentUnit;
            infoImg.sprite = unit.gameObject.GetComponent<SpriteRenderer>().sprite;
            infoDescription.text = $"Health: {unit.health} \nCurrent cover: {unit.SumCover}";
        }
        else
        {

        }
    }
}
