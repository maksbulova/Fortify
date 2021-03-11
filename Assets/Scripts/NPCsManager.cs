using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCsManager : MonoBehaviour
{
    public static NPCsManager instance = null;

    public static List<Unit> attackTeam = new List<Unit>();
    public static List<Structure> defTeam = new List<Structure>();


    void Start()
    {
        if (instance == null)
        {
            instance = this; 
        }
        else if (instance == this)
        { 
            Destroy(gameObject); 
        }
        DontDestroyOnLoad(gameObject);

        InitializeManager();

    }

    private void InitializeManager()
    {
        
    }

    /*
    private IEnumerator ActAll<T>(List<T> team) where T : Object
    {
        foreach (T obj in team)
        {
            obj.NpcAct();
            yield return new WaitForSeconds(0.01f);
        }
    }
    */

    public IEnumerator ActAll<T>(List<T> team) where T : Object
    {
        foreach (T obj in team)
        {
            StartCoroutine(obj.NpcAct()); // каждый с задержкой в 0-1 сек
        }

        yield return new WaitForSeconds(1.0f);  // TODO привязать к скорости анимации
    }

    public IEnumerator ActAll(bool def)
    {
        if (def)
        {
            yield return StartCoroutine(ActAll(defTeam));
        }
        else
        {
            yield return StartCoroutine(ActAll(attackTeam));
        }
    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            // Debug.Log("Ход атаки");
            // StartCoroutine(ActAll(attackTeam));
            ActAll(attackTeam);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            // Debug.Log("Ход защиты");
            // StartCoroutine(ActAll(defTeam));
            ActAll(defTeam);
        }
    }
}
