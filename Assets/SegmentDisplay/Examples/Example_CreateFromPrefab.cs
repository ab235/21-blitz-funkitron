//    Example: CreateFromPrefab

//    Creating new SegmentDisplay from prefab during runtime


using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_CreateFromPrefab : MonoBehaviour {

		public GameObject segmentDisplayPrefab;

		void Start() {

			// Create new gameobject from prefab
			GameObject segmentDisplayObject = (GameObject)(Instantiate(segmentDisplayPrefab));

			// Set parent and position on screen
			segmentDisplayObject.name = "Prefab Test Display";
			segmentDisplayObject.transform.parent = this.transform;
			segmentDisplayObject.transform.localPosition = Vector3.zero;

			// Get SegmentDisplay script attached to object
			SegmentDisplay segmentDisplay = segmentDisplayObject.GetComponent<SegmentDisplay>();

			// Change display settings, these have to be done right after creating object since new display
			// will get initialized on next update loop or when adding text to it
			segmentDisplay.DigitCount = 11;
			segmentDisplay.DisplayType = SegmentDisplay.DisplayTypes.FourteenSegment;

			// Add text
			segmentDisplay.SetText("PREFAB TEST");

			// Colors can be freely changed also after display is initialized
			segmentDisplay.OnColor = new Color(0.9f, 0.9f, 0f);
			segmentDisplay.OffColor = new Color(0.1f, 0.1f, 0f);

			// Wait few seconds
			segmentDisplay.GetSDController().AddCommand(new PauseCommand(3f));

			// Slowly clear the display (after the pause)
			segmentDisplay.GetSDController().AddCommand(new ClearCommand(ClearCommand.Methods.DigitByDigitFromLeft, 1f));

		}

	}

}
