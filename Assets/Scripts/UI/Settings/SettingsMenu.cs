using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;

namespace Moonshot.Gameplay.UI
{
	public class SettingsMenu : MonoBehaviour
	{
		private CanvasGroup m_canvasGroup = null;

		public void ToggleShow()
		{
			//bool isActive = !gameObject.activeInHierarchy;
			//if ( isActive )
			//{
			//	Show();
			//}
			//else
			//{
			//	Hide();
			//}

			GameMode.Instance.TogglePause();
		}

		private void Show()
		{
			gameObject.SetActive( true );
			m_canvasGroup.alpha = 1;
		}

		private void Hide()
		{
			gameObject.SetActive( false );
			m_canvasGroup.alpha = 0;
		}

		private void OnEnable()
		{
			TimeManager.Instance.SetTimeScale( 0, 0 );
		}

		private void OnDisable()
		{
			TimeManager.Instance.SetTimeScale( 1, 0 );
		}

		private void Start()
		{
			Hide();
			GameMode.Instance.OnGamePauseChangedEvent += OnGamePauseChanged;
		}

		private void OnGamePauseChanged( GameMode gameMode )
		{
			GameMode.EState state = gameMode.CurrentState;
			if ( state == GameMode.EState.Paused )
			{
				Show();
			}
			else if ( state == GameMode.EState.Playing )
			{
				Hide();
			}
		}

		private void Awake()
		{
			m_canvasGroup = GetComponent<CanvasGroup>();
		}
	}
}