using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Juicy;
using Xam.Audio;

namespace Moonshot.Gameplay.Environment
{
	public class Planet : MonoBehaviour
	{
		[SerializeField] private float m_bounceDuration = 0.5f;

		[Header( "SFX" )]
		[SerializeField] private SfxClip m_collisionSfx = default;

		private BounceFx m_bounceFx = null;

		private void OnCollisionEnter2D( Collision2D collision )
		{
			m_bounceFx.Play( m_bounceDuration );
			m_collisionSfx?.PlaySfx();
		}

		private void Awake()
		{
			m_bounceFx = GetComponent<BounceFx>();
		}
	}
}