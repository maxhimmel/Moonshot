using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Testing
{
	public class DummyMovement : MonoBehaviour
	{
		[SerializeField] private float m_moveRate = 3;

		[SerializeField] private bool m_stopPrevMovement = true;
		[SerializeField] private float m_impulseForce = 10;

		private Rigidbody2D m_rigidbody = null;
		private float m_nextMoveTime = 0;

		private void FixedUpdate()
		{
			if ( m_nextMoveTime <= Time.timeSinceLevelLoad )
			{
				m_nextMoveTime = Time.timeSinceLevelLoad + m_moveRate;

				if ( m_stopPrevMovement )
				{
					m_rigidbody.velocity = Vector2.zero;
				}

				Vector2 randImpulse = Random.insideUnitCircle* m_impulseForce;
				m_rigidbody.AddForce( randImpulse, ForceMode2D.Impulse );
			}
		}

		private void Awake()
		{
			m_rigidbody = GetComponentInChildren<Rigidbody2D>();
			m_nextMoveTime = Time.timeSinceLevelLoad + m_moveRate;
		}
	}
}