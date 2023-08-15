//    Class for image based segment digit


using UnityEngine.UI;

namespace Leguar.SegmentDisplay {

	public class SingleDigitImage : SingleDigit {

		public Image[] segmentImages;
		public Image decimalPointImage;
		public Image apostropheImage;
		public Image colonLowerImage;
		public Image colonUpperImage;

		public override int BaseSegmentCount {
			get {
				return segmentImages.Length;
			}
		}

		internal override void createSegments() {

			if (base.mode==Mode.Digit) {

				for (int n = 0; n<BaseSegmentCount; n++) {
					segments[n]=new SingleSegmentImage(parentDisplay, segmentImages[n]);
				}

				if (base.decimalPointEnabled) {
					segments[BaseSegmentCount]=new SingleSegmentImage(parentDisplay, decimalPointImage);
				} else {
					decimalPointImage.gameObject.SetActive(false);
				}

				if (base.apostropheEnabled) {
					segments[BaseSegmentCount+(base.decimalPointEnabled ? 1 : 0)]=new SingleSegmentImage(parentDisplay, apostropheImage);
				} else {
					apostropheImage.gameObject.SetActive(false);
				}

				colonLowerImage.gameObject.SetActive(false);
				colonUpperImage.gameObject.SetActive(false);

			} else {

				segments[0]=new SingleSegmentImage(parentDisplay, colonLowerImage);
				segments[1]=new SingleSegmentImage(parentDisplay, colonUpperImage);

				for (int n = 0; n<BaseSegmentCount; n++) {
					segmentImages[n].gameObject.SetActive(false);
				}

				decimalPointImage.gameObject.SetActive(false);
				apostropheImage.gameObject.SetActive(false);

			}

		}

		internal void spreadAndMultiplySize(float spread, float multipX, float multipY) {
			foreach (SingleSegment segment in segments) {
				((SingleSegmentImage)(segment)).spreadAndMultiplySize(spread,multipX,multipY);
			}
		}

	}

}
