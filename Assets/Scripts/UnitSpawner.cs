using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
<<<<<<< Updated upstream
    private Unit[] units;
    
=======
    private Unit unit;

    [SerializeField] private Transform UnitsContainer;
    [SerializeField] private Transform[] spawnPoints;
    private TerrainTile[] spawnTiles;
    

    private IEnumerator Start()
    {
        yield return null;

        spawnTiles = new TerrainTile[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnTiles[i] = GetTerrain(spawnPoints[i].position);
        }
    }

    [ContextMenu("Spawn wave")]
    public void SpawnWave()
    {
        SpawnWave(spawnTiles.Length);
    }

    public void SpawnWave(int spawnAmount)
    {
        List<TerrainTile> avaiableTiles = new List<TerrainTile>();

        foreach (TerrainTile tile in spawnTiles)
        {
            if (tile.currentUnit == null)
            {
                avaiableTiles.Add(tile);
            }
        }

        spawnAmount = Mathf.Min(spawnAmount, avaiableTiles.Count);

        for (int i = 0; i < spawnAmount; i++)
        {
            int rndTileIndex = Random.Range(0, avaiableTiles.Count);

            SpawnUnit(avaiableTiles[rndTileIndex], unit);
            avaiableTiles.RemoveAt(rndTileIndex);
        }
    }

    private Unit SpawnUnit(TerrainTile spawnTile, Unit unit)
    {
        return SpawnUnit(spawnTile.transform.position, unit);
    }

    private Unit SpawnUnit(Vector3 spawnPosition, Unit unit)
    {
        spawnPosition.z = unitZPos;

        return Instantiate(unit, spawnPosition, Quaternion.identity, UnitsContainer);
    }
>>>>>>> Stashed changes
}
