using Godot;
using System;

/**
A Player a játékos által irányítható karakter.
Az ActorBase-ből öröklődik, ezért rendelkezik alap statokkal (HP, támadás stb.).
**/
public partial class Player : ActorBase
{
	//védekezési állapotok (blokkolás, kitérés) a harcroláshoz.
	public bool IsGuarding { get; set; } = false;
	public bool IsDodging { get; set; } = false;

	//A játékos izometrikus térbenlévő pozíció megadása.
	private Vector2I gridPosition = new Vector2I(40, 0);
	
	//Hivatkozás kör rendszer kezelő TurnManager-re.
	private TurnManager turnManager;

	public override void _Ready()
	{
		//A játék indításakor véletlen osztályt ad a játékosnak. (Fighter / Archer / Assassin / Mage)
		AssignRandomClass();

		//Kezdeti pozíció beállítása a pályán (képernyő koordinátáká átkonvertálva)
		Position = IsoToScreen(gridPosition);

		//TurnManager referenciájának lekérése
		turnManager = GetNode<TurnManager>("../TurnManager");
	}

	//A statokat hordozó objektum (pl. Archer, Mage, stb.)
	public ActorBase Class { get; private set; }

	//Véletlen class generálása a játékosnak itt kapja meg a statjait a játákos(pl. MaxHp, CurrentHp, stb.)
	public void AssignRandomClass()
	{
		Class = ActorFactory.CreateRandom();
		//a random kapot osztály statjainak lekérése az osztályából
		CopyStatsFrom(Class);

		//kiírjuk a consolba hogy milyen osztályt kapot
		GD.Print($"Player class → {Class.ClassName}");
	}

	//Játékos input kezelése csak a saját körében nyílakal
	public override void _Input(InputEvent @event)
	{
		if (!turnManager.IsPlayerTurn()) return;

		if (@event.IsActionPressed("ui_up")) Move(Vector2I.Up);
		if (@event.IsActionPressed("ui_down")) Move(Vector2I.Down);
		if (@event.IsActionPressed("ui_left")) Move(Vector2I.Left);
		if (@event.IsActionPressed("ui_right")) Move(Vector2I.Right);
	}

	//Grid pozíció módosítása és a sprite (a karakter modelje) vizuális frissítése
	private void Move(Vector2I dir)
	{
		gridPosition += dir; // magát a logikai rács helyét frissiti
		GridPos = gridPosition; // ez az actorbasehez kell hogy a ranget számolja támadáshoz
		Position = IsoToScreen(gridPosition); //magát a spritetot rakja odébb
	}
	//Jelenlegi grid pozíció lekérdezése harc/AI döntésekhez
	public Vector2I GetGridPosition() => gridPosition;

	//Kör végén reseteljük a védelmi flag-eket, különben a játékos örökre dodgolna
	public void ResetFlags()
	{
		IsGuarding = false;
		IsDodging = false;
	}
}
