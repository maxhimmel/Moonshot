using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Extensions;

namespace Moonshot.Utility
{
	public class RotateFx : MonoBehaviour
	{
		public bool IsPlaying { get { return m_updateRoutine != null; } }

		[SerializeField] private bool m_playOnStart = true;
		[SerializeField] private bool m_isLooping = true;

		[Space]
		[SerializeField] private Vector3 m_axis = Vector3.forward;
		[SerializeField] private AnimationCurve m_animCurve = AnimationCurve.Linear( 0, 0, 1, 1 );

		private Coroutine m_updateRoutine = null;

		private void Start()
		{
			if ( m_playOnStart )
			{
				Play();
			}
		}

		public void Stop()
		{
			if ( IsPlaying )
			{
				StopCoroutine( m_updateRoutine );
				m_updateRoutine = null;
			}
		}

		public void Play()
		{
			if ( IsPlaying ) { return; }

			m_updateRoutine = StartCoroutine( Update_Coroutine() );
		}

		private IEnumerator Update_Coroutine()
		{
			float duration = m_animCurve.GetDuration();
			float timer = 0;

			while ( timer < duration || m_isLooping )
			{
				timer += Time.deltaTime;

				float value = m_animCurve.Evaluate( timer );
				transform.localRotation = Quaternion.Euler( m_axis * value );

				yield return null;
			}

			m_updateRoutine = null;
		}
	}
}