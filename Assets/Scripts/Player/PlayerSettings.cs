using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;

namespace Moonshot.Gameplay.Player
{
	public class PlayerSettings : MonoBehaviour
	{
		[SerializeField] private bool m_useClockwiseMovement = true;

		private void Start()
		{
			OnValidate();
		}

		private void OnValidate()
		{
			if ( !Application.isPlaying ) { return; }

			PlayerController player = DynamicPool.Instance.GetFirstPooledObjectByType<PlayerController>();
			if ( player != null )
			{
				player.SetMoveInterpreter( MoveInterpreterHelper.CreateInterpreter( m_useClockwiseMovement ) );
			}
		}
	}
}