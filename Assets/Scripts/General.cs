using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class General
{
    public static Vector3 up = new Vector3(0, 0.5f);  // TODO подкоректировать под перспективу (сейчас перспектива это деление на два )
    public static Vector3 upleft = new Vector3(-0.75f, 0.25f);
    public static Vector3 upright = new Vector3(0.75f, 0.25f);
    public static Vector3 down = new Vector3(0, -0.5f);
    public static Vector3 downleft = new Vector3(-0.75f, -0.25f);
    public static Vector3 downright = new Vector3(0.75f, -0.25f);

    public static List<Vector3> allDirections = new List<Vector3>() { up, upleft, upright, down, downleft, downright };


    public static bool DiceCheck(int modifier = 0, int basic = 5, int max = 10)
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

        if (Random.Range(1, max + 1) <= basic)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    public enum Armor
    {
        // TODO добавь промежуточные значения
        none = 0,       // открытые позиции и обычные солдаты
        wood = 2,       // полевые укрепления
        steel = 4,      // бронированная техника
        concrete = 6    // долговечные капитальные укрепления
    }

    public enum DamageType
    {
        shrapnel,
        highExplosive,
        armorePiercing
    }

    public enum Experience
    {
        recruit = 1,    // фермер с винтовкой
        regular = 2,    // прошедшие строевую подготовку или пережившие пару боев
        veteran = 3     // месяцы и годы боев 
    }

    public static TerrainTile GetTerrain(Vector3 check_pos) // обратиться к тайлу поля на заданой коорденате
    {

        check_pos.z = 1.5f;

        Ray ray = new Ray(check_pos, Vector3.back);

        Physics.Raycast(ray, out RaycastHit hit, 1);


        // Debug.DrawLine(ray.origin, ray.origin + ray.direction, Color.red, 2f); // луч

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Terrain"))
        {
            return hit.collider.gameObject.GetComponent<TerrainTile>();
        }
        else
        {
            return null;
        }

    }


}
