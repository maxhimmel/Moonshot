using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Moonshot.Gameplay.UI
{
	public class SelectMarkerElement : MonoBehaviour
	{
		[SerializeField] private Transform m_parentOverride = default;

		private CanvasGroup m_canvasGroup = null;

		private void Awake()
		{
			m_canvasGroup = GetComponent<CanvasGroup>();
			Hide( null );

			Transform parent = (m_parentOverride != null) ? m_parentOverride : transform.parent;
			Debug.Assert( parent != null, $"SelectMarkerElement requires a parent!", this );

			EventTrigger parentTrigger = parent.gameObject.AddComponent<EventTrigger>();

			//parentTrigger.triggers.Add( CreateEvent( EventTriggerType.PointerEnter, Show ) );
			//parentTrigger.triggers.Add( CreateEvent( EventTriggerType.PointerExit, Hide ) );

			parentTrigger.triggers.Add( CreateEvent( EventTriggerType.Select, Show ) );
			parentTrigger.triggers.Add( CreateEvent( EventTriggerType.Deselect, Hide ) );
		}

		private EventTrigger.Entry CreateEvent( EventTriggerType type, UnityAction<BaseEventData> @event )
		{
			EventTrigger.Entry newEvent = new EventTrigger.Entry();
			newEvent.eventID = type;
			newEvent.callback.AddListener( @event );

			return newEvent;
		}

		private void Show( BaseEventData data )
		{
			m_canvasGroup.alpha = 1;
		}

		private void Hide( BaseEventData data )
		{
			m_canvasGroup.alpha = 0;
		}
	}
}