using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static General;


public class Unit : Object
{
    [Tooltip("сколько тайлов пройдено за ход"), SerializeField, Range(1, 5)]
    private int speed;

    [Tooltip("скорость анимации ходьбы (в секундах)"), SerializeField]
    private float animationSpeed = 1;


    [Space, SerializeField]
    private ParticleSystem Effect; // черновик


    private bool CheckField(TerrainTile tile, out int tileRating)
    {
        const float coverCoef = 2;
        tileRating = (int)(tile.cover * coverCoef + tile.mobility);

        if (tile.currentUnit != null)
        {
            return false;
        }
        else if (tile.currentStructure != null)
        {
            tileRating *= 2;
        }

        return true;
    }


    // проверка на проходимость, выбор пути, перемещение
    private void MovementTurn()
    {
        // фикс бага при слишком частой ходьбе
        StopCoroutine(nameof(MoveToTile));
        transform.position = currentTile.transform.position + Vector3.back;

        if (DiceCheck(basic: 8, modifier: this.Mobility + currentTile.mobility))  // успешная проврка - выбраться
        {
            TerrainTile choosenTile = ChooseTile();

            StartCoroutine(MoveToTile(choosenTile));
        }
        else
        {
            // застрял
        }

    }

    private TerrainTile ChooseTile()
    {
        List<TerrainTile> ways = new List<TerrainTile>();   // тайлы нижней полусферы 

        ways.Add(GetTerrain(gameObject.transform.position + downLeft));
        ways.Add(GetTerrain(gameObject.transform.position + down));
        ways.Add(GetTerrain(gameObject.transform.position + downRight));

        List<TerrainTile> possibleWays = new List<TerrainTile>();

        for (int i = 0; i < ways.Count; i++)
        {
            if (ways[i].currentStructure != null)
            {
                return ways[i];
            }
            else if (ways[i].currentUnit == null)
            {
                possibleWays.Add(ways[i]);
            }
        }

        if (possibleWays.Count != 0)
        {
            int rnd = Random.Range(0, possibleWays.Count);

            return possibleWays[rnd];
        }
        else
        {
            return null;
        }

    }

    private float EasingInOut(float x)
    {
        return x < 0.5 ? 4 * Mathf.Pow(x, 3) : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }

    private IEnumerator MoveToTile(TerrainTile destinationTile)
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
            StartCoroutine(MoveToTile(attackedTile));
        }
    }
    

    IEnumerator Start()
    {
        SetTag("Unit");
        NPCsManager.JoinTeam<Unit>(this);

        yield return null;
        AttachTerrainTile();

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
        MovementTurn();
    }

    public override IEnumerator NpcAct()
    {
        yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
        MovementTurn();

        //TODO сюда добавить потом стрельбу юнита по структурам

        // не костыль, но можно как-то более органично имплементировать в другой код
        // что чинит: NCPsManager обращается последовательно ко всем юнитам. Без этого кода юниты начали бы действовать 
        // сразу, и в случае смерти меняли список по которому идет менеджер
    }


}
