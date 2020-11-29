using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;

namespace Moonshot.Gameplay.MiniMap
{
	public class MiniMapWorldMarker : DynamicPoolAttachment<MiniMapWorldMarker>
	{
		[SerializeField] private Sprite m_marker = default;

		public Vector3 GetWorldPosition()
		{
			return transform.position;
		}

		public Sprite GetMarker()
		{
			return m_marker;
		}
	}
}