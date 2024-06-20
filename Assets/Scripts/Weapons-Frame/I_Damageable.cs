public interface I_Damageable
{
    void TakeDamage(float amount);
    void Die();
    void Repair();
    bool CanRecieveDamage();
}
