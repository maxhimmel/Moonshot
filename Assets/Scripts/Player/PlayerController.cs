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
		[SerializeField] private float m_groundOffset = 0.15f;
		[SerializeField] private float m_gravityForce = 2;

		[Header( "Movement" )]
		[SerializeField] private float m_acceleration = 10;
		[SerializeField] private float m_maxSpeed = 5;
		[SerializeField] private float m_jumpStrength = 8;

		private Rewired.Player m_input = null;

		private Rigidbody2D m_rigidbody = null;
		private CircleCollider2D m_collider = null;

		private Vector3 m_desiredDirection = Vector3.up;
		private Vector3 m_prevDirection = Vector3.up;
		private float m_currentSpeed = 0;
		
		private void Update()
		{
			SetDesiredDirection( GetMoveInput() );
		}

		private void SetDesiredDirection( Vector3 direction )
		{
			m_desiredDirection = direction;
		}

		private Vector3 GetMoveInput()
		{
			Vector3 rawInput = m_input.GetAxis2D( Action.MoveHorizontal, Action.MoveVertical );
			return Vector3.ClampMagnitude( rawInput, 1 );
		}

		private void FixedUpdate()
		{
			Accelerate();
			ApplyMovement();

			UpdateState();
		}

		private Vector3 GetCurrentDirection()
		{
			float currentAngle = Vector2.SignedAngle( Vector2.up, -GetGravityDirection() );
			Vector3 dirToCurrentAngle = Quaternion.Euler( 0, 0, currentAngle ) * Vector3.up;
			//Debug.DrawRay( m_orbitTarget.position, dirToCurrentAngle, Color.cyan, 0.1f );

			return dirToCurrentAngle;
		}

		private Vector3 GetGravityDirection()
		{
			return (m_orbitTarget.position - m_rigidbody.position).normalized;
		}

		private Vector3 GetTargetDirection()
		{
			Vector3 nonEmptyMoveInput = (m_desiredDirection.sqrMagnitude <= 0)
				? m_prevDirection
				: m_desiredDirection;
			
			float targetAngle = Vector2.SignedAngle( Vector2.up, nonEmptyMoveInput );
			Vector3 dirToTargetAngle = Quaternion.Euler( 0, 0, targetAngle ) * Vector3.up;
			//Debug.DrawRay( m_orbitTarget.position, dirToTargetAngle, Color.magenta, 0.1f );

			return dirToTargetAngle;
		}

		private void Accelerate()
		{
			float speedDelta = m_acceleration * Time.deltaTime;
			m_currentSpeed = Mathf.MoveTowards( m_currentSpeed, GetDesiredSpeed(), speedDelta );
		}

		private float GetDesiredSpeed()
		{
			Vector3 dirToCurrentAngle = GetCurrentDirection();
			Vector3 dirToTargetAngle = GetTargetDirection();


			float rawAlignmentStrength = 1 - Mathf.Max( 0, Vector3.Dot( dirToCurrentAngle, dirToTargetAngle ) );
			float alignmentStrength = Mathf.Sqrt( rawAlignmentStrength );

			float inputStrength = m_desiredDirection.magnitude;
			return m_maxSpeed * (alignmentStrength * inputStrength);
		}

		private void ApplyMovement()
		{
			Vector3 dirToCurrentAngle = GetCurrentDirection();
			Vector3 dirToTargetAngle = GetTargetDirection();


			float moveAngle = Vector2.SignedAngle( dirToCurrentAngle, dirToTargetAngle );
			float angleDelta = m_currentSpeed * System.Math.Sign( moveAngle );
			Vector2 dirToNextAngle = Quaternion.AngleAxis( angleDelta, Vector3.forward ) * dirToCurrentAngle;
			
			Vector2 nextPos = m_orbitTarget.position
				+ dirToNextAngle * GetSurfaceRadius()
				+ dirToNextAngle * GetPlayerRadius()
				+ dirToNextAngle * m_groundOffset;

			m_rigidbody.MovePosition( nextPos );
		}

		private float GetSurfaceRadius()
		{
			CircleCollider2D collider = m_orbitTarget.GetComponentInChildren<CircleCollider2D>();
			if ( collider == null ) { return -1; }

			return collider.radius;
		}

		private float GetPlayerRadius()
		{
			return m_collider.radius;
		}

		private void UpdateState()
		{
			m_prevDirection = (m_desiredDirection.sqrMagnitude <= 0)
				? m_prevDirection
				: m_desiredDirection;
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