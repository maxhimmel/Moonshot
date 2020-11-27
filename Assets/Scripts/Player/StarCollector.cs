using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;

namespace Moonshot.Gameplay.Pickups
{
	public class StarCollector : ProximityBucket2D<StarPickup>, ICollector
	{
		[Header( "Star Collector" )]
		[SerializeField] private float m_pullForce = 30;

		void ICollector.Collect( IPickup pickupItem )
		{
			pickupItem.Acquire();
		}

		private void FixedUpdate()
		{
			if ( HasAvailableTargets )
			{
				foreach ( StarPickup star in Targets )
				{
					Vector3 forceDir = (transform.position - star.transform.position).normalized;
					star.AddForce( forceDir * m_pullForce );
				}
			}
		}
	}
}