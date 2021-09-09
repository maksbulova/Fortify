using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystem : MonoBehaviour
{
    private NPCsManager npcs;
    [SerializeField] private Button nextTurnButton;


    private enum TurnPhases
    {
        defShoot,
        defBuild,
        attackMove,
        attackSpawn
    }

    private static TurnPhases turnPhase;

    private void Start()
    {
        npcs = gameObject.GetComponent<NPCsManager>();

        turnPhase = TurnPhases.defShoot;
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
                case TurnPhases.defShoot:
                    // стрельба траншей

                    yield return StartCoroutine(npcs.ActAll(def: true));

                    turnPhase = TurnPhases.defBuild;
                    repeat = true;
                    break;

                case TurnPhases.defBuild:

                    // игрок строит
                    nextTurnButton.interactable = true; // отключение кнопки лежит в самой кнопке

                    turnPhase = TurnPhases.attackMove;
                    repeat = false;  // ждать нажатия кнопки
                    break;

                case TurnPhases.attackMove:
                    // движение юнитов

                    yield return StartCoroutine(npcs.ActAll(def: false));

                    turnPhase = TurnPhases.attackSpawn;
                    repeat = true;
                    break;

                case TurnPhases.attackSpawn:

                    // спаун юнитов TODO
                    turnPhase = TurnPhases.defShoot;
                    repeat = true;
                    break;

                default:
                    break;
            }

        } while (repeat);





    }

}
