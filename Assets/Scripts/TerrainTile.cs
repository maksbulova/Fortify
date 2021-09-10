using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static General;

public class TerrainTile : MonoBehaviour, IDamageable
{
    [Tooltip("чем выше тем сложнее попасть")]
    public int cover;
    [Tooltip("чем выше тем легче выбраться")]
    public int mobility;
    [SerializeField]
    private TerrainTile[] shockWaveChangedTiles;

    [Space]
    [HideInInspector]
    public Unit currentUnit;
    [HideInInspector]
    public Structure currentStructure;


    private void Start()
    {
        gameObject.tag = "Terrain";
    }


    private void OnMouseDown()
    {
        //Debug.Log(object_here);
    }

    public void TakeHit(General.DamageType damageType, float damageAmount, float accuracy, float piercing)
    {
        if (currentUnit != null)
        {
            currentUnit.TakeHit(damageType, damageAmount, accuracy, piercing);
        }
        else if (currentStructure != null)
        {
            currentStructure.TakeHit(damageType, damageAmount, accuracy: 0, piercing);
        }

        TakeDamage(damageType, damageAmount);
    }

    public void TakeDamage(DamageType damageType, float damageAmount)
    {
        if (damageType == DamageType.shockWave)
        {
            // TODO проверить пересоздает ли смена тайла объект на этом тайле
        }
    }
}
