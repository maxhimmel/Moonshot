using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Gameplay
{
	using ReConsts;

	public class LinearMovement : MonoBehaviour
	{
		[Header( "Gravity" )]
		[SerializeField] private float m_gravityForce = 9.81f;

		[Header( "Movement" )]
		[SerializeField] private float m_acceleration = 10;
		[SerializeField] private float m_maxSpeed = 5;
		[SerializeField] private float m_jumpHeight = 3.25f;

		private Rewired.Player m_input = null;

		private Rigidbody2D m_rigidbody = null;
		private Vector2 m_velocity = Vector2.zero;
		private Vector2 m_desiredVelocity = Vector2.zero;
		private bool m_isJumpDesired = false;
		private bool m_isGrounded = false;

		private void Update()
		{
			Vector2 moveInput = GetMoveInput();
			SetDesiredVelocity( moveInput );

			if ( m_input.GetButtonDown( Action.Jump ) )
			{
				TryJump();
			}
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

		private bool TryJump()
		{
			m_isJumpDesired = true;
			return true;
		}
		
		private void FixedUpdate()
		{
			// UPDATE STATE ...
			//m_velocity = m_rigidbody.velocity;


			// ACCELERATE ...
			float speedDelta = m_acceleration * Time.deltaTime;
			m_velocity.x = Mathf.MoveTowards( m_velocity.x, m_desiredVelocity.x, speedDelta );


			// JUMP ...
			if ( m_isJumpDesired )
			{
				Jump();
			}


			// GRAVITY ...
			if ( !m_isGrounded )
				m_velocity.y -= m_gravityForce * Time.deltaTime;


			// APPLY MOVEMENT ...
			//m_rigidbody.velocity = m_velocity;
			m_rigidbody.MovePosition( m_rigidbody.position + m_velocity * Time.deltaTime );
		}

		private void OnCollisionEnter2D( Collision2D collision )
		{
			m_velocity += collision.relativeVelocity;

			if ( Vector2.Dot( Vector2.up, collision.relativeVelocity ) >= 0.9f )
			{
				m_isGrounded = true;
				m_velocity.y = 0;
			}
		}

		private void OnCollisionStay2D( Collision2D collision )
		{
			if ( m_isGrounded ) { return; }

			m_velocity += collision.relativeVelocity;
		}

		private void Jump()
		{
			m_isJumpDesired = false;
			m_velocity.y += GetJumpForce();

			m_isGrounded = false;
		}

		private float GetJumpForce()
		{
			float gravForce = m_gravityForce * m_rigidbody.gravityScale;
			return Mathf.Sqrt( 2.0f * gravForce * m_jumpHeight );
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