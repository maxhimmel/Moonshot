using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xam.Utility.Patterns;

namespace Moonshot.Gameplay.UI
{
	using Player;

	public class ControlsSettingsWidget : MonoBehaviour
	{
		[SerializeField] private Button m_clockwiseButton = default;
		[SerializeField] private Button m_relativeButton = default;

		private void Start()
		{
			m_clockwiseButton.onClick.AddListener( () => PlayerSettings.Instance.SetMovementMode( true ) );
			m_relativeButton.onClick.AddListener( () => PlayerSettings.Instance.SetMovementMode( false ) );
		}
	}
}