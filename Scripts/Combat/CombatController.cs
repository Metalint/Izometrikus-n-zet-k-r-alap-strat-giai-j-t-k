using Godot;
using System;

/**
itt vannak a támadások és egyéb képességek használatának sebzésének kiszámitása.
**/
public static class CombatController
{
	//Random generátor a kritikus és dodge esélyekhez.
	private static Random rng = new();

 	//Ellenőrzi, hogy két szereplő támadási távolságon belül van-e
	public static bool InRange(ActorBase a, ActorBase b)
	{
		int dx = Math.Abs(a.GridPos.X - b.GridPos.X);
		int dy = Math.Abs(a.GridPos.Y - b.GridPos.Y);

		//diagonális távolságot használunk így átlósan is lehet támadni
		return Math.Max(dx, dy) <= a.AttackRange;
	}

	
	//Sima alap támadás (minden karakter tudja használni)
	public static string Attack(ActorBase attacker, ActorBase defender)
	{

		//Ha támadási távolságon kivülvan, akkor nem sebzünk
		if (!InRange(attacker, defender))
		{
			return $"{attacker.ClassName} is out of range!";
		}

		//Dodge esélye, ha sikerül a támadás elkerülve
		if (rng.NextDouble() < defender.DodgeChance)
		{
			return $"{defender.ClassName} dodged!";
		}

		//Alap sebzés kiszámítása (védelem levonása)
		int rawDamage = attacker.AttackDamage;
		int finalDamage = Math.Max(rawDamage - defender.Defense, 0);

		//A tényleges sebzés dogde és defense után.
		defender.ReceiveDamage(finalDamage);

		return $"{attacker.ClassName} dealt {finalDamage} damage to {defender.ClassName}";
	}

	//Különleges támadások (pl. Fireball, CritAttack, RangedShot)
	public static void AbilityAttack(ActorBase attacker, ActorBase defender, int abilityDamage)
	{

		//Ha cooldown alatt van az ability nem használható
		if (attacker.AbilityCooldown > 0)
		{
			GD.Print($"{attacker.ClassName} ability on cooldown: {attacker.AbilityCooldown} turns left");
			return;
		}

		//Ellenőrzi hogy elég közel van e a képesség használásához.
		if (!InRange(attacker, defender))
		{
			GD.Print($"{attacker.ClassName}'s ability is out of range!");
			return;
		}

		//A különleges képességet is ki lehet dogeolni.
		if (rng.NextDouble() < defender.DodgeChance)
		{
			GD.Print($"{defender.ClassName} dodged the ability!");
			return;
		}

		//A sebzésből a védekezés levonása.
		int finalDamage = Math.Max(abilityDamage - defender.Defense, 0);

		//A sebzés
		defender.ReceiveDamage(finalDamage);

		//A képesség cooldownra rakása
		attacker.TriggerCooldown();

		GD.Print($"{attacker.ClassName} used ability for {finalDamage} damage!");
	}
}
