using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static General;

public abstract class Object : MonoBehaviour, IDamageable
{
    public TerrainTile currentTile;

    public abstract int Cover { get; }

    protected void SetTag(string tag)
    {
        gameObject.tag = tag;
    }

    [Header("Параметры юнита")]
    [Tooltip("здоровье"), Range(1, 10)]
    public int health;
    [Tooltip("количество выстрелов за ход"), Range(1, 10)]
    public int rate;   // TODO приравнять к HP
    [Tooltip("чем выше тем сложнее пробить"), Range(0, 10)]
    public int armor;
    [Tooltip("чем выше тем сложнее попасть"), Range(-10, 10)]
    public int cover;
    [Tooltip("чем выше тем вероятнее выбраться"), Range(-10, 10)]
    public int mobility;
    [Tooltip("урон за каждого соладта (хп) в отряде"), Range(1, 5)]
    public float meleeDmg;
    [Tooltip("Урон наносимый за 1 попадание (1 пуля = 1 труп) "), Range(1, 10)]
    public int damage;
    [Tooltip("Минимальная и максимальная дальность стрельбы (включительно)")]
    [Range(1, 10)] public int maxRange;
    [Range(1, 10)] public int minRange = 1;
    //Если подавление 0.5, то противник будет прижат только при одновременном огне с двух позиций
    [Tooltip("Эффективность подавления"), Range(0, 5)]
    public float suppression;
    [Tooltip("Влияет противоположно укрытию"), Range(-3, 3)]
    public int accuracy;




    public virtual IEnumerator delayedStart()  // надо подождать пока создастся земля, чтоб было к чему обращаться
    {
        yield return new WaitForSeconds(0.1f);
        AttachTerrainTile();
    }

    public abstract IEnumerator NpcAct();


    protected abstract void Death();


    public void AttachTerrainTile(TerrainTile tile = null)     // привязать переданный тайл к текущему объекту, а к тайлу объект
    {
        /*
        if (tile == null)
        {
            tile = GetTerrain(gameObject.transform.position);
        }
        */
        
        tile = tile ?? General.GetTerrain(gameObject.transform.position);

        currentTile = tile;

        if (this.CompareTag("Unit"))
        {
            tile.currentUnit = this as Unit;
        }
        else if (this.CompareTag("Structure"))
        {
            tile.currentStructure = this as Structure;
        }


        // TODO для случая если тайла нет
    }

    public void DetachTerrainTile()    // отвязать тайл от этого объекта
    {
        if (this.CompareTag("Unit"))
        {
            currentTile.currentUnit = null;
        }
        else if (this.CompareTag("Structure"))
        {
            currentTile.currentStructure = null;
        }

        currentTile = null;
    }

    public virtual void TakeHit(DamageType damageType, float damageAmount, float accuracy, float piercing)
    {
        float hitModifiers = +accuracy -Cover;
        bool hitted = DiceCheck(modifier: hitModifiers);

        if (hitted)
        {
            float armorModifiers = +piercing -armor;
            bool penetrated = DiceCheck(modifier: armorModifiers);

            if (penetrated)
            {
                TakeDamage(damageType, damageAmount);
            }
        }

    }

    public abstract void TakeDamage(DamageType damageType, float damageAmount);
}
