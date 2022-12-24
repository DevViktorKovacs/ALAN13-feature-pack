using ALAN13featurepack.GameWorld;
using ALAN13featurepack.Utility;
using Godot;
using Godot.Collections;
using System;

public class ControlPanel : Panel
{
	public TileGridControl TileGridControl { get; set; }

	Dictionary<int, string> pathFinderAlgorithms = new Dictionary<int, string>()
	{
		{ 0, "Built in A* (Godot)" },
		{ 1, "Simple Manhattan A*" },
		{ 2, "ALAN*" },
	   
	};
	public override void _Ready()
	{
		var itemList = this.GetChild<ItemList>();

		itemList.AddItem(pathFinderAlgorithms[0]);
		itemList.AddItem(pathFinderAlgorithms[1]);
		itemList.AddItem(pathFinderAlgorithms[2]);
	}
	
	private void _on_ItemList_item_selected(int index)
	{
		DebugHelper.PrettyPrintVerbose(pathFinderAlgorithms[index]);

		TileGridControl.GenerateAstar((ALAN13featurepack.Interfaces.PathfinderAlgorithm)index);
	}

}


