using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay.Patterns;

namespace Moonshot.Gameplay.Environment.Factory
{
	public class BackgroundFactory : Factory<Background>
	{
		public Vector2 GetLossyTileSize()
		{
			if ( m_prefab == null ) { return Vector2.zero; }

			return m_prefab.GetLossyTileSize();
		}

		public override Background Create( Vector3 position = default, Quaternion rotation = default, Transform parent = null )
		{
			Background newBg = base.Create( position, rotation, parent );

			if ( newBg != null )
			{
				newBg.gameObject.layer = gameObject.layer;
			}

			return newBg;
		}

		protected override void Awake()
		{
			m_prefab.Init();

			base.Awake();
		}
	}
}