//    Example: Spinning Counter

//    This example code itself is dummy. Object itself in the scene is main thing, showing one simple way to add working display to 3D objects.


using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_SpinningCounter : MonoBehaviour {

		public SegmentDisplay counterDisplay;

		private float rotation;
		private int count;

		void Start() {
			rotation = 0f;
			count = 0;
		}

		void Update() {

			float prevRotation = rotation;
			rotation += Time.deltaTime * 150f * (0.5f+Mathf.Sin(rotation/360f*Mathf.PI)*1.5f);
			rotation = rotation % 360f;
			this.transform.localRotation = Quaternion.Euler(new Vector3(0f, rotation, 0f));

			if (prevRotation<180f && rotation>=180f) {
				count++;
				if (count < 1000) {
					counterDisplay.SetText("000");
					counterDisplay.AddText("" + count);
				} else {
					counterDisplay.SetText("Err");
				}
			}

		}

	}

}
