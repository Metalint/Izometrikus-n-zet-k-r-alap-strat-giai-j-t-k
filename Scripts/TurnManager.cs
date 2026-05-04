using System;
using Godot;
using System.Threading.Tasks;

/**
Itt kezeljük a körváltás és a felek tevékenységét a körjük alatt, meg a felhasználói felület frissítését és inicializálását is itt kezeljük. 
**/
public partial class TurnManager : Node
{
	//Jelzi, épp a játékos köre van-e
	private bool playerTurn = true;
	//A játék véget ért-e
	private bool gameEnded = false;

	//Hivatkozás a Player és Enemy node-okra
	private Player player;
	private Enemy enemy;
	
	//A UI elemek (pl. hp, kör jelzés, game over)
	private Label playerHPLabel;
	private Label enemyHPLabel;
	private Label turnLabel;
	private Label gameOverLabel;
	private RichTextLabel combatLog;

	//Inicializálás a jelenet betöltésekor
	public override void _Ready()
	{
		player = GetNode<Player>("../Player");
		enemy = GetNode<Enemy>("../Enemy");
		turnLabel = GetNode<Label>("../CanvasLayer/TurnLabel");
		playerHPLabel = GetNode<Label>("../CanvasLayer/PlayerHpLabel");
		enemyHPLabel = GetNode<Label>("../CanvasLayer/EnemyHpLabel");
		gameOverLabel = GetNode<Label>("../CanvasLayer/GameOverLabel");
		combatLog = GetNode<RichTextLabel>("../CanvasLayer/CombatLog");
		
		//Kezdeti UI frissítés
		UpdateTurnLabel();
		UpdateUI();
		
		//TurnManager referencia eltárolása globálisan (ActorBase számára)
		ActorBase.TM = this;
	}

	//A Godot minden frame-ben meghívja (folyamatos futás)
	public override void _Process(double delta)
	{
		//Csak akkor fut le amikor a játoks köre van.
		if (playerTurn)
		{
			//Ha a játékos lenyomja Entert, befejezi a körét
			if (Input.IsActionJustPressed("ui_accept"))
			{
				GD.Print("Játékos befejezte a körét.");

				 PlayerAttackPhase();

				//Játékos befejezi a körét és az enemy köre jön.
				playerTurn = false;
				UpdateTurnLabel();

				//Az enemy köre jön.
				EnemyTurn();
			}	
		}
	}

	//Játékos támadási fázisa sima támadás és képesség.
	private void PlayerAttackPhase()
	{
	//Csak akkor támadjon, ha range-ben van
		if (CombatController.InRange(player, enemy))
		{
		//Sima támadás
			CombatController.Attack(player, enemy);
			string log = CombatController.Attack(player.Class, enemy.Class);
			AddLog(log);

		//Ability használata ha a class támogatja
		if (player.Class is Archer archer)
			CombatController.AbilityAttack(archer, enemy, archer.RangedShot());

		if (player.Class is Mage mage)
			CombatController.AbilityAttack(mage, enemy, mage.Fireball());

		if (player.Class is Assassin assassin)
			CombatController.AbilityAttack(assassin, enemy, assassin.CritAttack());
		}
	//UI frissítése
	UpdateUI();

	//Ability cooldown tickelődik minden kör végén
	player.TickCooldown();
	}

	//A combat log szöveg hozzáadása a richtext labelhez.
	public void AddLog(string text)
	{
	combatLog.AppendText(text + "\n");
	combatLog.ScrollToLine(combatLog.GetLineCount() - 1); // auto scroll hogy magától menjen a combat log és a legfrisseb interakciót lássák.
	}

	//Az ellenség köre mozgás és támadás (async mert tween animáció van használva)
	private async Task EnemyTurn()
	{
		GD.Print("Ellenség lép...");
		
		//Többször lép egymás után, így közelít a playerhez
		for (int i = 0; i < 5; i++)
		{
			await enemy.MakeMove(player);
		}
		
		//enemy támad
		EnemyAttackPhase();

		UpdateUI();
		enemy.TickCooldown();

		//1 másodperc várakozás az ellenség mozgása után, kis késleltetés animáció miatt
		GetTree().CreateTimer(1.0).Timeout += () =>
		{
			playerTurn = true;
			UpdateTurnLabel();
			GD.Print("Vissza a játékoshoz.");
		};
	}
	
	//Az ellenség támadása (normál + ability class szerint)
	private void EnemyAttackPhase()
	{
		if (CombatController.InRange(enemy, player))
		{
			CombatController.Attack(enemy, player);
			string log = CombatController.Attack(enemy.Class, player.Class);
			AddLog(log);

			if (enemy.Class is Archer archer)
				CombatController.AbilityAttack(archer, player, archer.RangedShot());

			if (enemy.Class is Mage mage)
				CombatController.AbilityAttack(mage, player, mage.Fireball());

			if (enemy.Class is Assassin assassin)
				CombatController.AbilityAttack(assassin, player, assassin.CritAttack());
		}
	}
	
	//UI frisítése a sebződés alapján
	public void UpdateUI()
	{
		playerHPLabel.Text = $"Player HP: {player.CurrentHP}/{player.MaxHP}";
		enemyHPLabel.Text = $"Enemy HP: {enemy.CurrentHP}/{enemy.MaxHP}";
	}

	//A kör jelzése a játék felületén.
	private void UpdateTurnLabel()
	{
		turnLabel.Text = playerTurn ? "Játékos köre" : "Ellenség köre";
	}
	
	//A játék vége (győztes megjelenítése + kilépés 3 mp múlva)
	public void UpdateGameEnd(string winner)
	{
		if (gameEnded) return; // ne fusson le kétszer
			gameEnded = true;

   		gameOverLabel.Visible = true;
		gameOverLabel.Text = $"{winner} Wins! Game closing in 3s...";

		GD.Print($"{winner} Wins! Game closing in 3s...");
	
		GetTree().CreateTimer(3.0).Timeout += () =>
		{
			GD.Print("Exiting Game");
			GetTree().Quit();	
		};
	}
	
	//Memgmondja hogy a játékos (player) köre van-e.
	public bool IsPlayerTurn()
	{
		return playerTurn;
	}
}
