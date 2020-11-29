using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Utility
{
	[RequireComponent( typeof( SpriteRenderer ) )]
	public class SpriteSwapper : MonoBehaviour
	{
		[SerializeField] private Sprite[] m_sprites = default;

		private void Start()
		{
			SpriteRenderer renderer = GetComponent<SpriteRenderer>();
			if ( renderer != null )
			{
				renderer.sprite = GetRandomSprite();
			}
		}

		private Sprite GetRandomSprite()
		{
			if ( m_sprites == null || m_sprites.Length <= 0 ) { return null; }

			int randIdx = Random.Range( 0, m_sprites.Length );
			return m_sprites[randIdx];
		}
	}
}