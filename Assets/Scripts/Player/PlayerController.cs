using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Xam.Utility.Extensions;
using Xam.Initialization;

namespace Moonshot.Gameplay.Player
{
	using ReConsts;
	using Pickups;

	[SelectionBase]
	public class PlayerController : MonoBehaviour
	{
		private readonly int k_isRunningAnimParam = Animator.StringToHash( "IsRunning" );
		private readonly int k_isGroundedAnimParam = Animator.StringToHash( "IsGrounded" );
		private readonly int k_apexReachedAnimParam = Animator.StringToHash( "ApexReached" );
		private readonly int k_jumpAnimParam = Animator.StringToHash( "Jump" );

		private Vector2 RelativeRight { get { return m_orbitTarget.transform.right; } }

		[Header( "Orbit" )]
		[SerializeField] private Rigidbody2D m_orbitTarget = default;
		[SerializeField] private float m_gravityForce = 2;
		[SerializeField] private float m_groundOffset = 0.1f;

		[Header( "Movement" )]
		[SerializeField] private bool m_beginGrounded = true;
		[SerializeField] private float m_acceleration = 10;
		[SerializeField] private float m_maxSpeed = 5;
		[SerializeField] private float m_maxAirSpeed = 1;

		[Header( "Jumping" )]
		[SerializeField] private float m_jumpHeight = 3;
		[SerializeField] private float m_longJumpGravityScale = 1;
		[SerializeField] private float m_shortJumpGravityScale = 2;

		[Header( "Orbit Collision" )]
		[SerializeField] private float m_jumpPushForce = 10;
		[SerializeField] private float m_landMinPushForce = 2;
		[SerializeField] private float m_landMaxPushForce = 10;
		[SerializeField] private float m_landTorque = 30;

		private Rewired.Player m_input = null;
		private IMoveInterpreter m_moveInterpreter = new RelativeMoveInterpreter();

		private Rigidbody2D m_rigidbody = null;
		private CircleCollider2D m_collider = null;

		private Animator m_animator = null;

		private ICollector m_starCollector = null;
		
		private Vector3 m_gravityNormal = Vector3.up;
		private float m_currentAngle = 0;
		private float m_currentSpeed = 0;
		private float m_currentGravity = 0;
		private float m_currentJumpForce = 0;
		private int m_currentFacingDir = -1;
		private float m_prevJumpForce = 0;
		private Vector3 m_desiredMoveDirection = Vector3.zero;
		private bool m_isJumpDesired = false;
		private bool m_isLongJumping = false;
		private bool m_isGrounded = false;
		private float m_nextLandPushForce = 0;

		public void SetMoveInterpreter( IMoveInterpreter newInterpreter )
		{
			m_moveInterpreter = newInterpreter;
		}

		private void Update()
		{
			if ( m_input != null )
			{
				Vector3 moveInput = GetMoveInput();
				SetMoveDirection( moveInput );

				if ( m_input.GetButtonDown( Action.Jump ) )
				{
					TryJump();
				}
				else if ( m_input.GetButtonUp( Action.Jump ) )
				{
					StopJumping();
				}

				if ( m_input.GetButtonDown( Action.Pause ) )
				{
					GameMode.Instance.TogglePause();
				}
			}

			UpdateAnimator();
		}

		private Vector3 GetMoveInput()
		{
			if ( m_moveInterpreter == null ) { return Vector3.zero; }

			Vector2 rawMoveInput = m_input.GetAxis2D( Action.MoveHorizontal, Action.MoveVertical );
			Vector2 clampedMoveInput = Vector2.ClampMagnitude( rawMoveInput, 1 );

			return m_moveInterpreter.Interpret( clampedMoveInput, m_gravityNormal );
		}

		private void SetMoveDirection( Vector3 moveDirection )
		{
			m_desiredMoveDirection = moveDirection;
		}

		private bool TryJump()
		{
			if ( !m_isGrounded ) { return false; }

			m_isJumpDesired = true;
			m_isLongJumping = true;

			m_rigidbody.gravityScale = m_longJumpGravityScale;

			return true;
		}

		private void StopJumping()
		{
			if ( !m_isLongJumping ) { return; }

			m_isLongJumping = false;

			m_rigidbody.gravityScale = m_shortJumpGravityScale;
		}

		private void UpdateAnimator()
		{
			bool isRunning = !Mathf.Approximately( m_currentSpeed, 0 );
			m_animator.SetBool( k_isRunningAnimParam, isRunning );

			m_animator.SetBool( k_isGroundedAnimParam, m_isGrounded );
		}

		private void FixedUpdate()
		{
			UpdateState();

			Accelerate();
			ApplyAngleVelocity();

			if ( m_isJumpDesired )
			{
				Jump();
			}

			ApplyGravity();
			ApplyFinalMovement();

			ApplyRotation();
		}

		private void UpdateState()
		{
			m_gravityNormal = Quaternion.Euler( 0, 0, m_currentAngle ) * RelativeRight;

			if ( !Mathf.Approximately( m_currentSpeed, 0 ) )
			{
				m_currentFacingDir = System.Math.Sign( m_currentSpeed );
			}

			bool isJumpApexReached = m_prevJumpForce > 0 && m_currentJumpForce <= 0;
			if ( isJumpApexReached )
			{
				OnJumpApexReached();
			}

			bool wasPreviouslyGrounded = m_isGrounded;
			m_isGrounded = m_currentGravity <= 0;

			if ( !wasPreviouslyGrounded && m_isGrounded )
			{
				OnLanded();
			}
		}

