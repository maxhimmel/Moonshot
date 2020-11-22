using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Gameplay.Environment
{
	[RequireComponent( typeof( SpriteRenderer ) )]
	public class Background : MonoBehaviour
	{
		private float PixelsPerUnit { get { return m_sprite.pixelsPerUnit; } }
		private Vector2 Size { get { return m_sprite.rect.size; } }
		private float Scale { get { return transform.lossyScale.x; } }

		private Sprite m_sprite = null;

		public void Init()
		{
			SpriteRenderer renderer = GetComponent<SpriteRenderer>();
			Debug.Assert( renderer != null, $"Invalid background object. Missing SpriteRenderer component.", this );

			m_sprite = renderer.sprite;
		}

		public Vector2 GetLossyTileSize()
		{
			if ( m_sprite == null ) { return Vector2.zero; }

			return Scale * (Size / PixelsPerUnit);
		}

		private void Awake()
		{
			if ( m_sprite == null )
			{
				Init();
			}
		}
	}
}