//    Example: CurrentTimeClock

//    Simply read current real time and set it to display


using System;
using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_CurrentTimeClock : MonoBehaviour {

		public SegmentDisplay display; // Reference is set in Unity inspector

		private float timer; // Used to update display only twice a second

		void Start() {
			setTimeToDisplay();
			timer = 0f;
		}

		void Update() {
			timer += Time.deltaTime;
			if (timer > 0.5f) {
				setTimeToDisplay();
				timer -= 0.5f;
			}
		}

		private void setTimeToDisplay() {

			// Current time
			DateTime dt = DateTime.Now;
			int h = dt.Hour;
			int m = dt.Minute;
			int s = dt.Second;

			// Create string in form of HH:MM:SS and set it to display
			string str = (h < 10 ? "0" : "") + h + ":" + (m < 10 ? "0" : "") + m + ":" + (s < 10 ? "0" : "") + s;
			display.SetText(str);

			// Make first colon (digit index 5) to blink
			if (dt.Millisecond < 500) {
				display[5].SetColonPointStates(false, true);
			} else {
				display[5].SetColonPointStates(true, false);
			}

		}

	}

}
