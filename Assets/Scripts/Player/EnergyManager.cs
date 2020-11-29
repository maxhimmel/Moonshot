using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;
using Xam.Initialization;

namespace Moonshot.Gameplay
{
	using Player;

	public class EnergyManager : SingletonMono<EnergyManager>
	{
		public event System.Action<EnergyManager> OnEnergyAmountChangedEvent;
		public event System.Action<EnergyManager> OnEnergyDepletedEvent;

		public float CurrentEnergyRatio { get { return CurrentEnergy / m_maxEnergy; } }
		public float CurrentEnergy { get; private set; } = 0;
		public float MaxEnergy { get { return m_maxEnergy; } }

		[Header( "Energy" )]
		[SerializeField] private float m_maxEnergy = 30;
		[SerializeField] private float m_energyDecaySpeed = 1;

		[Space]
		[SerializeField] private float m_deathDuration = 3;
		[SerializeField] private float m_deathDetachForce = 5;

		private Coroutine m_playerLoseRoutine = null;

		private void Update()
		{
			if ( LevelInitializer.IsInitialized )
			{
				float decayDelta = m_energyDecaySpeed * Time.deltaTime;
				AddEnergy( -decayDelta );
			}
		}

		public void AddEnergy( float energy )
		{
			float prevEnergy = CurrentEnergy;

			CurrentEnergy += energy;
			CurrentEnergy = Mathf.Clamp( CurrentEnergy, 0, m_maxEnergy );
			

			if ( prevEnergy != CurrentEnergy )
			{
				OnEnergyAmountChangedEvent?.Invoke( this );

				if ( CurrentEnergy <= 0 )
				{
					OnEnergyDepletedEvent?.Invoke( this );
					BeginPlayerLoseSequence();
				}
			}
		}

		private void BeginPlayerLoseSequence()
		{
			m_playerLoseRoutine = StartCoroutine( PlayerLoseSequence_Coroutine() );
		}

		private IEnumerator PlayerLoseSequence_Coroutine()
		{
			PlayerController player = DynamicPool.Instance.GetFirstPooledObjectByType<PlayerController>();

			player.SetGameplayControlsActive( false );
			player.SetFreefallActive( true );
			player.PlayDeathAnim();

			if ( player.IsGrounded )
			{
				player.Body.AddForce( player.GravityNormal * m_deathDetachForce, ForceMode2D.Impulse );
			}

			float timer = 0;
			while ( timer < 1 )
			{
				timer += Time.deltaTime / m_deathDuration;

				yield return null;
			}

			GameMode.Instance.Lose();
		}

		protected override void Awake()
		{
			base.Awake();

			AddEnergy( m_maxEnergy );
		}
	}
}