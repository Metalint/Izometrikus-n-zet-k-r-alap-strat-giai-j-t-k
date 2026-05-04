using Godot;
using System;

public partial class MainMenu : Control
{
	   public override void _Ready()
	{
		var startButton = GetNode<Button>("CenterContainer/VBoxContainer/StartButton");
		var optionsButton = GetNode<Button>("CenterContainer/VBoxContainer/OptionsButton");
		var exitButton = GetNode<Button>("CenterContainer/VBoxContainer/ExitButton");

		startButton.Pressed += OnStartPressed;
		optionsButton.Pressed += OnOptionsPressed;
		exitButton.Pressed += OnExitPressed;
	}
	
	private void OnStartPressed()
	{
		GD.Print("Start Game");
		GetTree().ChangeSceneToFile("res://Scenes/Main.tscn");
	}

	private void OnOptionsPressed()
	{
		GD.Print("Options Clicked");
		GetTree().ChangeSceneToFile("res://Scenes/options.tscn");
	}
	
	private void OnExitPressed()
	{
		GD.Print("Exiting Game");
		GetTree().Quit();
	}
	
}
