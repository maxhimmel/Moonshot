using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Gameplay.Player
{
	public static class MoveInterpreterHelper
	{
		public static IMoveInterpreter CreateInterpreter( bool isClockwise )
		{
			return isClockwise
				? (IMoveInterpreter)new ClockwiseMoveInterpreter()
				: (IMoveInterpreter)new RelativeMoveInterpreter();
		}
	}

	public interface IMoveInterpreter
	{
		Vector2 Interpret( Vector2 moveInput, Vector2 gravityNormal );
	}

	public class ClockwiseMoveInterpreter : IMoveInterpreter
	{
		private Vector2 WorldRight { get { return Vector2.right; } }

		Vector2 IMoveInterpreter.Interpret( Vector2 moveInput, Vector2 gravityNormal )
		{
			Vector2 gravityTangent = Quaternion.Euler( 0, 0, -90 ) * gravityNormal;
			float moveDir = Vector2.Dot( moveInput, WorldRight );

			return gravityTangent * moveDir;
		}
	}

	public class RelativeMoveInterpreter : IMoveInterpreter
	{
		Vector2 IMoveInterpreter.Interpret( Vector2 moveInput, Vector2 gravityNormal )
		{
			return Vector3.ProjectOnPlane( moveInput, gravityNormal );
		}
	}
}