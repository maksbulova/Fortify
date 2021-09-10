using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static General;

public interface IDamageable
{
    void TakeHit(DamageType damageType, float damageAmount, float accuracy, float piercing);
    void TakeDamage(DamageType damageType, float damageAmount);
}
