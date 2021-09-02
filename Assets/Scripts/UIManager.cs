using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public Text infoTitle;
    public Image infoImg;
    public Text infoText;
    

    private void Start()
    {
        StartCoroutine(CheckObject());
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown("i"))
        {
            DisplayInfo(General.GetTerrain(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
        */
    }

    private IEnumerator CheckObject()
    {
        while (true)
        {
            DisplayInfo(General.GetTerrain(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            
            yield return null;
        }
    }

    private void DisplayInfo(TerrainTile tile)
    {
        if (tile == null)
            return;

        // інфа про структуру, юніт чи терейн під курсором
        if (tile.currentStructure != null)
        {
            Structure structure = tile.currentStructure;
            infoTitle.text = FormateTitle(structure.gameObject.name);
            infoImg.sprite = TilemapsManager.TilemapStructure.GetSprite(TilemapsManager.TilemapStructure.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            infoText.text = $"Health: {structure.health}";
        }
        else if (tile.currentUnit != null)
        {
            Unit unit = tile.currentUnit;
            infoTitle.text = FormateTitle(unit.gameObject.name);
            infoImg.sprite = unit.gameObject.GetComponent<SpriteRenderer>().sprite;
            infoText.text = $"Health: {unit.health} \nCurrent cover: {unit.SumCover}";
        }
        else
        {
            infoTitle.text = FormateTitle(tile.gameObject.name);
            infoImg.sprite = TilemapsManager.TilemapTerrain.GetSprite(TilemapsManager.TilemapTerrain.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            infoText.text = $"Cover: {tile.cover} \nMobility: {tile.mobility}";
        }
    }

    private string FormateTitle(string rawName)
    {
        return (char.ToUpper(rawName[0]) + rawName.Substring(1)).Replace("(Clone)", "");
    }
}
