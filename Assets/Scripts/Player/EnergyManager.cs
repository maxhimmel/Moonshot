using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xam.Utility.Patterns;
using Xam.Initialization;

namespace Moonshot.Gameplay
{
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
					GameMode.Instance.Lose();
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();

			AddEnergy( m_maxEnergy );
		}
	}
}