using Godot;
using System;

/**
Absztrakt ősosztály minden harci karakterhez.
A konkrét harcos osztályok (Fighter, Archer, Assassin, Mage) ebből öröklődnek.
**/
public abstract partial class ActorBase : Node2D
{
	// harcos osztály neve (Fighter, Archer, Assassin, Mage)
	public string ClassName { get; protected set; }

	// Életerő adatok maximum és a jelenlegi életerő.
	public int MaxHP { get; protected set; }
	public int CurrentHP { get; protected set; }

	// Alap harci statok
	public int AttackDamage { get; protected set; }
	public int Defense { get; protected set; }
	public float DodgeChance { get; protected set; }

	// Támadás hatótáv alap értékkel (1 = közelharc)
	public int AttackRange { get; protected set; } = 1;

	// Különleges képesség (ability) kör alapú visszatöltése
	public int AbilityCooldownMax { get; protected set; } = 2;
	public int AbilityCooldown { get; private set; } = 0;

	// izometrikus Rács pozíció.
	public Vector2I GridPos { get; set; }

	// Izometrikus konverzióhoz használt rács méret.
	public float TileSize = 32;

	// Hivatkozás a turn managerre (globális elérés)
	public static TurnManager TM;

	public override void _Ready()
	{
		// A node pozíciójának frissítése a rácspozíció alapján.
		Position = IsoToScreen(GridPos);
	}

	//Kör végén csökkenti a cooldown értékét, ha éppen aktív.
	public void TickCooldown()
	{
		if (AbilityCooldown > 0)
			AbilityCooldown--;
	}

	//Különleges támadás elhasználása a cooldown indítása.
	public void TriggerCooldown()
	{
		AbilityCooldown = AbilityCooldownMax;
	}

	//Rácskoordináta átalakítása izometrikus képernyő pozícióvá.
	public Vector2 IsoToScreen(Vector2I g)
	{
		float size = TileSize;
		return new Vector2(
			(g.X - g.Y) * (size / 2),
			(g.X + g.Y) * (size / 4)
		);
	}

	//Sebzödés, életerő csökkentése, halál ellenőrzése (játék vége).
	public void ReceiveDamage(int dmg)
	{
		CurrentHP -= dmg;
		if (CurrentHP < 0) CurrentHP = 0;

		GD.Print($"{ClassName} HP → {CurrentHP}/{MaxHP}");

		// Ha a karakter meghal, turn manager értesítése
		if (IsDead() && ActorBase.TM != null)
		{
			ActorBase.TM.UpdateGameEnd(this is Enemy ? "Player" : "Enemy");
		}
	}

	//Visszaadja, hogy a karakter életereje elfogyott-e.
	public bool IsDead() => CurrentHP <= 0;

	
	//Másik kasztból átmásolja a statisztikákat. Ezt használjuk a random kaszt kiosztásakor.
	public void CopyStatsFrom(ActorBase src)
	{
		ClassName = src.ClassName;
		MaxHP = src.MaxHP;
		CurrentHP = src.CurrentHP;
		AttackDamage = src.AttackDamage;
		Defense = src.Defense;
		DodgeChance = src.DodgeChance;
		AttackRange = src.AttackRange;
		AbilityCooldownMax = src.AbilityCooldownMax;
	}
}
