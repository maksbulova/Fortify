using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile : MonoBehaviour, IDamageable
{
    [Tooltip("чем выше тем сложнее попасть")]
    public int cover;
    [Tooltip("чем выше тем легче выбраться")]
    public int mobility;
    
    public TerrainTile[] changedTiles;

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

    public void TakeDamage(General.DamageType damageType, float damageAmount)
    {
        throw new System.NotImplementedException();
    }
}
