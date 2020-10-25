using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class Structure : Object
{
    [Header("Параметры строения")]
    [Tooltip("количество выстрелов за ход")]
    [Range(1, 10)]
    public int rate;
    public int damage;
    public int range;
    [Space]
    [Header("Техническая инфа")]
    [Tooltip("тайл который заменит ландшафт под сооружением")]
    public TileBase ground;
    public TileBase tileImg;   // !!! только для информации при создании тайла через карточку, не создавать тайл в ручную!!!




    public ParticleSystem Effect;


    List<TerrainTile> shotZone = new List<TerrainTile>();


    public void Shoot()
    {
        List<TerrainTile> enemies = SearchEnemy();
        if (enemies.Count != 0)
        {
            for (int i = 0; i < rate; i++)
            {
                int rnd = Random.Range(0, enemies.Count);
                enemies[rnd].currentUnit.takeDamage(damage);
            }

            // Debug.Log("fire");
        }
        
    }

    private List<TerrainTile> SearchEnemy()
    {
        List<TerrainTile> enemies = new List<TerrainTile>();

        foreach (TerrainTile tile in shotZone)
        {
            if (tile.currentUnit != null)
            {
                enemies.Add(tile);
            }
        }

        return enemies;
    }

    private void OnMouseDown()
    {
        Shoot();
        
    }

    public override void NpcAct()
    {
        Shoot();
    }

    public override void Death()
    {
        NPCsManager.defTeam.Remove(this);
        base.Death();
    }


    void Start()
    {
        SetTag("Structure");

        //shoot_test = AssetDatabase.LoadAssetAtPath<TileBase>("/Assets/Tiles/shot_test");

        NPCsManager.defTeam.Add(this);

        StartCoroutine("delayedStart");

        TilemapsManager.TilemapTerrain.SetTile(TilemapsManager.TilemapTerrain.WorldToCell(transform.position), ground);

    }

    IEnumerator delayedStart()
    {
        yield return new WaitForSeconds(0.1f);
        Attach();


        for (int r = 1; r <= range; r++)
        {
            shotZone.Add(GetTerrain(gameObject.transform.position + r * up));
            shotZone.Add(GetTerrain(gameObject.transform.position + r * upleft));
            shotZone.Add(GetTerrain(gameObject.transform.position + r * upright));
            // TODO добавь как-то не простреливать поверх других траншей? и стрельбу назад 
        }

        /*
        foreach (TerrainTile tile in shotZone)
        {
            Debug.DrawLine(tile.gameObject.transform.position, tile.transform.position + Vector3.back, Color.yellow, 5f);
        }
        */

    }


}
