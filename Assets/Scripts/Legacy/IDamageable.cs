using static Legacy.General;

namespace Legacy
{
    public interface IDamageable
    {
        void TakeHit(DamageType damageType, float damageAmount, float accuracy, float piercing, float suppression);
        void TakeDamage(DamageType damageType, float damageAmount);
    }
}
