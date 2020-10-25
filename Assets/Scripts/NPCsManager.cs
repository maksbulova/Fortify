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


    public IEnumerator ActAll(bool def)
    {
        if (def)
        {
            /*
            foreach(Object obj in defTeam)
            {
                obj.NpcAct();
            }
            */

            for (int i = 0; i < defTeam.Count; i++)
            {
                defTeam[i].NpcAct();
                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            for (int i = 0; i < attackTeam.Count; i++)
            {
                attackTeam[i].NpcAct();
                yield return new WaitForSeconds(0.1f);
            }
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("В атаку!");
            StartCoroutine(ActAll(false));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Держать строй!");
            StartCoroutine(ActAll(true));
        }
    }
}
