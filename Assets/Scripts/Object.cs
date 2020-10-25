using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Object : MonoBehaviour
{
    [HideInInspector]
    public TerrainTile currentField;

    protected Vector3 up = new Vector3(0, 0.5f);  // TODO подкоректировать под перспективу (сейчас перспектива это деление на два )
    protected Vector3 upleft = new Vector3(-0.75f, 0.25f);
    protected Vector3 upright = new Vector3(0.75f, 0.25f);

    protected void SetTag(string tag)
    {
        gameObject.tag = tag;
    }



    private void Update()
    {

    }

    protected TerrainTile GetTerrain(Vector3 check_pos) // обратиться к тайлу поля на заданой коорденате
    {
        check_pos.z = 1.5f;

        Ray ray = new Ray(check_pos, Vector3.back);

        Physics.Raycast(ray, out RaycastHit hit, 1);


        Debug.DrawLine(ray.origin, ray.origin + ray.direction, Color.red, 2f); // луч
        
        if (hit.collider.gameObject.CompareTag("Terrain"))
        {
            return hit.collider.gameObject.GetComponent<TerrainTile>();
        }
        else
        {
            return null;
        }
        
    }


    public virtual void NpcAct()
    {
        // TODO
    }

    public virtual void Death()
    {
        Debug.Log(this + "dead");
        Destroy(gameObject);
    }


    public void Attach(TerrainTile tile = null)
    {
        if (tile == null)
        {
            tile = GetTerrain(gameObject.transform.position);
        }
        
        currentField = tile;

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

    public void Detach()
    {
        if (this.CompareTag("Unit"))
        {
            currentField.currentUnit = null;
        }
        else if (this.CompareTag("Structure"))
        {
            currentField.currentStructure = null;
        }

        currentField = null;
    }


    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            collision.gameObject.GetComponent<TerrainTile>().currentObject = this;
        }

    }
    */

}
