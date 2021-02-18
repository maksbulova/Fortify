using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : Object
{
    [Header("Параметры юнита")]
    [Tooltip("здоровье"), Range(1, 10)]
    public int health;
    // public int armor; TODO
    [Tooltip("чем выше тем сложнее попасть"), Range(-10, 10)]
    public int cover;
    [Tooltip("чем выше тем вероятнее выбраться"), Range(-10, 10)]
    public int mobility;
    [Tooltip("сколько тайлов пройдено за ход"), Range(1, 5)]
    public int speed;
    [Tooltip("урон за каждого соладта (хп) в отряде"), Range(1, 5)]
    public float meleeDmg;

    [Space, Header("Технические детали")]
    private float supression = 0;
    [Tooltip("скорость анимации ходьбы")]
    private float animSpeed = 4;
    [Space]
    public ParticleSystem Effect; // черновик




    private int RateField(TerrainTile tile)    // оценка тайла
    {
        int Rate = 1; // дефолтный тайл лучше занятого

        if (tile.currentUnit != null)   // занятый тайл оценивает нулем
        {
            Rate = 0;
        }
        else if (tile.currentStructure != null)
        {
            Rate += 5;
        }
        else
        {
            Rate += tile.cover * 2 + mobility;
        }

        return Rate;
    }


    private void Movement()  //  TODO можно оптимизироваь, например не рассматривать занятые тайлы, сразу атаковать структуры
        // проверка на проходимость, выбор пути, перемещение
    {
        if (Check(modifier: this.mobility + currentTile.mobility, basic: 8))  // успешная проврка - выбраться
        {
            // Debug.Log("выбрался");

            StopCoroutine("MoveTo");

            transform.position = currentTile.transform.position + Vector3.back; // фикс бага при слишком частой ходьбе

            List<TerrainTile> ways = new List<TerrainTile>();   // тайлы нижней полусферы 

            ways.Add(GetTerrain(gameObject.transform.position + downleft));
            ways.Add(GetTerrain(gameObject.transform.position + down));
            ways.Add(GetTerrain(gameObject.transform.position + downright));

            List<int> weights = new List<int>(3); 

            int weightSum = 0;

            for (int i = 0; i < ways.Count; i++)
            {
                weights.Add(RateField(ways[i]));
                weightSum += weights[i];
            }

            if (weights[1] > 0)
            {
                weights[1] += 2;  // приоритет к движению по прямой
                weightSum += 2;
            }

            if (weightSum == 0) // если нет доступных путей
            {
                supression += 1;
            }
            else
            {
                TerrainTile tileToMove;
                int rnd = Random.Range(0, weightSum);
                if (rnd < weights[0])
                {
                    tileToMove = ways[0];
                }
                else if (rnd < weights[0] + weights[1])
                {
                    tileToMove = ways[1];
                }
                else
                {
                    tileToMove = ways[2];
                }

                if (tileToMove.currentStructure != null)
                {
                    MeleeCharge(tileToMove);
                }
                else
                {
                    StartCoroutine(MoveTo(tileToMove));
                }
            }

        }
        else
        {
            // Debug.Log("застрял");
        }

    }

    private IEnumerator MoveTo(TerrainTile tile)
    {
        transform.position = currentTile.transform.position + Vector3.back;

        Vector3 pos = tile.transform.position + Vector3.back;

        Detach();
        Attach(tile);

        GetComponent<SpriteRenderer>().sortingLayerName = "units";
        
        Vector3 dir;

        while((pos - this.transform.position).magnitude > 0.01f)
        {
            dir = (pos - this.transform.position) * animSpeed;
            transform.Translate(dir * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        transform.position = pos + Vector3.down * 0.001f; // полукостыль для рендера поверх терейна
        GetComponent<SpriteRenderer>().sortingLayerName = "terrain";


    }

    
    private void MeleeCharge(TerrainTile tile) // ход на клетку со структурой, нужен чтобы два юнита могли атаковать одну структуру, но не ходили на одно поле
    {
        Structure structure = tile.currentStructure;

        int dmgToDef = Mathf.FloorToInt(this.health * this.meleeDmg);
        int dmgToAtt = Mathf.FloorToInt(structure.health * structure.meleeDmg);

        if (this.health > dmgToAtt) // выживет 
        {
            structure.takeDamage(dmgToDef);
            this.takeDamage(dmgToAtt);
            StartCoroutine(MoveTo(tile));
        }
        else  // не выживет, тоже нанесет урон и может уничтожить защитника, но оставит дорогу другому юниту для атаки
        {
            structure.takeDamage(dmgToDef);
            this.takeDamage(dmgToAtt);
        }
    }
    

    void Start()
    {
        SetTag("Unit");

        StartCoroutine("delayedStart");

        NPCsManager.attackTeam.Add(this);
    }

    public void takeHit(int dmg, float supres, int accuracy)//, int penetration)
    {
        // Debug.Log("hit");

        Instantiate(Effect, transform.position, Quaternion.identity);

        if (Check(modifier: +accuracy -currentTile.cover)) // успех проверки - попадание
        {
            // TODO чек на броню
            takeDamage(dmg);

        }
        this.supression += supres; // интересно оно вызовет ошибку раз при смерти эта штука вызывается после дестроя?
    }
    public override void takeDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
            Death();
    }

    protected override void Death()
    {
        NPCsManager.attackTeam.Remove(this);

        Detach();
        Destroy(gameObject);

    }

    private void OnMouseDown()
    {
        Movement();
    }

    public override void NpcAct()
    {
        StartCoroutine(delayedAct());
        // Movement();

        //TODO сюда добавить потом стрельбу юнита по структурам
    }

    private IEnumerator delayedAct() // не костыль, но можно как-то более органично имплементировать в другой код
    // что чинит: NCPsManager обращается последовательно ко всем юнитам. Без этого кода юниты начали бы действовать 
    // сразу, и в случае смерти меняли список по которому идет менеджер
    {
        yield return new WaitForFixedUpdate();
        Movement();
    }

}
