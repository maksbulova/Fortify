﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static General;

public abstract class Object : MonoBehaviour, IDamageable
{
    protected TerrainTile currentTile;

    [Header("Параметры юнита")]
    [Tooltip("здоровье"), SerializeField, Range(1, 10)]
    protected int health;
    [Tooltip("количество выстрелов за ход"), SerializeField, Range(1, 10)]
    protected int fireRate; // TODO приравнять к HP
    [Tooltip("чем выше тем сложнее пробить"), SerializeField, Range(0, 10)]
    protected int armor;
    [Tooltip("чем выше тем сложнее попасть"), SerializeField, Range(-10, 10)]
    protected int cover;
    [Tooltip("чем выше тем вероятнее выбраться"), SerializeField, Range(-10, 10)]
    protected int mobility;
    [Tooltip("урон за каждого соладта (хп) в отряде"), SerializeField, Range(1, 5)]
    protected float meleeDmg;
    [Tooltip("Урон наносимый за 1 попадание (1 пуля = 1 труп) "), SerializeField, Range(1, 10)]
    protected int shotDamage;
    protected int armorPiercing;
    [Tooltip("Минимальная и максимальная дальность стрельбы (включительно)")]
    [Range(1, 10)] protected int maxRange;
    [Range(1, 10)] protected int minRange = 1;
    //Если подавление 0.5, то противник будет прижат только при одновременном огне с двух позиций
    [Tooltip("Эффективность подавления"), SerializeField, Range(0, 5)]
    protected float suppression;
    [Tooltip("Влияет противоположно укрытию"), SerializeField, Range(-3, 3)]
    protected int accuracy;


    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
        }
    }

    public int Cover
    {
        get
        {
            return this.cover + currentTile.cover;
        }
    }

    protected void SetTag(string tag)
    {
        gameObject.tag = tag;
    }
    public float MeleeDamage
    {
        get
        {
            return health * meleeDmg;
        }
    }



    public virtual IEnumerator delayedStart()  // надо подождать пока создастся земля, чтоб было к чему обращаться
    {
        yield return new WaitForSeconds(0.1f);

        AttachTerrainTile();
    }

    public abstract IEnumerator NpcAct();


    protected abstract void Death();


    // привязать переданный тайл к текущему объекту, а к тайлу объект
    public void AttachTerrainTile(TerrainTile tile = null)
    {
        tile = tile ?? GetTerrain(gameObject.transform.position);

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

    public void TakeHit(DamageType damageType, float damageAmount, float accuracy, float piercing)
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

    public void TakeDamage(DamageType damageType, float damageAmount)
    {
        health -= Mathf.FloorToInt(damageAmount);

        if (DeathCheck())
        {
            Death();
        }

    }


    protected bool DeathCheck()
    {
        if (health <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
