using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static General;

public abstract class Object : MonoBehaviour, IDamageable
{
    protected TerrainTile currentTile;

    [Header("Параметры юнита")]
    [SerializeField, Range(1, 10), Tooltip("здоровье")]
    protected int health;
    [SerializeField, Range(1, 10), Tooltip("выстрелов от каждого юнита (каждого хп)")]
    protected float fireRate;
    [SerializeField, Range(0, 10), Tooltip("чем выше тем сложнее пробить")]
    protected int armor;
    [SerializeField, Range(-10, 10), Tooltip("Укрытие и маскировка. Чем выше тем сложнее попасть")]
    protected int cover;
    [SerializeField, Range(-10, 10), Tooltip("чем выше тем вероятнее выбраться")]
    protected int mobility;
    [Tooltip("урон за каждого соладта (хп) в отряде"), SerializeField, Range(1, 5)]
    protected float meleeDmg;
    [SerializeField, Range(1, 10), Tooltip("Урон наносимый за 1 попадание (1 пуля = 1 труп) ")]
    protected int shotDamage;
    protected int armorPiercing;
    [Tooltip("Минимальная и максимальная дальность стрельбы (включительно)")]
    [Range(1, 10)] protected int maxRange;
    [Range(1, 10)] protected int minRange = 1;
    //Если подавление 0.5, то противник будет прижат только при одновременном огне с двух позиций
    [SerializeField, Range(0, 5), Tooltip("Эффективность подавления")]
    protected float suppressionEfficiency;
    [SerializeField, Range(-3, 3), Tooltip("Влияет противоположно укрытию")]
    protected int accuracy;

    private float suppression;

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
            return this.cover + currentTile.cover + Mathf.FloorToInt(suppression);
        }
    }

    protected int FireRate
    {
        get
        {
            return Mathf.FloorToInt(fireRate * Health);
        }
    }

    protected int Accuracy
    {
        get
        {
            return accuracy - Mathf.FloorToInt(suppression);
        }
    }

    protected int Mobility
    {
        get
        {
            return mobility - Mathf.FloorToInt(suppression);
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

<<<<<<< Updated upstream

    public virtual IEnumerator DelayedStart()  // надо подождать пока создастся земля, чтоб было к чему обращаться
    {
        yield return new WaitForSeconds(0.1f);

        AttachTerrainTile();
    }

=======
>>>>>>> Stashed changes
    public abstract IEnumerator NpcAct();


    protected abstract void Death();


    // привязать переданный тайл к текущему объекту, а к тайлу объект
    public void AttachTerrainTile(TerrainTile tile = null)
    {
        if (tile == null)
        {
            tile = GetTerrain(gameObject.transform.position);
        }

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

    public void TakeHit(DamageType damageType, float damageAmount, float accuracy, float piercing, float suppressionEfficiency)
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

        suppression += suppressionEfficiency;
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
