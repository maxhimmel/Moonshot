using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Testing
{
	using ReConsts;

	public class TopDownMovement : MonoBehaviour
	{
		[Header( "Movement" )]
		[SerializeField] private float m_acceleration = 10;
		[SerializeField] private float m_maxSpeed = 5;

		private Rewired.Player m_input = null;

		private Rigidbody2D m_rigidbody = null;
		private Vector2 m_velocity = Vector2.zero;
		private Vector2 m_desiredVelocity = Vector2.zero;

		private void Update()
		{
			Vector2 moveInput = GetMoveInput();
			SetDesiredVelocity( moveInput );
		}

		private Vector2 GetMoveInput()
		{
			Vector2 rawInput = m_input.GetAxis2D( Action.MoveHorizontal, Action.MoveVertical );
			Vector2 moveInput = Vector2.ClampMagnitude( rawInput, 1 );

			return moveInput;
		}

		private void SetDesiredVelocity( Vector3 moveDirection )
		{
			m_desiredVelocity = moveDirection * m_maxSpeed;
		}

		private void FixedUpdate()
		{
			// UPDATE STATE ...
			m_velocity = m_rigidbody.velocity;


			// ACCELERATE ...
			float speedDelta = m_acceleration * Time.deltaTime;
			m_velocity = Vector2.MoveTowards( m_velocity, m_desiredVelocity, speedDelta );


			// APPLY MOVEMENT ...
			m_rigidbody.velocity = m_velocity;
		}

		private void Start()
		{
			m_input = Rewired.ReInput.players.GetPlayer( 0 );
		}

		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody2D>();
		}
	}
}