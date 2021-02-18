using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TilemapsManager : MonoBehaviour
{
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]

    public static TilemapsManager instance = null;

    public static Tilemap TilemapStructure;
    public static Tilemap TilemapTerrain;
    public static Tilemap TilemapEffects;

    void Start()
    {
        // Теперь, проверяем существование экземпляра
        if (instance == null)
        { // Экземпляр менеджера был найден
            instance = this; // Задаем ссылку на экземпляр объекта
        }
        else if (instance == this)
        { // Экземпляр объекта уже существует на сцене
            Destroy(gameObject); // Удаляем объект
        }
        // Теперь нам нужно указать, чтобы объект не уничтожался
        // при переходе на другую сцену игры
        DontDestroyOnLoad(gameObject);

        // И запускаем собственно инициализатор
        InitializeManager();

        
    }

    private void InitializeManager()
    {
        TilemapStructure = GameObject.Find("Grid/structures").GetComponent<Tilemap>();
        TilemapTerrain = GameObject.Find("Grid/terrain").GetComponent<Tilemap>();
        TilemapEffects = GameObject.Find("Grid/effects").GetComponent<Tilemap>();
    }

}
