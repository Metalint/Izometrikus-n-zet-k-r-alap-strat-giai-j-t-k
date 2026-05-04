public partial class Fighter : ActorBase
{
	public Fighter()
	{
		ClassName = "Fighter";
		MaxHP = 150;
		CurrentHP = 150;
		AttackDamage = 20;
		Defense = 10;
		DodgeChance = 0.05f;

		AbilityCooldownMax = 2;
		AttackRange = 1;  //Támadási távolság
	}

	public int ShieldBlock()
	{
		return Defense * 2;
	}
}
