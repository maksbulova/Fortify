﻿using UnityEngine;
using static Legacy.General;

namespace Legacy
{
    public class TerrainTile : MonoBehaviour, IDamageable
    {
        [Tooltip("чем выше тем сложнее попасть")]
        public int cover;
        [Tooltip("чем выше тем легче выбраться")]
        public int mobility;
        [SerializeField]
        private TerrainTile[] shockWaveChangedTiles;

        [Space]
        [HideInInspector]
        public Unit currentUnit;
        [HideInInspector]
        public Structure currentStructure;


        private void Start()
        {
            gameObject.tag = "Terrain";
        }


        public void TakeHit(DamageType damageType, float damageAmount, float accuracy, float piercing, float suppression)
        {
            if (currentUnit != null)
            {
                currentUnit.TakeHit(damageType, damageAmount, accuracy: 0, piercing, suppression);
            }
            else if (currentStructure != null)
            {
                currentStructure.TakeHit(damageType, damageAmount, accuracy: 0, piercing, suppression);
            }

            TakeDamage(damageType, damageAmount);
        }

        public void TakeDamage(DamageType damageType, float damageAmount)
        {
            if (damageType == DamageType.shockWave)
            {
                // TODO проверить пересоздает ли смена тайла объект на этом тайле
            }
        }
    }
}
