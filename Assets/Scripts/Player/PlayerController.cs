using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Xam.Utility.Extensions;

namespace Moonshot.Gameplay.Player
{
	using ReConsts;

	[SelectionBase]
	public class PlayerController : MonoBehaviour
	{
		[Header( "Orbit" )]
		[SerializeField] private Rigidbody2D m_orbitTarget = default;
		[SerializeField] private float m_gravityForce = 2;

		[Header( "Movement" )]
		[SerializeField] private float m_acceleration = 10;
		[SerializeField] private float m_maxSpeed = 5;
		[SerializeField] private float m_jumpHeight = 3;
		[SerializeField] private float m_groundedProbeLength = 0.2f;
		[SerializeField] private LayerMask m_groundedProbeLayer = 0;

		private Rewired.Player m_input = null;

		private Rigidbody2D m_rigidbody = null;
		private CircleCollider2D m_collider = null;
		
		private Vector3 m_velocity = Vector3.zero;
		private Vector3 m_desiredVelocity = Vector3.zero;
		private Vector3 m_gravityNormal = Vector3.up;
		private bool m_isGrounded = false;
		private bool m_isJumpDesired = false;

		private void Update()
		{
			SetDesiredVelocity( GetMoveInput() );

			if ( m_input.GetButtonDown( Action.Jump ) )
			{
				TryJump();
			}
		}

		private void SetDesiredVelocity( Vector3 moveDirection )
		{
			m_desiredVelocity = moveDirection * m_maxSpeed;
		}

		private bool TryJump()
		{
			if ( !m_isGrounded ) { return false; }

			m_isJumpDesired = true;
			return true;
		}

		private Vector3 GetMoveInput()
		{
			Vector2 rawMoveInput = m_input.GetAxis2D( Action.MoveHorizontal, Action.MoveVertical );
			Vector2 clampedMoveInput = Vector2.ClampMagnitude( rawMoveInput, 1 );
			Vector3 moveInput = Vector3.ProjectOnPlane( clampedMoveInput, m_gravityNormal ).normalized * clampedMoveInput.magnitude;

			return moveInput;
		}

		private void FixedUpdate()
		{
			UpdateState();

			Accelerate();	
			
			if ( m_isJumpDesired )
			{
				Jump();
			}

			ApplyGravity();
			ApplyVelocity();
		}

		private void UpdateState()
		{
			m_velocity = m_rigidbody.velocity;
			m_gravityNormal = GetGravityNormal();
			m_isGrounded = Physics2D.Raycast( m_rigidbody.position, -GetGravityNormal(), m_groundedProbeLength, m_groundedProbeLayer );
		}

		private Vector3 GetGravityNormal()
		{
			return (m_rigidbody.position - m_orbitTarget.position).normalized;
		}

		private void Accelerate()
		{
			Vector3 xAxis = Vector3.ProjectOnPlane( Vector3.right, m_gravityNormal ).normalized;
			Vector3 yAxis = Vector3.ProjectOnPlane( Vector3.up, m_gravityNormal ).normalized;

			float currentX = Vector3.Dot( m_velocity, xAxis );
			float currentY = Vector3.Dot( m_velocity, yAxis );

			float speedDelta = m_acceleration * Time.deltaTime;
			float newX = Mathf.MoveTowards( currentX, m_desiredVelocity.x, speedDelta );
			float newY = Mathf.MoveTowards( currentY, m_desiredVelocity.y, speedDelta );

			m_velocity += xAxis * (newX - currentX) + yAxis * (newY - currentY);
		}

		private void ApplyGravity()
		{
			float gravForce = m_gravityForce * m_rigidbody.gravityScale;
			m_velocity -= m_gravityNormal * gravForce * Time.deltaTime;
		}

		private void ApplyVelocity()
		{
			m_rigidbody.velocity = m_velocity;
		}

		private void Jump()
		{
			m_isJumpDesired = false;

			float jumpForce = Mathf.Sqrt( 2f * m_gravityForce * m_jumpHeight );
			m_velocity += GetGravityNormal() * jumpForce;
		}

		private void Start()
		{
			m_input = ReInput.players.GetPlayer( 0 );
		}

		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody2D>();
			m_collider = GetComponentInChildren<CircleCollider2D>();
		}
	}
}