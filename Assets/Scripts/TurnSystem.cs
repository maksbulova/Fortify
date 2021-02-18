using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour
{
    NPCsManager npcs;

    private void Start()
    {
        npcs = gameObject.GetComponent<NPCsManager>();
    }

    public void StartGame()
    {

    }

    enum Phases
    {
        attackTurn,
        defenceTurn
    }
}
