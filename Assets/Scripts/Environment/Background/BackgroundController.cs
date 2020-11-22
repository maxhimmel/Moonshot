using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Gameplay.Environment
{
	using Factory;

	public class BackgroundController : MonoBehaviour
	{
		[SerializeField] private Transform m_followTarget = default;

		[Space]
		[SerializeField] private int m_gridResolution = 3;
		[SerializeField] private Vector2 m_scrollSpeed = Vector2.zero;

		private BackgroundFactory m_bgFactory = default;
		private Transform[] m_bgTiles = null;
		private Vector2 m_maxDistance = Vector2.zero;

		private void LateUpdate()
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
				int offset = (int)Mathf.Sign( tile2Follow.x ) * m_gridResolution;
				tilePos += Vector3.right * lossyTileSize.x * offset;
			}
			if ( yDist >= m_maxDistance.y )
			{
				int offset = (int)Mathf.Sign( tile2Follow.y ) * m_gridResolution;
				tilePos += Vector3.up * lossyTileSize.y * offset;
			}

			return tilePos;
		}

		private void Awake()
		{
			m_bgFactory = GetComponentInChildren<BackgroundFactory>();
		}

		private void Start()
		{
			InitBgTiles();
		}

		private void InitBgTiles()
		{
			int totalTileCount = m_gridResolution * m_gridResolution;
			m_bgTiles = new Transform[totalTileCount];

			Vector2 lossyTileSize = GetLossyTileSize();
			Vector3 gridOffset = lossyTileSize * Mathf.FloorToInt( m_gridResolution / 2 );

			for ( int idx = 0; idx < totalTileCount; ++idx )
			{
				int row = idx / m_gridResolution;
				int col = idx % m_gridResolution;

				Vector3 tilePos = new Vector3()
				{
					x = row * lossyTileSize.x,
					y = col * lossyTileSize.y,
					z = transform.position.z
				};
				tilePos -= gridOffset;

				Background newBgTile = m_bgFactory.Create( tilePos, Quaternion.identity, transform );
				m_bgTiles[idx] = newBgTile.transform;
			}

			m_maxDistance = (m_gridResolution - 2) * lossyTileSize;
			m_maxDistance += lossyTileSize / 2f;
		}

		private Vector2 GetLossyTileSize()
		{
			return m_bgFactory.GetLossyTileSize();
		}
	}
}