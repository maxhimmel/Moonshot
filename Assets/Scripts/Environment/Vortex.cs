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
			Debug.Log( $"PLAYER ENTERED ZONE - <size=20>WIN GAME</size>" );

			base.OnTargetEntered( target );
		}
	}
}