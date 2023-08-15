//    Sprite based SingleSegment


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	internal class SingleSegmentSprite : SingleSegment {
		
		private SpriteRenderer spriteRenderer;
		private Vector3 defaultPosition;
		
		internal SingleSegmentSprite(SegmentDisplaySprite parentDisplay, SpriteRenderer spriteRenderer) : base(parentDisplay) {
			this.spriteRenderer=spriteRenderer;
            spriteRenderer.sortingOrder = parentDisplay.SpriteSortingOrder;
			defaultPosition=spriteRenderer.transform.localPosition;
			base.SetState(false);
		}
		
		internal void multiplyPosition(float m) {
			spriteRenderer.transform.localPosition=defaultPosition*m;
		}

        internal void setSortingOrder(int sortingOrder) {
            spriteRenderer.sortingOrder = sortingOrder;
        }

        protected override void setColor(Color color) {
			spriteRenderer.color = color;
		}
		
	}
	
}
