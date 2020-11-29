using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xam.Utility.Patterns;

namespace Moonshot.Gameplay.MiniMap
{
	public class MiniMapMarkerManager : MonoBehaviour
	{
		[SerializeField] private Camera m_miniMapCamera = default;
		[SerializeField] private RectTransform m_markerContainer = default;
		[SerializeField] private float m_maskRadius = 60;
		[SerializeField] private float m_fadeRadius = 45;

		private Dictionary<MiniMapWorldMarker, Image> m_markers = new Dictionary<MiniMapWorldMarker, Image>();

		private void Update()
		{
			IEnumerable<MiniMapWorldMarker> worldMarkers = DynamicPool.Instance.GetPooledObjectsByType<MiniMapWorldMarker>();
			if ( worldMarkers == null ) { return; }

			Rect containerRect = m_markerContainer.rect;
			float relativeAspect = m_miniMapCamera.pixelHeight / containerRect.size.y;

			Vector2 centerOffset = containerRect.size / 2f;

			foreach ( MiniMapWorldMarker marker in worldMarkers )
			{
				if ( marker == null ) { continue; }

				Image element = CreateMarkerElement( marker );
				if ( element == null ) { continue; }

				RectTransform elementTrans = element.rectTransform;

				Vector3 markerWorldPos = marker.GetWorldPosition();
				Vector3 markerScreenPos = m_miniMapCamera.WorldToScreenPoint( markerWorldPos );

				Vector2 newPos = markerScreenPos / relativeAspect;
				Vector2 center2NewPos = newPos - centerOffset;

				float distFromCenter = center2NewPos.magnitude;
				if ( distFromCenter >= m_maskRadius )
				{
					newPos = centerOffset + center2NewPos.normalized * m_maskRadius;
				}

				elementTrans.up = center2NewPos;
				elementTrans.anchoredPosition = newPos;

				CanvasGroup elementCanvasGroup = element.GetComponent<CanvasGroup>();

				float maxDist = m_maskRadius - m_fadeRadius;
				float distOffset = Mathf.Clamp( distFromCenter - m_fadeRadius, 0, maxDist );
				elementCanvasGroup.alpha = Mathf.Lerp( 0, 1, distOffset / maxDist );
			}
		}

		private Image CreateMarkerElement( MiniMapWorldMarker marker )
		{
			if ( marker == null ) { return null; }

			if ( m_markers.TryGetValue( marker, out Image existingMarker ) )
			{
				return existingMarker;
			}

			Sprite sprite = marker.GetMarker();
			if ( sprite == null ) { return null; }

			GameObject newObj = new GameObject( $"Marker_{sprite.name}" );
			newObj.transform.SetParent( m_markerContainer, false );

			newObj.AddComponent<CanvasGroup>();

			Image newImage = newObj.AddComponent<Image>();
			newImage.sprite = sprite;
			newImage.SetNativeSize();

			RectTransform newRect = newImage.rectTransform;
			newRect.anchorMin = newRect.anchorMax = Vector2.zero;

			m_markers.Add( marker, newImage );

			return newImage;
		}


#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			UnityEditor.Handles.color = Color.white;
			UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.forward, m_maskRadius );

			UnityEditor.Handles.color = Color.yellow;
			UnityEditor.Handles.DrawWireDisc( transform.position, Vector3.forward, m_fadeRadius );
		}
#endif	
	}
}