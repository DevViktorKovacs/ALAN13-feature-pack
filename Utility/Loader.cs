using ALAN13featurepack.Utility;
using Godot;
using System;
using System.Diagnostics;

public class Loader : Node2D
{
	[Export]
	public int PollingInterval = 100;

	public event EventHandler<LoadingFinishedEventArgs> LoadingFinished;

	ResourceInteractiveLoader loader;

	Resource resource;

	Stopwatch stopWatch = new Stopwatch();

	Error status = Error.Ok;

	string assetName;

	int assetCount;

	Label loadingLabel;

	ProgressBar loadingProgress;

	public override void _Ready()
	{
		loadingLabel = (Label)this.GetChildByName(nameof(loadingLabel));

		loadingProgress = (ProgressBar)this.GetChildByName(nameof(loadingProgress));
	}

	public void LoadAsset(string path, string assetName, int chunkCount = 0)
	{
		status = Error.Ok;

		assetCount = chunkCount;

		this.assetName = assetName;

		loadingProgress.Visible = true;

		loadingLabel.Visible = true;

		loader = ResourceLoader.LoadInteractive(path);

		SetProcess(true);

		stopWatch.Start();
	}

	public override void _Process(float delta)
	{
		if (loader == null)
		{
			SetProcess(false);

			return;
		}

		stopWatch.Restart();

		while (stopWatch.ElapsedMilliseconds < PollingInterval && status != Error.FileEof)
		{
			status = loader.Poll();
		}

		if (status == Error.FileEof)
		{
			resource = loader.GetResource();

			loader.Dispose();

			loader = null;

			var e = new LoadingFinishedEventArgs() { Resource = resource, Error = status, AssetName = assetName };

			OnLoadingFinished(e);

			return;
		}

		if (status == Error.Ok)
		{
			UpdateProgres();
		}
		else
		{
			loader.Dispose();

			loader = null;

			var e = new LoadingFinishedEventArgs() { Error = status };

			OnLoadingFinished(e);
		}
	}

	private void UpdateProgres()
	{
		var stageCount = assetCount == 0 ? loader.GetStageCount() : assetCount;

		var progress = (float)(loader.GetStage()) / stageCount;

		loadingProgress.Value = progress * 100;
	}

	public virtual void OnLoadingFinished(LoadingFinishedEventArgs e)
	{
		LoadingFinished?.Invoke(this, e);
	}

	public void SetProgress(bool max)
	{
		loadingProgress.Value = max ? loadingProgress.MaxValue : loadingProgress.MinValue;
	}

	public void HideAll()
	{
		loadingProgress.Visible = false;
		loadingLabel.Visible = false;
	}
}

public class LoadingFinishedEventArgs : EventArgs
{
	public Resource Resource { get; set; }

	public Error Error { get; set; }

	public string AssetName { get; set; }
}
