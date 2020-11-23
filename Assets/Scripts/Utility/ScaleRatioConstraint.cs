using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Utlity
{
	[ExecuteInEditMode]
	public class ScaleRatioConstraint : MonoBehaviour
	{
		[SerializeField] private Transform m_source = default;
		[SerializeField] private Vector3 m_worldOffset = Vector3.one;

		private void Awake()
		{
			transform.localScale = GetAdjustedScale();
		}

		private Vector3 GetAdjustedScale()
		{
			Vector3 sourceScale = m_source.localScale;
			Vector3 newOffset = new Vector3()
			{
				x = m_worldOffset.x / sourceScale.x * 2,
				y = m_worldOffset.y / sourceScale.y * 2,
				z = m_worldOffset.z / sourceScale.z * 2
			};

			return Vector3.one + newOffset;
		}

#if UNITY_EDITOR
		private void Update()
		{
			if ( transform.hasChanged )
			{
				transform.localScale = GetAdjustedScale();
			}
		}
#endif
	}
}