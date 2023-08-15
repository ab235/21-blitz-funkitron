//    Image based SingleSegment


using UnityEngine;
using UnityEngine.UI;

namespace Leguar.SegmentDisplay {
	
	internal class SingleSegmentImage : SingleSegment {
		
		private Image image;
		private RectTransform rectTransform;
		private Vector3 originalPosition;
		private Vector2 originalSize;

		internal SingleSegmentImage(SegmentDisplay parentDisplay, Image image) : base(parentDisplay) {
			this.image = image;
			rectTransform = image.transform.GetComponent<RectTransform>();
			originalPosition = rectTransform.anchoredPosition3D;
			originalSize = rectTransform.sizeDelta;
#if UNITY_2017_1_OR_NEWER
			image.raycastTarget=false;
#endif
			base.SetState(false);
		}
		
		protected override void setColor(Color color) {
			image.color=color;
		}
		
		internal void spreadAndMultiplySize(float spread, float multipX, float multipY) {

			Vector3 pos = rectTransform.anchoredPosition3D;
			pos.x = originalPosition.x * spread * multipX;
			pos.y = originalPosition.y * spread * multipY;
			pos.z = originalPosition.z * spread;
			rectTransform.anchoredPosition3D = pos;

			Vector2 size = rectTransform.sizeDelta;
			size.x = originalSize.x * multipX;
			size.y = originalSize.y * multipY;
			rectTransform.sizeDelta = size;

		}

	}
	
}
