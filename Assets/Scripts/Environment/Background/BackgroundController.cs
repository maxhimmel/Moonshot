using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Gameplay.Environment
{
	public class BackgroundController : MonoBehaviour
	{
		[SerializeField] private Transform m_followTarget = default;

		[Space]
		[SerializeField] private int m_gridSize = 3;
		[SerializeField] private Background m_bgPrefab = default;

		[Space]
		[SerializeField] private Transform m_bgContainer = default;
		[SerializeField] private int m_bgPixelsPerUnit = 100;
		[SerializeField] private Vector2 m_bgSize = new Vector2( 1200, 800 );
		[SerializeField] private float m_bgScale = 2;

		[Space]
		[SerializeField] private Vector2 m_scrollSpeed = Vector2.zero;

		private Transform[] m_bgTiles = null;
		private int m_columnRowCount = -1;
		private Vector2 m_maxDistance = Vector2.zero;

		private void Update()
		{
			for ( int idx = 0; idx < m_bgTiles.Length; ++idx )
			{
				Transform tile = m_bgTiles[idx];

				tile.position += GetScrollOffset();
				tile.position = GetWrapPosition( tile.position );
			}
		}

		private Vector3 GetScrollOffset()
		{
			return m_scrollSpeed * Time.deltaTime;
		}

		private Vector3 GetWrapPosition( Vector3 tilePos )
		{
			Vector3 tile2Follow = m_followTarget.position - tilePos;
			float xDist = Mathf.Abs( tile2Follow.x );
			float yDist = Mathf.Abs( tile2Follow.y );

			Vector3 lossyTileSize = GetLossyTileSize();
			if ( xDist >= m_maxDistance.x )
			{
				int offset = (int)Mathf.Sign( tile2Follow.x ) * m_columnRowCount;
				tilePos += Vector3.right * lossyTileSize.x * offset;
			}
			if ( yDist >= m_maxDistance.y )
			{
				int offset = (int)Mathf.Sign( tile2Follow.y ) * m_columnRowCount;
				tilePos += Vector3.up * lossyTileSize.y * offset;
			}

			return tilePos;
		}

		private void Awake()
		{
			InitBgTiles();
		}

		private void InitBgTiles()
		{
			int bgTileCount = m_bgContainer.childCount;

			m_columnRowCount = (int)Mathf.Sqrt( bgTileCount );
			m_bgTiles = new Transform[bgTileCount];

			for ( int idx = 0; idx < bgTileCount; ++idx )
			{
				m_bgTiles[idx] = m_bgContainer.GetChild( idx );
			}

			Vector2 lossyTileSize = GetLossyTileSize();

			m_maxDistance = (m_columnRowCount - 2) * lossyTileSize;
			m_maxDistance += lossyTileSize / 2f;
		}

		private Vector2 GetLossyTileSize()
		{
			return m_bgScale * (m_bgSize / m_bgPixelsPerUnit);
		}
	}
}