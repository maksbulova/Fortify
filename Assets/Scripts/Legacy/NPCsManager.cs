using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy
{
    public class NPCsManager : MonoBehaviour
    {
        private static List<Unit> attackTeam = new List<Unit>();
        private static List<Structure> defTeam = new List<Structure>();

        public IEnumerator ActAll<T>(List<T> team) where T : Object
        {
            foreach (T obj in team)
            {
                // каждый с задержкой в 0-1 сек
                StartCoroutine(obj.NpcAct()); 
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

        public static void JoinTeam<T>(T joiningObject)
        {
            if (typeof(T) == typeof(Unit))
            {
                attackTeam.Add(joiningObject as Unit);
            }
            else if(typeof(T) == typeof(Structure))
            {
                defTeam.Add(joiningObject as Structure);

            }
        }

        public static void LeaveTeam<T>(T leavingObject)
        {
            if (typeof(T) == typeof(Unit))
            {
                attackTeam.Remove(leavingObject as Unit);
            }
            else if (typeof(T) == typeof(Structure))
            {
                defTeam.Remove(leavingObject as Structure);
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
}
