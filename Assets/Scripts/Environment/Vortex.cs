using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Gameplay;

namespace Moonshot.Gameplay
{
	using Player;

	public class Vortex : ProximityBucket2D<PlayerController>
	{
		[Header( "Vortex" )]
		[SerializeField] private float m_pullForce = 20;
		[SerializeField] private float m_minPullForce = 10;
		[SerializeField] private float m_pullDuration = 5;

		private CircleCollider2D m_trigger = null;
		private Rigidbody2D m_vortexBody = null;

		private bool m_isPlayerGathered = false;
		private Coroutine m_playerWinRoutine = null;

		protected override void OnTargetEntered( PlayerController target )
		{
			base.OnTargetEntered( target );

			if ( EnergyManager.Exists )
			{
				EnergyManager.Instance.enabled = false;
			}

			m_isPlayerGathered = true;
			BeginPlayerWinSequence( target );
		}

		private void BeginPlayerWinSequence( PlayerController player )
		{
			m_playerWinRoutine = StartCoroutine( PlayerWinSequence_Coroutine( player ) );
		}

		private IEnumerator PlayerWinSequence_Coroutine( PlayerController player )
		{
			player.SetGameplayControlsActive( false );
			player.SetFreefallActive( true );
			player.PlayWinAnim();

			float timer = 0;
			while ( timer < 1 )
			{
				timer += Time.deltaTime / m_pullDuration;
				timer = Mathf.Min( 1, timer );

				Vector2 player2Vortex = (m_vortexBody.position - player.Body.position);
				float dist = player2Vortex.magnitude;
				float normalizedDist = dist / m_trigger.radius;
				Vector2 pullDir = player2Vortex / dist;

				PullPlayer( player, pullDir, normalizedDist );
				ScalePlayer( player, pullDir, normalizedDist );

				yield return null;
			}

			GameMode.Instance.Win();
		}

		private void PullPlayer( PlayerController player, Vector2 pullDirection, float normalizedDist )
		{
			float pullScalar = (1 - normalizedDist) * (1 - normalizedDist);
			float pullForce = Mathf.Max( m_minPullForce, m_pullForce * pullScalar );

			player.Body.AddForce( pullDirection * pullForce );
		}

		private void ScalePlayer( PlayerController player, Vector2 pullDirection, float normalizedDist )
		{
			Vector2 localScaleDir = player.transform.InverseTransformDirection( pullDirection );
			player.transform.up = -pullDirection;
			player.transform.localScale = -localScaleDir * normalizedDist + Vector2.right;
		}

		protected override bool CanBeGathered( Collider2D collider )
		{
			if ( m_isPlayerGathered ) { return false; }

			return base.CanBeGathered( collider );
		}

		protected override void Awake()
		{
			base.Awake();

			m_vortexBody = GetComponent<Rigidbody2D>();
			m_trigger = GetComponentInChildren<CircleCollider2D>();
		}
	}
}