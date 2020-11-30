using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xam;
using Xam.Ui;

namespace Moonshot.Gameplay.UI
{
	public class LoseMenu : MonoBehaviour
	{
		[SerializeField] private Button m_againButton = default;
		[SerializeField] private Button m_exitButton = default;

		[Space]
		[SerializeField] private float m_showSpeed = 1;
		
		private CanvasGroupWrapper m_canvasGroupWrapper = null;

		private void Awake()
		{
			m_canvasGroupWrapper = GetComponent<CanvasGroupWrapper>();
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
			m_canvasGroupWrapper.CrossFade( 1, m_showSpeed );
		}
	}
}