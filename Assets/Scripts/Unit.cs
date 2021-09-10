using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static General;


public class Unit : Object
{
    [Tooltip("сколько тайлов пройдено за ход"), SerializeField, Range(1, 5)]
    private int speed;

    [Space, Header("Технические детали")]
    private float supression = 0;
    [Tooltip("скорость анимации ходьбы (в секундах)"), SerializeField]
    private float animationSpeed = 1;


    [Space, SerializeField]
    private ParticleSystem Effect; // черновик


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
        if (DiceCheck(basic: 8, modifier: this.mobility + currentTile.mobility))  // успешная проврка - выбраться
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
                    MeleeAttack(tileToMove);
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

        DetachTerrainTile();
        AttachTerrainTile(destinationTile);

        GetComponent<SpriteRenderer>().sortingLayerName = "units";


        for (float t = 0; t <= 1; t += Time.deltaTime / animationSpeed)
        {
            transform.position = Vector3.Lerp(startPos, finishPos, EasingInOut(t));
            yield return new WaitForFixedUpdate();
        }


        transform.position = finishPos + Vector3.down * 0.001f; // полукостыль для рендера поверх терейна
        GetComponent<SpriteRenderer>().sortingLayerName = "terrain";


    }

    // ход на клетку со структурой, TODO чтобы два юнита могли атаковать одну структуру, но не ходили на одно поле
    private void MeleeAttack(TerrainTile attackedTile) 
    {
        Structure structure = attackedTile.currentStructure;

        float attackerDmg = Mathf.FloorToInt(this.MeleeDamage);
        int defenderDmg = Mathf.FloorToInt(structure.MeleeDamage);

        structure.TakeDamage(DamageType.melee, attackerDmg);
        this.TakeDamage(DamageType.melee, defenderDmg);

        if (this.health > 0)
        {
            StartCoroutine(MoveTo(attackedTile));
        }
    }
    

    void Start()
    {
        SetTag("Unit");

        StartCoroutine("delayedStart");

        NPCsManager.JoinTeam<Unit>(this);
    }


    protected override void Death()
    {
        StopAllCoroutines();

        NPCsManager.LeaveTeam(this);

        DetachTerrainTile();
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

        // не костыль, но можно как-то более органично имплементировать в другой код
        // что чинит: NCPsManager обращается последовательно ко всем юнитам. Без этого кода юниты начали бы действовать 
        // сразу, и в случае смерти меняли список по которому идет менеджер
    }


}
