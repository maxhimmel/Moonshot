using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;

namespace Moonshot.Gameplay
{
	using Player;

	public class Vortex : ProximityBucket2D<PlayerController>
	{
		protected override void OnTargetEntered( PlayerController target )
		{
			base.OnTargetEntered( target );
			
			GameMode.Instance.Win();
		}
	}
}