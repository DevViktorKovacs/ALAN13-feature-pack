using ALAN13featurepack.Interfaces;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALAN13featurepack.Utility
{
	public class InputProcessor : Node
	{
		public event EventHandler<InputEventArgs> MouseMotion;

		public event EventHandler<InputEventArgs> MouseScrollUp;

		public event EventHandler<InputEventArgs> MouseScrollDown;

		public event EventHandler<InputEventArgs> MouseLeftUp;

		public event EventHandler<InputEventArgs> MouseLeftDown;

		public event EventHandler<InputEventArgs> MouseRightClick;

		public event EventHandler<InputEventArgs> MouseMiddleClick;

		public event EventHandler<InputEventArgs> MousePanned;

		public event EventHandler<InputEventArgs> MousePanRelease;

		public event EventHandler<InputEventArgs> MouseDragged;

		public event EventHandler<InputEventArgs> MouseDragRelease;

		public event EventHandler<InputEventArgs> KeyPressed;

		public event EventHandler<InputEventArgs> ActionTriggered_UIFocusNext;

		public event EventHandler<InputEventArgs> ActionTriggered_UILeft;

		public event EventHandler<InputEventArgs> ActionTriggered_UIRight;

		public event EventHandler<InputEventArgs> ActionTriggered_UIUp;

		public event EventHandler<InputEventArgs> ActionTriggered_UIDown;

		public event EventHandler<InputEventArgs> ActionTriggered_UIPageUp;

		public event EventHandler<InputEventArgs> ActionTriggered_UIPageDown;

		private Vector2 panStart;

		private Vector2 dragStart;

		private bool drag;

		public static bool VerboseLogging = false;

		public static bool InputBlocked = false;

		public static bool TutorialEnabled = true;

		public static bool TextEditMode = false;

		public static bool IsInDebugMode = true;

		public static bool IsSteamInitialized = false;

		public static int gridIds = 0;

		public static bool InterferenceDisabled = false;

		public override void _Input(InputEvent @event)
		{
			if (@event is InputEventMouse)
			{
				HandleMouseEvent((InputEventMouse)@event);
			}

			if (@event is InputEventKey)
			{
				if (InputBlocked) return;

				HandleInputKeyEvent((InputEventKey)@event);
			}
		}

		private void HandleInputKeyEvent(InputEventKey @event)
		{
			if (@event.IsPressed())
			{
				var input = OS.GetScancodeString(@event.Scancode);

				var e = new InputEventArgs()
				{
					InputString = input
				};

				DebugHelper.PrettyPrintVerbose($"Input received: {e.InputString}", ConsoleColor.DarkGray);

				RaiseInputEvent(e, KeyPressed);
			}

			if (Input.IsActionJustReleased("ui_focus_next"))
			{
				RaiseInputEvent(new InputEventArgs(), ActionTriggered_UIFocusNext);
			}

			if (Input.IsActionJustReleased("ui_page_up"))
			{
				RaiseInputEvent(new InputEventArgs(), ActionTriggered_UIPageUp);
			}

			if (Input.IsActionJustReleased("ui_page_down"))
			{
				RaiseInputEvent(new InputEventArgs(), ActionTriggered_UIPageDown);
			}
		}

		private void HandleMouseEvent(InputEventMouse @event)
		{
			if (@event is InputEventMouseButton)
			{
				HandleMouseButtonPress((InputEventMouseButton)@event);

			}

			if (@event is InputEventMouseMotion)
			{
				HandleMouseMotion((InputEventMouseMotion)@event);
			}
		}

		private void HandleMouseButtonPress(InputEventMouseButton @event)
		{
			if (@event.ButtonIndex == (int)MouseButtonIndex.Right)
			{
				panStart = @event.GlobalPosition;

				var e = new InputEventArgs()
				{
					MouseButtonIndex = @event.ButtonIndex
				};

				RaiseInputEvent(e, MouseRightClick);

				return;
			}

			if (@event.ButtonIndex == (int)MouseButtonIndex.ScrollUp)
			{
				RaiseInputEvent(new InputEventArgs(), MouseScrollUp);
			}

			if (@event.ButtonIndex == (int)MouseButtonIndex.ScrollDown)
			{
				RaiseInputEvent(new InputEventArgs(), MouseScrollDown);
			}

			if (@event.ButtonIndex == (int)MouseButtonIndex.Left && !@event.Pressed)
			{
				drag = false;

				var e = new InputEventArgs()
				{
					MouseButtonIndex = @event.ButtonIndex
				};

				RaiseInputEvent(e, MouseLeftUp);
			}

			if (@event.ButtonIndex == (int)MouseButtonIndex.Left && @event.Pressed)
			{
				dragStart = @event.GlobalPosition;

				drag = true;

				var e = new InputEventArgs()
				{
					MouseButtonIndex = @event.ButtonIndex
				};

				RaiseInputEvent(e, MouseLeftDown);

				return;
			}
		}

		private void HandleMouseMotion(InputEventMouseMotion @event)
		{
			var e = new InputEventArgs()
			{
				MouseButtonIndex = @event.ButtonMask,

				Offset = panStart - @event.GlobalPosition
			};

			if (Input.IsActionPressed(StaticData.InputData[CommandKey.RightClick]))
			{
				panStart = @event.GlobalPosition;

				RaiseInputEvent(e, MousePanned);

				return;
			}

			if (drag)
			{
				e.Offset = dragStart - @event.GlobalPosition;

				RaiseInputEvent(e, MouseDragged);

				return;
			}

			RaiseInputEvent(e, MouseMotion);
		}

		public virtual void RaiseInputEvent(InputEventArgs e, EventHandler<InputEventArgs> eventHandler)
		{
			eventHandler?.Invoke(this, e);
		}
	}

	public class InputEventArgs : EventArgs
	{
		public Vector2 Offset { get; set; }

		public string InputString { get; set; }

		public int MouseButtonIndex { get; set; }
	}

	public enum MouseButtonIndex
	{
		Release = 0,
		Left = 1,
		Right = 2,
		Middle = 3,
		ScrollUp = 5,
		ScrollDown = 4
	}
}
