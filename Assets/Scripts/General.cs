using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class General
{
    // TODO подкоректировать под перспективу (сейчас перспектива это деление на два )
    public static Vector3 up = Vector3.up * 0.5f;
    public static Vector3 upLeft = Vector3.up * 0.25f + Vector3.left * 0.75f;
    public static Vector3 upRight = Vector3.up * 0.25f + Vector3.right * 0.75f;
    public static Vector3 down = Vector3.down * 0.5f;
    public static Vector3 downLeft = Vector3.down * 0.25f + Vector3.left * 0.75f;
    public static Vector3 downRight = Vector3.down * 0.25f + Vector3.right * 0.75f;

    public static List<Vector3> allDirections = new List<Vector3>() { up, upLeft, upRight, down, downLeft, downRight };


    public static bool DiceCheck(float basic = 5, float modifier = 0, float max = 10)
    // бросок 1 кубика dmax, чем выше mod тем вероятнее успех, basic - базовый шанс
    {

        // TODO попробуй оптимизировать чтоли

        if (basic - modifier <= 0) // бросок будет >100%
        {
            basic = 1; // максимальный шанс 90%
        }
        else if (basic - modifier >= max) // бросок будет <0%
        {
            basic = max - 1; // минимальный шанс 10% (минимальное значение кубика 1 а не 0)
        }
        else
        {
            basic -= modifier;
        }

        float diceResult = Random.Range(1, max);

        if (diceResult >= basic)
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
        // kinetic includes bullets, shrapnel, armore piercing shells
        kinetic,
        // highExplosive
        shockWave,
        melee
    }

    public enum Experience
    {
        recruit = 1,    // фермер с винтовкой
        regular = 2,    // прошедшие строевую подготовку или пережившие пару боев
        veteran = 3     // месяцы и годы боев 
    }


    // обратиться к тайлу поля на заданой коорденате
    public static TerrainTile GetTerrain(Vector3 tilePos) 
    {
        // TODO round to cell size to prevent null expection
        tilePos.z = 1.5f;

        Ray ray = new Ray(tilePos, Vector3.back);

        Physics.Raycast(ray, out RaycastHit hit, 1);


        // Debug.DrawLine(ray.origin, ray.origin + ray.direction, Color.red, 2f);

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
