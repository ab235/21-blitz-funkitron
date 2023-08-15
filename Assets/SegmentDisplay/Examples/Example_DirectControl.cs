//    Example: Direct Control

//    Controlling single digits and single segments of display directly


using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_DirectControl : MonoBehaviour {

		public SegmentDisplay segmentDisplay;

		private float updateTimer;
		private float typeTimer;

		void Start() {

			updateTimer = 0f;
			typeTimer = 0f;

		}

		void Update() {

			updateTimer += Time.deltaTime;
			typeTimer = (typeTimer+Time.deltaTime)%15f;

			if ((typeTimer<10f && updateTimer>0.1f) || (typeTimer>=10f && updateTimer>0.025f)) {

				if (typeTimer < 5f) {
					setRandomCharacterToRandomPosition();
				} else if (typeTimer < 10f) {
					changeRandomSegmentAtRandomPosition();
				} else {
					showRaysAnimation(typeTimer%1);
				}

				updateTimer = 0f;

			}

		}

		private void setRandomCharacterToRandomPosition() {
			string characters = "0123456789_ABCDEFGHIJKLMNOPQRSTUVWXYZ []*+-/\\";
			char randomChar = characters[Random.Range(0, characters.Length)];
			SingleDigit randomDigit = segmentDisplay[Random.Range(0, segmentDisplay.DigitCount)];
			randomDigit.SetChar(randomChar);
		}

		private void changeRandomSegmentAtRandomPosition() {
			SingleDigit randomDigit = segmentDisplay[Random.Range(0, segmentDisplay.DigitCount)];
			SingleSegment randomSegment = randomDigit[Random.Range(0, randomDigit.TotalSegmentCount)];
			randomSegment.SetState(!randomSegment.GetState());
		}

		private void showRaysAnimation(float percent) {
			int[] segmentOrder = { 9, 10, 7, 11, 12, 13, 6, 8 }; // Assuming using 14 segment digits
			int segmentToTurnOn = segmentOrder[(int)(segmentOrder.Length*percent)];
			segmentDisplay[0].Clear();
			segmentDisplay[0][segmentToTurnOn].SetState(true);
		}

	}

}
