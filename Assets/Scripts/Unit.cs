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
    [Tooltip("скорость анимации ходьбы (в секундах)")]
    public float animSpeed = 1;

    public int SumCover
    {
        get
        {
            return this.cover + currentTile.cover;
        }
    }

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
        if (General.DiceCheck(modifier: this.mobility + currentTile.mobility, basic: 8))  // успешная проврка - выбраться
        {
            // Debug.Log("выбрался");

            StopCoroutine("MoveTo");

            transform.position = currentTile.transform.position + Vector3.back; // фикс бага при слишком частой ходьбе

            List<TerrainTile> ways = new List<TerrainTile>();   // тайлы нижней полусферы 

            ways.Add(General.GetTerrain(gameObject.transform.position + General.downleft));
            ways.Add(General.GetTerrain(gameObject.transform.position + General.down));
            ways.Add(General.GetTerrain(gameObject.transform.position + General.downright));

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

    private float EasingInOut(float x)
    {
        return x < 0.5 ? 4 * Mathf.Pow(x, 3) : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }

    private IEnumerator MoveTo(TerrainTile destinationTile)
    {
        Vector3 startPos = currentTile.transform.position + Vector3.back;
        Vector3 finishPos = destinationTile.transform.position + Vector3.back;

        Detach();
        Attach(destinationTile);

        GetComponent<SpriteRenderer>().sortingLayerName = "units";


        for (float t = 0; t <= 1; t += Time.deltaTime / animSpeed)
        {
            transform.position = Vector3.Lerp(startPos, finishPos, EasingInOut(t));
            yield return new WaitForFixedUpdate();
        }


        transform.position = finishPos + Vector3.down * 0.001f; // полукостыль для рендера поверх терейна
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
            // TODO движение или анимация
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

        if (General.DiceCheck(modifier: +accuracy -SumCover)) // успех проверки - попадание
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

    public override IEnumerator NpcAct()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
        Movement();

        //TODO сюда добавить потом стрельбу юнита по структурам
    }
    // не костыль, но можно как-то более органично имплементировать в другой код
    // что чинит: NCPsManager обращается последовательно ко всем юнитам. Без этого кода юниты начали бы действовать 
    // сразу, и в случае смерти меняли список по которому идет менеджер
}
