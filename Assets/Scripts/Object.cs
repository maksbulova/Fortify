using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Object : MonoBehaviour
{
    public TerrainTile currentTile;


    protected void SetTag(string tag)
    {
        gameObject.tag = tag;
    }



    public virtual IEnumerator delayedStart()  // надо подождать пока создастся земля, чтоб было к чему обращаться
    {
        yield return new WaitForSeconds(0.1f);
        Attach();
    }

    public abstract IEnumerator NpcAct();

    public abstract void takeDamage(int dmg);


    protected abstract void Death();


    public void Attach(TerrainTile tile = null)     // привязать переданный тайл к текущему объекту, а к тайлу объект
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

    public void Detach()    // отвязать тайл от этого объекта
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

}
