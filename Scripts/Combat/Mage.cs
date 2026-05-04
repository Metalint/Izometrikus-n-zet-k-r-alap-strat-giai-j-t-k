public partial class Mage : ActorBase
{
	public Mage()
	{
		ClassName = "Mage";
		MaxHP = 70;
		CurrentHP = 70;
		AttackDamage = 15;
		Defense = 2;
		DodgeChance = 0.08f;

		AttackRange = 5;	//Támadási távolság
		AbilityCooldownMax = 2;
	}

	public int Fireball()
	{
		return AttackDamage + 25;
	}
}
