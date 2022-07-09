using ALAN13featurepack.Interfaces;
using System;

namespace ALAN13featurepack.GameWorld.CharacterStates
{
	public class AnimationFinishedEventArgs : EventArgs
	{
		public int Frame { get; set; }

		public int FrameCount { get; set; }

		public string AnimationName { get; set; }

		public decimal AnimiationID { get; set; }

	}

	public class CommandFinishedEventArgs : EventArgs
	{
		public StateEnum PreviousState { get; set; }

		public StateEnum NewState { get; set; }

		public int CommandID { get; set; }

		public bool Handled { get; set; }
	}

	public class EnteringCellEventArgs : EventArgs
	{
		public TileCell OldCell { get; set; }
		public TileCell NewCell { get; set; }

	}

	public class StateFinishedEventArgs : EventArgs
	{
		public StateEnum NextState { get; set; }

		public CommandKey Input { get; set; }
	}
}
