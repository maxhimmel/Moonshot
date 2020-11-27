using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Gameplay.Pickups
{
	public class StarPickup : MonoBehaviour, IPickup
	{
		[SerializeField] private float m_energyRefill = 10;

		private Rigidbody2D m_rigidbody = null;

		void IPickup.Acquire()
		{
			if ( EnergyManager.Exists )
			{
				EnergyManager.Instance.AddEnergy( m_energyRefill );
			}

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