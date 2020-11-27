using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Gameplay.Pickups
{
	public interface ICollector
	{
		void Collect( IPickup pickupItem );
	}
}