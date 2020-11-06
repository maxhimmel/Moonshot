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
		[SerializeField] private float m_lowGravitationForce = 1.75f;
		[SerializeField] private float m_fullGravitationForce = 3;
		[SerializeField] private float m_gravityFullStrengthRadius = 3;
		[SerializeField] private ForceMode2D m_gravityMode = ForceMode2D.Force;

		[Header( "Movement" )]
		[SerializeField] private float m_acceleration = 10;
		[SerializeField] private float m_maxSpeed = 5;
		[SerializeField] private float m_jumpStrength = 30;

		private Rewired.Player m_input = null;

		private Rigidbody2D m_rigidbody = null;
		private CircleCollider2D m_collider = null;
		
		private Vector3 m_currentDirection = Vector3.up;
		private Vector3 m_prevMoveInput = Vector3.up;
		private float m_currentSpeed = 0;
		private float m_currentAngle = 0;

		private void FixedUpdate()
		{
			// Poll input ...
			Vector3 rawMoveInput = m_input.GetAxis2D( Action.MoveHorizontal, Action.MoveVertical );
			Vector3 clampedMoveInput = Vector3.ClampMagnitude( rawMoveInput, 1 );
			
			// Clamp movement direction relative to gravity direction ...
			Vector3 gravityDir = (m_orbitTarget.position - m_rigidbody.position).normalized;
			Vector3 moveInput = clampedMoveInput.sqrMagnitude <= 0//Vector3.ProjectOnPlane( clampedMoveInput, gravityDir );
				? m_prevMoveInput
				: clampedMoveInput;






			float targetAngle = Vector2.SignedAngle( Vector2.up, moveInput );
			Vector3 dirToTargetAngle = Quaternion.Euler( 0, 0, targetAngle ) * Vector3.up;
			Debug.DrawRay( m_orbitTarget.position, dirToTargetAngle, Color.magenta, 0.1f );


			float currentAngle = Vector2.SignedAngle( Vector2.up, -gravityDir );
			Vector3 dirToCurrentAngle = Quaternion.Euler( 0, 0, currentAngle ) * Vector3.up;
			Debug.DrawRay( m_orbitTarget.position, dirToCurrentAngle, Color.cyan, 0.1f );


			float moveAngle = Mathf.DeltaAngle( currentAngle, targetAngle );
			Vector3 moveDir = Vector3.Cross( Vector3.forward, dirToCurrentAngle ) * System.Math.Sign( moveAngle );
			Debug.DrawRay( m_rigidbody.position, moveDir, Color.green, 0.1f );


			float alignmentStrength = 1 - Mathf.Max( 0, Vector3.Dot( dirToCurrentAngle, dirToTargetAngle ) );
			alignmentStrength = Mathf.Sqrt( alignmentStrength );
			Debug.DrawRay( m_rigidbody.position, moveDir * alignmentStrength, Color.yellow, 0.1f );


			float inputStrength = clampedMoveInput.magnitude;
			Debug.DrawRay( m_rigidbody.position, moveDir * inputStrength, Color.blue, 0.1f );


			float desiredSpeed = alignmentStrength * inputStrength;
			Debug.DrawRay( m_rigidbody.position, moveDir * desiredSpeed, Color.red, 0.1f );



			float speedDelta = m_acceleration * Time.deltaTime;
			m_currentSpeed = Mathf.MoveTowards( m_currentSpeed, desiredSpeed * m_maxSpeed, speedDelta );



			float nextAngle = currentAngle + m_currentSpeed * System.Math.Sign( moveAngle );
			Vector3 dirToNextAngle = Quaternion.Euler( 0, 0, nextAngle ) * Vector3.up;
			Debug.DrawRay( m_orbitTarget.position, dirToNextAngle, Color.white, 0.1f );


			Vector3 nextPos = m_orbitTarget.position
				+ dirToNextAngle.VectorXY() * GetSurfaceRadius()
				+ dirToNextAngle.VectorXY() * GetPlayerRadius()
				+ dirToNextAngle.VectorXY() * m_groundOffset;
			m_rigidbody.MovePosition( nextPos );




			m_prevMoveInput = moveInput;






			//float signedAngle = Vector2.SignedAngle( -gravityDir, clampedMoveInput );
			//int signDir = 0;
			//if ( signedAngle > 0 ) { signDir = 1; }
			//else if ( signedAngle < 0 ) { signDir = -1; }

			////float speedDelta = m_acceleration * Time.deltaTime * Mathf.Sign( signedAngle );
			////float desiredSpeed = m_maxSpeed;
			////m_currentAngle = Mathf.MoveTowards( m_currentAngle, signedAngle, speedDelta );
			//m_currentAngle += (m_acceleration * Time.deltaTime * signDir);
			//m_currentAngle = Mathf.Clamp( m_currentAngle, -m_maxSpeed, m_maxSpeed );
			
			////transform.position = m_orbitTarget.position
			////	+ (Quaternion.Euler( 0, 0, m_currentAngle ) * Vector3.forward).VectorXY() * GetSurfaceRadius()
			////	+ (Quaternion.Euler( 0, 0, m_currentAngle ) * Vector3.forward).VectorXY() * GetPlayerRadius();


			//Vector3 clampedMoveInputNorm = clampedMoveInput.normalized;
			//Vector3 targetDestination = m_orbitTarget.position
			//	+ clampedMoveInputNorm.VectorXY() * GetSurfaceRadius()
			//	+ clampedMoveInputNorm.VectorXY() * GetPlayerRadius();

			//Debug.DrawLine( m_orbitTarget.position, targetDestination, Color.cyan, 0.1f );
			


			//Vector3 dir = Quaternion.Euler( 0, 0, m_currentAngle ) * -gravityDir;
			//Debug.DrawRay( m_orbitTarget.position, dir, Color.magenta, 0.1f );




			//if ( clampedMoveInput.sqrMagnitude <= 0 )
			//{
			//	clampedMoveInput = m_prevDirection;
			//}


			////// DEBUG ...
			////Debug.DrawRay( transform.position, clampedMoveInput, Color.white, 3 );
			////Debug.DrawRay( transform.position, moveInput, Color.green, 3 );
			////Debug.DrawRay( transform.position, m_velocity.normalized, Color.red, 3 );


			////// Acceleration ...
			////float speedDelta = m_acceleration * Time.deltaTime;
			////Vector3 desiredVelocity = moveInput * m_maxSpeed;
			////m_velocity = new Vector3()
			////{
			////	x = Mathf.MoveTowards( m_velocity.x, desiredVelocity.x, speedDelta ),
			////	y = Mathf.MoveTowards( m_velocity.y, desiredVelocity.y, speedDelta )
			////};

			//// Acceleration ...
			//float speedDelta = m_acceleration * Time.deltaTime;
			//float desiredSpeed = Mathf.Max( 0, 1 - Vector3.Dot( clampedMoveInput.normalized, m_currentDirection.normalized ) );
			//desiredSpeed *= m_maxSpeed;

			//m_currentSpeed = Mathf.MoveTowards( m_currentSpeed, desiredSpeed, speedDelta );
			//m_currentDirection = Vector3.RotateTowards( m_currentDirection, clampedMoveInput, m_currentSpeed * Mathf.Deg2Rad, 0 );

			//Debug.DrawRay( m_orbitTarget.position, m_currentDirection, Color.red, 0.1f );

			////// NOT ideal, see below ...
			////transform.position = m_orbitTarget.position
			////	+ m_currentDirection.VectorXY() * GetSurfaceRadius()
			////	+ m_currentDirection.VectorXY() * GetPlayerRadius();

			//// Desired, BUT the m_currentDirection when clampedMoveInput is zero needs fixing!
			//m_rigidbody.MovePosition( m_orbitTarget.position
			//	+ m_currentDirection.VectorXY() * GetSurfaceRadius()
			//	+ m_currentDirection.VectorXY() * GetPlayerRadius()
			//	+ m_currentDirection.VectorXY() * 0.15f // offset so it's not directly colliding with orbitTarget
			//);


			////Quaternion currentToDesired = Quaternion.FromToRotation( m_currentDirection, clampedMoveInput );
			////m_currentDirection = currentToDesired * Vector3.forward;

			////Debug.DrawRay( m_orbitTarget.position, m_currentDirection, Color.red, 0.1f );



			////// Gravity ...
			////if ( !IsGrounded() )
			////{
			////	float gravityForce = (m_rigidbody.position - m_orbitTarget.position).sqrMagnitude <= m_gravityFullStrengthRadius * m_fullGravitationForce
			////		? m_fullGravitationForce
			////		: m_lowGravitationForce;

			////	m_rigidbody.AddForce( gravityDir * gravityForce );
			////}


			////// Jump ...
			////if ( IsGrounded() )
			////{
			////	if ( m_input.GetButtonDown( Action.Jump ) )
			////	{
			////		m_velocity -= gravityDir * m_jumpStrength;

			////		m_orbitTarget.AddForceAtPosition( gravityDir * m_jumpStrength, m_rigidbody.position, ForceMode2D.Impulse );
			////	}
			////}


			////m_rigidbody.velocity = m_velocity;




			////Vector3 rawMoveInput = m_input.GetAxis2D( Action.MoveHorizontal, Action.MoveVertical );
			////Vector3 moveInput = rawMoveInput.normalized;

			////if ( moveInput.sqrMagnitude <= 0 )
			////{
			////	moveInput = m_prevDirection;
			////}

			//////float upAngle = Vector2.SignedAngle( Vector2.up, moveInput );

			////float speedDelta = (m_acceleration * Mathf.Deg2Rad) * Time.deltaTime;
			////m_currentDirection = Vector3.RotateTowards( m_currentDirection, moveInput, speedDelta, 1 );


			////transform.position = m_orbitTarget.position 
			////	+ m_currentDirection * GetSurfaceRadius() 
			////	+ m_currentDirection * GetPlayerRadius();

			//m_prevDirection = m_currentDirection;
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

		private bool IsGrounded()
		{
			const float distThreshold = 0.1f;
			float minDist = GetSurfaceRadius() + GetPlayerRadius() + distThreshold;

			return (m_orbitTarget.position - m_rigidbody.position).sqrMagnitude < minDist * minDist;
		}

		private void Update()
		{
			SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
			renderer.color = IsGrounded() ? Color.red : Color.white;
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


#if UNITY_EDITOR
		[Header( "Editor / Tools" )]
		[SerializeField] private Color m_fullStrengthRadiusColor = Color.blue;

		private void OnDrawGizmosSelected()
		{
			using ( new UnityEditor.Handles.DrawingScope( m_fullStrengthRadiusColor ) )
			{
				UnityEditor.Handles.DrawWireDisc( m_orbitTarget.position, Vector3.back, m_gravityFullStrengthRadius );
			}
		}
#endif
	}
}