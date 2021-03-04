using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class Object : MonoBehaviour
{
    [HideInInspector]
    public TerrainTile currentTile;

    protected static Vector3 up = new Vector3(0, 0.5f);  // TODO подкоректировать под перспективу (сейчас перспектива это деление на два )
    protected static Vector3 upleft = new Vector3(-0.75f, 0.25f);
    protected static Vector3 upright = new Vector3(0.75f, 0.25f);
    protected static Vector3 down = new Vector3(0, -0.5f);
    protected static Vector3 downleft = new Vector3(-0.75f, -0.25f);
    protected static Vector3 downright = new Vector3(0.75f, -0.25f);

    protected static List<Vector3> allDirections = new List<Vector3>() { up, upleft, upright, down, downleft, downright };

    public enum Armor
    {
        // TODO добавь промежуточные значения
        none = 0,       // открытые позиции и обычные солдаты
        wood = 2,       // полевые укрепления
        steel = 4,      // бронированная техника
        concrete = 6    // долговечные капитальные укрепления
    }

    public enum Experience
    {
        recruit = 1,    // фермер с винтовкой
        regular = 2,    // прошедшие строевую подготовку или пережившие пару боев
        veteran = 3     // месяцы и годы боев 
    }

    protected void SetTag(string tag)
    {
        gameObject.tag = tag;
    }

    protected bool Check(int modifier = 0, int basic=5, int max = 10)
    // бросок 1 кубика dmax, чем выше mod тем вероятнее успех, basic - базовый шанс
    {
        // TODO попробуй оптимизировать чтоли

        if (basic + modifier >= max) // бросок будет >100%
        {
            basic = max - 1; // максимальный шанс 90%
        }
        else if (basic - modifier <= 0) // бросок будет <0%
        {
            basic = 1; // минимальный шанс 10% (минимальное значение кубика 1 а не 0)
        }
        else
        {
            basic += modifier;
        }

        if (Random.Range(1, max+1) <= basic)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    protected static TerrainTile GetTerrain(Vector3 check_pos) // обратиться к тайлу поля на заданой коорденате
    {
        check_pos.z = 1.5f;

        Ray ray = new Ray(check_pos, Vector3.back);

        Physics.Raycast(ray, out RaycastHit hit, 1);


        // Debug.DrawLine(ray.origin, ray.origin + ray.direction, Color.red, 2f); // луч
        
        if (hit.collider.gameObject.CompareTag("Terrain"))
        {
            return hit.collider.gameObject.GetComponent<TerrainTile>();
        }
        else
        {
            return null;
        }
        
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
