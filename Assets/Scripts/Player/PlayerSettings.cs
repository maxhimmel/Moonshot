using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;

namespace Moonshot.Gameplay.Player
{
	public class PlayerSettings : SingletonMono<PlayerSettings>
	{
		public IMoveInterpreter CurrentMovement { get; private set; } = new RelativeMoveInterpreter();

		[SerializeField] private bool m_useClockwiseMovement = true;

		private void Start()
		{
			OnValidate();
		}

		private void OnValidate()
		{
			if ( Application.isPlaying )
			{
				SetMovementMode( m_useClockwiseMovement );
			}
		}

		public void SetMovementMode( bool isClockwise )
		{
			CurrentMovement = MoveInterpreterHelper.CreateInterpreter( isClockwise );

			m_useClockwiseMovement = isClockwise;
			SetPlayerControls( isClockwise );
		}

		private void SetPlayerControls( bool isClockwise )
		{
			PlayerController player = DynamicPool.Instance.GetFirstPooledObjectByType<PlayerController>();
			if ( player != null )
			{
				player.SetMoveInterpreter( CurrentMovement );
			}
		}
	}
}