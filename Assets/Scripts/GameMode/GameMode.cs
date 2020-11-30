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
			Paused,
			Win,
			Lose
		}

		public event System.Action<GameMode> OnGameWonEvent;
		public event System.Action<GameMode> OnGameLostEvent;
		public event System.Action<GameMode> OnGamePauseChangedEvent;

		public EState CurrentState { get; private set; } = EState.Playing;

		public void TogglePause()
		{
			EState state = CurrentState;
			if ( state == EState.Win || state == EState.Lose ) { return; }

			if ( state == EState.Playing )
			{
				state = EState.Paused;
			}
			else if ( state == EState.Paused )
			{
				state = EState.Playing;
			}

			CurrentState = state;

			Debug.Log( CurrentState );

			OnGamePauseChangedEvent?.Invoke( this );
		}

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