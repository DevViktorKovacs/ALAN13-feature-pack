using ALAN13featurepack.Interfaces;
using ALAN13featurepack.Utility;
using Godot;

public class BaseScene : Node2D
{
	public ControlPanel ControlPanel { get; set; }

	Loader loader;

	LevelController currentLevel;

	SteamManager steamManager;

	string loadingAssetname;

	public override void _Ready()
	{
		DebugHelper.Print("Loading...");

		var pathToLevel = "res://GameWorld/LevelController.tscn";

		loader = this.GetChild<Loader>();

		loader.LoadingFinished += Loader_LoadingFinished;

		loader.LoadAsset(pathToLevel, AssetKeys.FirstLevel.ToString(), 10);

		ControlPanel = this.GetChildRecursive<ControlPanel>();

		InputProcessor.VerboseLogging = true;

		DebugHelper.PrettyPrintVerbose($"Initializing Steam API...");

		steamManager = new SteamManager();

		try
		{
			var success = steamManager.Init();

			InputProcessor.IsSteamInitialized = success;

			steamManager.Update();
		}

		catch 
		{
			DebugHelper.PrintError($"Steam dll not found. Include steam_api64.dll in the root directory for steamworks integration!");
		}
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

				ControlPanel.Visible = true;

				ControlPanel.TileGridControl = currentLevel.TileGridControl;

				currentLevel.AssetName = loadingAssetname;

				return;
			}
		}
	}
}
