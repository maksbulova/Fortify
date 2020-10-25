using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Unit : Object
{
    [Header("Параметры юнита")]
    [Tooltip("здоровье и урон")]
    [Range(1, 10)]
    public int health;
    [Tooltip("снижает входящий урон")]
    [Range(0, 10)]
    public int armor;
    [Tooltip("чем выше тем сложнее попасть")]
    [Range(-10, 10)]
    public int dodge;
    [Tooltip("чем выше тем вероятнее выбраться")]
    [Range(-10, 10)]
    public int mobility;


    [Tooltip("скорость анимации ходьбы")]
    private float speed = 4;

    [Space]
    public ParticleSystem Effect; // черновик



    public bool Unstack(int mobility)
    {
        int rnd = Random.Range(1, 7);

        return (rnd + this.mobility + currentField.mobility > 3) ? true : false;    // чем больше тем вероятнее выбраться
    }


    public void Movement()  // TODO мразотно сделано, переделай, хотя-бы связь тайла к оценке
    {
        List<TerrainTile> ways = new List<TerrainTile>();   // тайлы нижней полусферы 



        ways.Add(GetTerrain(gameObject.transform.position + -upright));
        ways.Add(GetTerrain(gameObject.transform.position + -up));
        ways.Add(GetTerrain(gameObject.transform.position + -upleft));


        List<int> weight = new List<int>(3);

        int weightSum = 0;
        for (int i = 0; i < ways.Count; i++)
        {
            weight.Add(RateField(ways[i]));
            weightSum += weight[i];
        }
        // а еще нет приоритета к движению по прямой
        // TODO и кейс если нет доступных путей

        TerrainTile tileToMove;
        int rnd = Random.Range(0, weightSum+1);
        if (rnd < weight[0])
        {
            tileToMove = ways[0];
        }
        else if (rnd < weight[0] + weight[1])
        {
            tileToMove = ways[1];
        }
        else
        {
            tileToMove = ways[2];
        }

        Detach();
        Attach(tileToMove);
        StartCoroutine(Move(tileToMove.transform.position));
        // transform.position = new Vector3(moveToTile.gameObject.transform.position.x, moveToTile.gameObject.transform.position.y - 0.01f, transform.position.z); // TODO анимацию плавного движения, и косыль с -0,1ф из-за всратого рендера 

        
    }

    private IEnumerator Move(Vector3 pos)
    {
        // transform.position = new Vector3(moveToTile.gameObject.transform.position.x, moveToTile.gameObject.transform.position.y - 0.01f, transform.position.z); // TODO анимацию плавного движения, и косыль с -0,1ф из-за всратого рендера 
        GetComponent<SpriteRenderer>().sortingLayerName = "units";
        
        Vector3 dir;
        pos.z = 0;

        while((pos - transform.position).magnitude > 0.01f)
        {
            dir = (pos - transform.position) * speed;
            dir.z = 0;
            transform.Translate(dir * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        transform.position = pos - Vector3.up * 0.01f;
        GetComponent<SpriteRenderer>().sortingLayerName = "terrain";
    }


    public void Pin()
    {
        // TODO
    }
    
    private void Act()
    {
        if (Unstack(this.mobility + currentField.mobility))
        {
            Movement();
        }
        else
        {
            Pin();
        }
    }


    private int RateField(TerrainTile tile)     // занятый тайл оценивает нулем!!
    {
        int Rate = 1;

        if (tile.currentUnit != null)
        {
            Rate = 0;
        }
        else if (tile.currentStructure != null)
        {
            Rate = 20;
        }
        else
        {
            Rate += tile.protection*2 + mobility;
        }
        
        return Rate;
    }


    void Start()
    {
        SetTag("Unit");

        StartCoroutine("delayedStart");

        NPCsManager.attackTeam.Add(this);
    }

    IEnumerator delayedStart()
    {
        yield return new WaitForSeconds(0.1f);
        Attach();
    }

    public void takeDamage(int dmg)
    {
        // Debug.Log("hit");

        if (dmg > this.armor)
        {
            this.health -= dmg - armor;
        }

        Instantiate(Effect, transform.position, Quaternion.identity);

        // TODO бронебойность, и эффект для непробил
        if (health <= 0)
        {
            Death();
        }
    }

    public override void Death()
    {
        NPCsManager.attackTeam.Remove(this);
        base.Death();
    }

    private void OnMouseDown()
    {
        Movement();
    }

    public override void NpcAct()
    {
        Movement();
    }

    
}
