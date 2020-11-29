using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xam.Audio;

namespace Moonshot.Gameplay.UI
{
	public class AudioSettingsWidget : MonoBehaviour
	{
		[SerializeField] private Slider m_musicSlider = default;
		[SerializeField] private Slider m_sfxSlider = default;

		private void Awake()
		{
			m_musicSlider.onValueChanged.AddListener( OnMusicVolumeChanged );
			m_sfxSlider.onValueChanged.AddListener( OnSfxVolumeChanged );
		}

		private void OnMusicVolumeChanged( float newVolume )
		{
			AudioManager.Instance.SetMusicVolume( newVolume );
		}

		private void OnSfxVolumeChanged( float newVolume )
		{
			AudioManager.Instance.SetSfxVolume( newVolume );
		}
	}
}