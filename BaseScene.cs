using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;
using System;

public class BaseScene : Node2D
{
	Loader loader;

	LevelController currentLevel;

	string loadingAssetname;

	public override void _Ready()
	{
		DebugHelper.Print("Loading...");

		var pathToLevel = "res://GameWorld/LevelController.tscn";

		loader = this.GetChild<Loader>();

		loader.LoadingFinished += Loader_LoadingFinished;

		loader.LoadAsset(pathToLevel, AssetKeys.FirstLevel.ToString(), 10);
	}

	private void Loader_LoadingFinished(object sender, LoadingFinishedEventArgs e)
	{
		if (e.Error == Error.FileEof)
		{
			DebugHelper.Print($"Loading {e.AssetName} finished!");

			if (e.AssetName.Contains("Level"))
			{
				var scene = (PackedScene)e.Resource;

				var instance = (LevelController)scene.Instance();

				currentLevel = instance;

				loadingAssetname = e.AssetName;

				loader.SetProgress(true);

				loader.HideAll();

				this.AddChild(currentLevel);

				currentLevel.AssetName = loadingAssetname;

				return;
			}
		}
	}
}
