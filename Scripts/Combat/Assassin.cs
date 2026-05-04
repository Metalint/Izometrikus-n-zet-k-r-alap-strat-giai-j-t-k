using System;
public partial class Assassin : ActorBase
{
    private Random rand = new();

    public Assassin()
    {
        ClassName = "Assassin";
        MaxHP = 80;
        CurrentHP = 80;
        AttackDamage = 15;
        Defense = 3;
        DodgeChance = 0.25f;

        AbilityCooldownMax = 1;
        AttackRange = 1;    //Támadási távolság
    }

    public int CritAttack()
    {
        bool crit = rand.NextDouble() < 0.35;
        return crit ? AttackDamage * 3 : AttackDamage;
    }
}
