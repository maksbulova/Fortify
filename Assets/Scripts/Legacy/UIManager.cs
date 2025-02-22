using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Legacy
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Text infoTitle;
        [SerializeField] private Image infoImg;
        [SerializeField] private Text infoText;
    

        private void Start()
        {
            StartCoroutine(CheckObject());
        }

        private IEnumerator CheckObject()
        {
            while (true)
            {
                Vector3 mousePos = Input.mousePosition;
                Vector3 tilePos = Camera.main.ScreenToWorldPoint(mousePos);
                TerrainTile tile = General.GetTerrain(tilePos);
                DisplayInfo(tile);
            
                yield return null;
            }
        }

        private void DisplayInfo(TerrainTile tile)
        {
            if (tile == null)
                return;

            // інфа про юніт, структуру чи терейн під курсором
            if (tile.currentUnit != null)
            {
                Unit unit = tile.currentUnit;
                infoTitle.text = FormateTitle(unit.gameObject.name);
                infoImg.sprite = unit.gameObject.GetComponent<SpriteRenderer>().sprite;
                infoText.text = $"Health: {unit.Health} \nCurrent cover: {unit.Cover}";
            }
            else if (tile.currentStructure != null)
            {
                Structure structure = tile.currentStructure;
                infoTitle.text = FormateTitle(structure.gameObject.name);
                infoImg.sprite = TilemapsManager.TilemapStructure.GetSprite(TilemapsManager.TilemapStructure.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
                infoText.text = $"Health: {structure.Health}";
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
}
