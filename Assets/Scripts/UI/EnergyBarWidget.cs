using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Moonshot.Gameplay.UI
{
	public class EnergyBarWidget : MonoBehaviour
	{
		[SerializeField] private Image m_energyFillElement = default;

		private void Start()
		{
			Debug.Assert( EnergyManager.Exists, $"No EnergyManager found in scene!\n" +
				$"Widget will not update.", this );

			EnergyManager.Instance.OnEnergyAmountChangedEvent += OnEnergyAmountChanged;
		}

		private void OnEnergyAmountChanged( EnergyManager energyManager )
		{
			m_energyFillElement.fillAmount = energyManager.CurrentEnergyRatio;
		}
	}
}