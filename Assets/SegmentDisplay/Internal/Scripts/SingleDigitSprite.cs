//    Class for sprite based segment digit


using UnityEngine;

namespace Leguar.SegmentDisplay {

	public class SingleDigitSprite : SingleDigit {

		public SpriteRenderer[] segmentSprites;
		public SpriteRenderer decimalPointSprite;
		public SpriteRenderer apostropheSprite;
		public SpriteRenderer colonLowerSprite;
		public SpriteRenderer colonUpperSprite;

		public override int BaseSegmentCount {
			get {
				return segmentSprites.Length;
			}
		}

		internal override void createSegments() {

            SegmentDisplaySprite parentSpriteDisplay = (SegmentDisplaySprite)(base.parentDisplay);

			if (base.mode==Mode.Digit) {

				for (int n = 0; n<BaseSegmentCount; n++) {
					segments[n]=new SingleSegmentSprite(parentSpriteDisplay, segmentSprites[n]);
				}

				if (base.decimalPointEnabled) {
					segments[BaseSegmentCount]=new SingleSegmentSprite(parentSpriteDisplay, decimalPointSprite);
				} else {
					decimalPointSprite.gameObject.SetActive(false);
				}

				if (base.apostropheEnabled) {
					segments[BaseSegmentCount+(base.decimalPointEnabled ? 1 : 0)]=new SingleSegmentSprite(parentSpriteDisplay, apostropheSprite);
				} else {
					apostropheSprite.gameObject.SetActive(false);
				}

				colonLowerSprite.gameObject.SetActive(false);
				colonUpperSprite.gameObject.SetActive(false);

			} else {

				segments[0]=new SingleSegmentSprite(parentSpriteDisplay, colonLowerSprite);
				segments[1]=new SingleSegmentSprite(parentSpriteDisplay, colonUpperSprite);

				for (int n = 0; n<BaseSegmentCount; n++) {
					segmentSprites[n].gameObject.SetActive(false);
				}

				decimalPointSprite.gameObject.SetActive(false);
				apostropheSprite.gameObject.SetActive(false);

			}

		}

		internal void spread(float m) {
			foreach (SingleSegment segment in segments) {
				((SingleSegmentSprite)(segment)).multiplyPosition(m);
			}
		}

        internal void doSpriteOrderRefresh() {
            int sortingOrder = ((SegmentDisplaySprite)(base.parentDisplay)).SpriteSortingOrder;
            foreach (SingleSegment segment in segments) {
                ((SingleSegmentSprite)(segment)).setSortingOrder(sortingOrder);
            }
        }
    
    }

}
