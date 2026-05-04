public partial class Archer : ActorBase
{
    public Archer()
    {
        ClassName = "Archer";
        MaxHP = 100;
        CurrentHP = 100;
        AttackDamage = 18;
        Defense = 5;
        DodgeChance = 0.1f;

        AttackRange = 4;        //Támadási távolság
        AbilityCooldownMax = 2;
    }

    public int RangedShot()
    {
        return AttackDamage + 10;
    }
}
