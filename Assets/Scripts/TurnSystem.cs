using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
    [SerializeField] private NPCsManager npcsManager;
    [SerializeField] private Button nextTurnButton;

    [SerializeField] private UnitSpawner unitSpawner;

    private enum TurnPhases
    {
        defNPC,
        defPlayer,
        attackNPC,
        attackSpawn
    }

    private static TurnPhases turnPhase;

    private void Start()
    {
        turnPhase = TurnPhases.defNPC;
        StartCoroutine(GameCycle());
    }


    public void NextTurnButton() // не смог запустить корутин из кнопки
    {
        StartCoroutine(GameCycle());
    }

    private IEnumerator GameCycle()  // игровой цикл: транше стреляют -> игрок строит -> юниты двигаются -> юниты спаунятся
    {
        bool repeat = false;

        do
        {
            switch (turnPhase)
            {
                case TurnPhases.defNPC:
                    // стрельба траншей

                    yield return StartCoroutine(npcsManager.ActAll(def: true));

                    turnPhase = TurnPhases.defPlayer;
                    repeat = true;
                    break;

                case TurnPhases.defPlayer:

                    // игрок строит
                    nextTurnButton.interactable = true; // отключение кнопки лежит в самой кнопке

                    turnPhase = TurnPhases.attackNPC;
                    repeat = false;  // ждать нажатия кнопки
                    break;

                case TurnPhases.attackNPC:
                    // движение юнитов

                    yield return StartCoroutine(npcsManager.ActAll(def: false));

                    turnPhase = TurnPhases.attackSpawn;
                    repeat = true;
                    break;

                case TurnPhases.attackSpawn:

                    unitSpawner.SpawnWave();

                    turnPhase = TurnPhases.defNPC;
                    repeat = true;
                    break;

                default:
                    break;
            }

        } while (repeat);


    }

}
