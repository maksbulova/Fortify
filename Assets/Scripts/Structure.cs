using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using static General;

public class Structure : Object
{
    [Header("Параметры строения")]

    [Tooltip("структура может стрелять через другую структуру если стрелок выше препятсвия")]
    [Range(0, 3)] public int high;


    [Space, Header("Стоимость")]


    [Space, Header("Техническая инфа")]
    [Tooltip("тайл который заменит ландшафт под сооружением")]
    public TileBase ground;
    public TileBase tileImg;   // !!! только для информации при создании тайла через карточку, не создавать тайл в ручную!!!
                               // и что ты имел этим в виду вообще??
                               // п.с. я так понимаю префаб этой вот струкутры перетаскивается в карточку, потом на сцене создает именно тайл, 
                               // а созданый тайл создается вместе с этим вот префабом, в котором обрабатывается вся логика
                               // хспд чиво 


    public void Shoot()
    {
        List<Unit> enemies = SearchEnemy();
        int rnd;

        if (enemies.Count > 0)
        {
            for (int i = 0; i < rate; i++)
            {
                // сломается если одним из выстрелов убьет мишень!!!!!!!
                rnd = Random.Range(0, enemies.Count);   // TODO распределение со смещением в начало списка
                enemies[rnd].takeHit(damage, suppression, accuracy);//, penetration);
            }

            // Debug.Log("fire");
        }
    }

    private List<Unit> SearchEnemy()
    {
        List<Unit> enemies = new List<Unit>();
        TerrainTile tile;

        foreach (Vector3 dir in General.allDirections)
        {
            for (int i = minRange; i <= maxRange; i++)
            {
                tile = General.GetTerrain(gameObject.transform.position + dir * i);

                Debug.DrawLine(tile.gameObject.transform.position, tile.transform.position + Vector3.back, Color.red, 5f);

                if (tile.currentStructure != null && tile.currentStructure.high >= this.high) // если на пути есть структура высотой >= меня, то в эту сторону стрелять дальше нельзя
                {
                    goto Obstacle;
                }
                else if (tile.currentUnit != null)
                {
                    enemies.Add(tile.currentUnit);
                }
            }
        Obstacle:;

        }
        /*
        foreach (Unit unit in enemies)
        {
            Debug.DrawLine(unit.gameObject.transform.position, unit.transform.position + Vector3.back, Color.yellow, 5f);
        }
        */
        return enemies;
    }



    private void OnMouseDown() // только в рамках теста, не забудь добавить колайдер тк в продакте он не нужен
    {
        Shoot();
    }

    public override IEnumerator NpcAct()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
        Shoot();
    }

    protected override void Death()
    {
        NPCsManager.defTeam.Remove(this);

        DetachTerrainTile();
        TilemapsManager.TilemapStructure.SetTile(TilemapsManager.TilemapStructure.WorldToCell(transform.position), null);
    }


    void Start()
    {
        SetTag("Structure");

        //shoot_test = AssetDatabase.LoadAssetAtPath<TileBase>("/Assets/Tiles/shot_test");

        NPCsManager.defTeam.Add(this);

        StartCoroutine("delayedStart");

        TilemapsManager.TilemapTerrain.SetTile(TilemapsManager.TilemapTerrain.WorldToCell(transform.position), ground); // чтоб все строения визуально были на одной высоте

    }


}
