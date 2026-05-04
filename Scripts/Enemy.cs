using Godot;
using System;
using System.Threading.Tasks;


/**
A Enemy az AI által irányított karakter.
Az ActorBase-ből öröklődik, ezért rendelkezik alap statokkal (HP, támadás stb.).
**/
public partial class Enemy : ActorBase
{
	private Vector2I gridPosition = new Vector2I(70, 0);

	//védekezési állapotok (blokkolás, kitérés) a harcroláshoz.
	public bool IsGuarding { get; set; } = false;
	public bool IsDodging { get; set; } = false;

	public override void _Ready()
	{
		//A játék indításakor véletlen osztályt ad a játékosnak. (Fighter / Archer / Assassin / Mage)
		AssignRandomClass();
		
		//Kezdeti pozíció beállítása a pályán (képernyő koordinátává konvertálva)
		GridPos = gridPosition;
		Position = IsoToScreen(gridPosition);
	}	

	//A statokat hordozó objektum (pl. Archer, Mage, stb.)
	public ActorBase Class { get; private set; }

	//Véletlen class generálása a játékosnak itt kapja meg a statjait a játákos(pl. MaxHp, CurrentHp, stb.)
	public void AssignRandomClass()
	{
		Class = ActorFactory.CreateRandom();
		CopyStatsFrom(Class);
		GD.Print($"Enemy class → {Class.ClassName}");
	}

	//Enemy AI mozgása távlolság alapján, távolság tartása class alapján
	public async Task MakeMove(Player player)
{
	Vector2I playerPos = player.GetGridPosition();

	int distX = Math.Abs(playerPos.X - gridPosition.X);
	int distY = Math.Abs(playerPos.Y - gridPosition.Y);
	int dist = Math.Max(distX, distY); // 8 irányos távolság

	// AI viselkedéshez minimális és maximális optimális távolság
	int optimalMin = 1;
	int optimalMax = AttackRange;

	// ARCHER → maradjon távol
	if (Class is Archer)
	{
		optimalMin = 3;
		optimalMax = 4;
	}

	// MAGE → közepes távolság
	if (Class is Mage)
	{
		optimalMin = 2;
		optimalMax = 3;
	}

	// Assassin / Fighter → közelharc
	if (Class is Assassin || Class is Fighter)
	{
		optimalMin = 1;
		optimalMax = 1;
	}

	// Ha túl közel van a játkoshoz távolódjon.
	if (dist < optimalMin)
	{
		Vector2I awayDir = new Vector2I(
			gridPosition.X - playerPos.X > 0 ? 1 : -1,
			gridPosition.Y - playerPos.Y > 0 ? 1 : -1
		);

		await MoveTo(gridPosition + awayDir);
		return;
	}

	// Ha túl messze van közeledjen.
	if (dist > optimalMax)
	{
		Vector2I dir = new Vector2I(
			playerPos.X > gridPosition.X ? 1 : -1,
			playerPos.Y > gridPosition.Y ? 1 : -1
		);

		await MoveTo(gridPosition + dir);
		return;
	}

	// Optimális rangen van ne mozogjon.
	return;
}
	//Enemy mozgása animálása a játékos felé.
	private async Task MoveTo(Vector2I newPos)
	{
	gridPosition = newPos;
	GridPos = newPos;

	Vector2 targetPos = IsoToScreen(newPos);

	//maga az animáció itt történik.
	var movement = GetTree().CreateTween();
	movement.TweenProperty(this, "position", targetPos, 0.15f);

	await ToSignal(movement, "finished");
	}

	//Kör végén reseteljük a védelmi flag-eket, különben a játékos örökre dodgolna
	public void ResetFlags()
	{
		IsGuarding = false;
		IsDodging = false;
	}
}
