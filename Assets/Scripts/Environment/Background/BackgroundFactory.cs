using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay.Patterns;

namespace Moonshot.Gameplay.Environment.Factory
{
	public class BackgroundFactory : Factory<Background>
	{
		protected override void Awake()
		{
			m_prefab.Init();

			base.Awake();
		}

		public Vector2 GetLossyTileSize()
		{
			if ( m_prefab == null ) { return Vector2.zero; }

			return m_prefab.GetLossyTileSize();
		}
	}
}