		private void OnJumpApexReached()
		{
			m_nextLandPushForce = Mathf.Lerp( m_landMinPushForce, m_landMaxPushForce, m_currentGravity / m_jumpHeight );

			m_isLongJumping = false;
			m_rigidbody.gravityScale = m_longJumpGravityScale;

			m_animator.SetTrigger( k_apexReachedAnimParam );
		}

		private void OnLanded()
		{
			float normalizedSpeed = m_currentSpeed / m_maxSpeed;
			float landTorque = m_landTorque * normalizedSpeed;
			SpinOrbitTarget( landTorque );

			PushOrbitTarget( -m_gravityNormal * m_nextLandPushForce );
		}

		private void Accelerate()
		{
			float targetAngle = Vector2.SignedAngle( RelativeRight, m_desiredMoveDirection );
			int moveDir = System.Math.Sign( Mathf.DeltaAngle( m_currentAngle, targetAngle ) );
			
			float speedDelta = m_acceleration * Time.deltaTime;
			float maxSpeed = moveDir * Mathf.Lerp( m_maxSpeed, m_maxAirSpeed, m_currentGravity / m_jumpHeight );
			
			m_currentSpeed = Mathf.MoveTowards( m_currentSpeed, m_desiredMoveDirection.magnitude * maxSpeed, speedDelta );
		}

		private void ApplyAngleVelocity()
		{
			m_currentAngle += m_currentSpeed;
		}

		private void Jump()
		{
			m_isJumpDesired = false;

			float gravForce = m_gravityForce * m_rigidbody.gravityScale;
			m_currentJumpForce = Mathf.Sqrt( 2.0f * gravForce * m_jumpHeight );

			PushOrbitTarget( -m_gravityNormal * m_jumpPushForce );

			m_animator.SetTrigger( k_jumpAnimParam );
		}

		private void ApplyGravity()
		{
			m_prevJumpForce = m_currentJumpForce;

			float gravDelta = m_gravityForce * m_rigidbody.gravityScale * Time.deltaTime;

			m_currentGravity += m_currentJumpForce * Time.deltaTime;
			m_currentJumpForce -= gravDelta;

			m_currentGravity -= gravDelta * Time.deltaTime;
			m_currentGravity = Mathf.Max( 0, m_currentGravity );
		}

		private void ApplyFinalMovement()
		{
			m_rigidbody.MovePosition( GetOrbitPlacement() );
		}

		private Vector2 GetOrbitPlacement()
		{
			float distFromOrbitCenter = GetSurfaceRadius() + GetPlayerRadius() + m_groundOffset + m_currentGravity;
			float angle2Rad = (m_currentAngle + m_orbitTarget.rotation) * Mathf.Deg2Rad;

			Vector2 dirFromOrbit = new Vector2()
			{
				x = distFromOrbitCenter * Mathf.Cos( angle2Rad ),
				y = distFromOrbitCenter * Mathf.Sin( angle2Rad )
			};

			return m_orbitTarget.position + dirFromOrbit;
		}

		private float GetSurfaceRadius()
		{
			CircleCollider2D orbitCollider = m_orbitTarget.GetComponentInChildren<CircleCollider2D>();
			return orbitCollider.radius;
		}

		private float GetPlayerRadius()
		{
			return m_collider.radius;
		}

		private void ApplyRotation()
		{
			Vector3 facingDirection = m_currentFacingDir * Vector3.back;
			Quaternion facing = Quaternion.LookRotation( facingDirection, m_gravityNormal );

			transform.rotation = facing;
		}

		private void PushOrbitTarget( Vector3 force )
		{
			m_orbitTarget.AddForce( force, ForceMode2D.Impulse );
		}

		private void SpinOrbitTarget( float torque )
		{
			m_orbitTarget.AddTorque( torque, ForceMode2D.Force );
		}

		private void OnCollisionEnter2D( Collision2D collision )
		{
			IPickup pickup = collision.rigidbody?.GetComponent<IPickup>();
			if ( pickup != null )
			{
				m_starCollector.Collect( pickup );
			}
		}

		private IEnumerator Start()
		{
			Vector2 orbitToPlayer = m_rigidbody.position - m_orbitTarget.position;
			m_currentAngle = Vector2.SignedAngle( RelativeRight, orbitToPlayer );

			if ( m_beginGrounded )
			{
				ApplyFinalMovement();
				m_isGrounded = true;
			}

			while ( !LevelInitializer.IsInitialized ) { yield return null; }

			m_input = ReInput.players.GetPlayer( 0 );
			SetMoveInterpreter( PlayerSettings.Instance.CurrentMovement );
		}

		private void Awake()
		{
			m_rigidbody = GetComponent<Rigidbody2D>();
			m_collider = GetComponentInChildren<CircleCollider2D>();

			m_animator = GetComponentInChildren<Animator>();

			m_starCollector = GetComponentInChildren<StarCollector>();
		}
	}
}