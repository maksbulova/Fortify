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
    [Tooltip("тайл который заменит ландшафт под сооружением"), SerializeField]
    private TileBase ground;
    [SerializeField]
    private TileBase structureTile;
    // !!! только для информации при создании тайла через карточку, не создавать тайл в ручную!!!
    // и что ты имел этим в виду вообще??
    // п.с. я так понимаю префаб этой вот струкутры перетаскивается в карточку, потом на сцене создает именно тайл, 
    // а созданый тайл создается вместе с этим вот префабом, в котором обрабатывается вся логика
    // хспд чиво 


    public void Shoot()
    {
        List<Unit> enemies = SearchEnemy();

        for (int i = 0; i < FireRate; i++)
        {
            if (enemies.Count > 0)
            {
                int rng = Random.Range(0, enemies.Count);
                Unit enemy = enemies[rng];
                enemy.TakeHit(DamageType.kinetic, shotDamage, Accuracy, armorPiercing, suppressionEfficiency);

                // if killed
                if (enemy == null)
                {
                    enemies.Remove(enemy);
                }
            }
            else
            {
                return;
            }
        }
    }

    private List<Unit> SearchEnemy()
    {
        List<Unit> enemies = new List<Unit>();

        foreach (Vector3 shotDirection in allDirections)
        {
            for (int i = minRange; i <= maxRange; i++)
            {
                TerrainTile tile = GetTerrain(gameObject.transform.position + shotDirection * i);

                // Debug.DrawLine(tile.gameObject.transform.position, tile.transform.position + Vector3.back, Color.red, 5f);

                // если на пути есть структура высотой >= меня, то в эту сторону стрелять дальше нельзя
                if (tile.currentStructure != null && tile.currentStructure.high >= this.high)
                {
                    break;
                }
                else if (tile.currentUnit != null)
                {
                    enemies.Add(tile.currentUnit);
                }
            }
        }
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
        NPCsManager.LeaveTeam(this);

        DetachTerrainTile();
        TilemapsManager.TilemapStructure.SetTile(TilemapsManager.TilemapStructure.WorldToCell(transform.position), null);
    }


    IEnumerator Start()
    {
        SetTag("Structure");

        //shoot_test = AssetDatabase.LoadAssetAtPath<TileBase>("/Assets/Tiles/shot_test");

        NPCsManager.JoinTeam<Structure>(this);

        yield return null;
        AttachTerrainTile();

        // чтоб все строения визуально были на одной высоте
        TilemapsManager.TilemapTerrain.SetTile(TilemapsManager.TilemapTerrain.WorldToCell(transform.position), ground); 
    }


}
