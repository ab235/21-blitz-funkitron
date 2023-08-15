//    Example: Players' Score


using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_PlayersScore : MonoBehaviour {

		public SegmentDisplay displayPlayer1;
		public SegmentDisplay displayPlayer2;

		private int scorePlayer1;
		private int scorePlayer2;

		private float timeCounter;

		void Start() {
			scorePlayer1 = 0;
			scorePlayer2 = 0;
			timeCounter = 0;
		}

		void Update() {
			timeCounter += Time.deltaTime;
			if (timeCounter>5f) {
				if (Random.value < 0.5f) {
					scorePlayer1++;
					displayPlayer1.SetText(""+scorePlayer1);
				} else {
					scorePlayer2++;
					displayPlayer2.SetText(""+scorePlayer2);
				}
				timeCounter = 0f;
			}
		}

	}

}
