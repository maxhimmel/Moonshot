using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;

namespace Moonshot.Gameplay
{
	public class GameMode : SingletonMono<GameMode>
	{
		public enum EState
		{
			Playing,
			Win,
			Lose
		}

		public event System.Action<GameMode> OnGameWonEvent;
		public event System.Action<GameMode> OnGameLostEvent;

		public EState CurrentState { get; private set; } = EState.Playing;

		public void Win()
		{
			if ( CurrentState != EState.Playing )
			{
				Debug.LogError( $"Cannot 'WIN' if game is not `PLAYING.`\n" +
					$"Game state has already been determined.", this );
				return;
			}

			Debug.Log( $"<size=25>WIN</size>" );

			CurrentState = EState.Win;
			OnGameWonEvent?.Invoke( this );
		}

		public void Lose()
		{
			if ( CurrentState != EState.Playing )
			{
				Debug.LogError( $"Cannot 'LOSE' if game is not `PLAYING.`\n" +
					$"Game state has already been determined.", this );
				return;
			}

			Debug.Log( $"<size=25>LOSE</size>" );

			CurrentState = EState.Lose;
			OnGameLostEvent?.Invoke( this );
		}
	}
}