using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Juicy;

namespace Moonshot.Gameplay.Environment
{
	public class Planet : MonoBehaviour
	{
		[SerializeField] private float m_bounceDuration = 0.5f;

		private BounceFx m_bounceFx = null;

		private void OnCollisionEnter2D( Collision2D collision )
		{
			m_bounceFx.Play( m_bounceDuration );
		}

		private void Awake()
		{
			m_bounceFx = GetComponent<BounceFx>();
		}
	}
}