using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Gameplay.Pickups
{
	public class StarPickup : MonoBehaviour, IPickup
	{
		[SerializeField] private float m_energyRefill = 10;
		[SerializeField] private float m_deathDuration = 0.65f;

		private Rigidbody2D m_rigidbody = null;
		private Coroutine m_deathRoutine = null;

		void IPickup.Acquire()
		{
			if ( EnergyManager.Exists )
			{
				EnergyManager.Instance.AddEnergy( m_energyRefill );
			}

			if ( m_deathRoutine == null )
			{
				m_deathRoutine = StartCoroutine( UpdateDeath_Coroutine() );
			}
		}

		private IEnumerator UpdateDeath_Coroutine()
		{
			Vector3 startScale = transform.localScale;
			Vector3 endScale = Vector3.zero;

			float timer = 0;
			while ( timer < 1 )
			{
				timer += Time.deltaTime / m_deathDuration;
				timer = Mathf.Min( timer, 1 );

				Vector3 newScale = Vector3.Lerp( startScale, endScale, timer );
				transform.localScale = newScale;

				yield return null;
			}

			m_deathRoutine = null;
			Destroy( gameObject );
		}

		public void AddForce( Vector3 force, ForceMode2D mode = ForceMode2D.Force )
		{
			m_rigidbody.AddForce( force, mode );
		}

		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody2D>();
		}
	}
}