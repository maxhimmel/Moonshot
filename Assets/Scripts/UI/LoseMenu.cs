using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xam;

namespace Moonshot.Gameplay.UI
{
	public class LoseMenu : MonoBehaviour
	{
		[SerializeField] private Button m_againButton = default;
		[SerializeField] private Button m_exitButton = default;

		private CanvasGroup m_canvasGroup = null;

		private void Awake()
		{
			m_canvasGroup = GetComponent<CanvasGroup>();
		}

		private void Start()
		{
			m_againButton.onClick.AddListener( GameManager.Instance.ReloadActiveLevel );
			m_exitButton.onClick.AddListener( GameManager.Instance.LoadMainMenu );

			GameMode.Instance.OnGameLostEvent += OnGameLost;

			gameObject.SetActive( false );
		}

		private void OnGameLost( GameMode gameMode )
		{
			gameObject.SetActive( true );
			m_canvasGroup.alpha = 1;
		}
	}
}