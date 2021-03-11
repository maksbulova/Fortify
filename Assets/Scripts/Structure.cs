using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class Structure : Object
{
    [Header("Параметры строения")]

    // TODO range ограничивает поля не только в инспекторе но и коде, проверь чтоб не было несостыковок
    [Tooltip("Сколько урона переживет  структура")]
    [Range(1, 10)] public int health;
    [Tooltip("количество выстрелов за ход")]
    [Range(1, 10)] public int rate;   // TODO приравнять к HP
    [Tooltip("Урон наносимый за 1 попадание (1 пуля = 1 труп) ")]
    [Range(1, 10)] public int damage;
    [Tooltip("Минимальная и максимальная дальность стрельбы (включительно)")]
    [Range(1, 10)] public int maxRange;
    [Range(1, 10)] public int minRange=1;
    [Tooltip("урон в рукопашной за каждое хп")]
    [Range(1, 5)] public float meleeDmg;
    [Tooltip("структура может стрелять через другую структуру если стрелок выше препятсвия")]
    [Range(0, 3)] public int high;
    // TODO armor
    // public Armor armor;
    [Tooltip("Эффективность подавления. Если подавление 0.5, то противник будет прижат только при одновременном огне с двух позиций")]
    [Range(0, 5)] public float suppression;
    [Tooltip("Влияет противоположно укрытию")]
    [Range(-3, 3)] public int accuracy;




    [Space, Header("Техническая инфа")]
    [Tooltip("тайл который заменит ландшафт под сооружением")]
    public TileBase ground;
    public TileBase tileImg;   // !!! только для информации при создании тайла через карточку, не создавать тайл в ручную!!!
    // и что ты имел этим в виду вообще??
    // п.с. я так понимаю префаб этой вот струкутры перетаскивается в карточку, потом на сцене создает именно тайл, 
    // а созданый тайл создается вместе с этим вот префабом, в котором обрабатывается вся логика


    public void Shoot()
    {
        List<Unit> enemies = SearchEnemy();
        int rnd;

        if (enemies.Count > 0)
        {
            for (int i = 0; i < rate; i++)
            {
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

    public override void takeDamage(int dmg) // если не будет ничем отличаться от той же функции в юните, то можно вынести в родителя
    {
        health -= dmg;

        if (health <= 0)
            Death();
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

        Detach();
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
