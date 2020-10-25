using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTile : MonoBehaviour
{
    [Tooltip("чем выше тем сложнее попасть")]
    public int protection;
    [Tooltip("чем выше тем легче выбраться")]
    public int mobility;

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
}
