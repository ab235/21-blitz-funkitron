//    Example: BootSequence

//    Example how SDController class can run things in sequence and let us know when everything is done


using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_BootSequence : MonoBehaviour {

		public SegmentDisplay display; // Reference is set in Unity inspector

		void Start() {

			// Get controller
			SDController sdController = display.GetSDController();

			// Show texts with pauses
			sdController.AddCommand(new ClearCommand());
			sdController.AddCommand(new PauseCommand(2f));
			sdController.AddCommand(new SetTextCommand("BOOT v1.24"));
			sdController.AddCommand(new PauseCommand(4f));
			sdController.AddCommand(new CallbackCommand(brightenUp));
			sdController.AddCommand(new PauseCommand(3f));
			sdController.AddCommand(new SetTextCommand("THINGIE LTD"));
			sdController.AddCommand(new PauseCommand(2.5f));
			sdController.AddCommand(new SetTextCommand("Starting up"));
			sdController.AddCommand(new PauseCommand(4.5f));
			sdController.AddCommand(new ClearCommand());

			// Show progressbar by filling parts of the display
			for (int n = 0; n<=display.DigitCount; n++) {
				// Clear
				sdController.AddCommand(new ClearCommand());
				// Set "xxx%" text to right side of display
				if (n<display.DigitCount-3) {
					sdController.AddCommand(new SetTextCommand(Mathf.RoundToInt(100f*n/display.DigitCount)+"%", SegmentDisplay.Alignments.Right));
				}
				// Fill part of display ("progress bar")
				sdController.AddCommand(new FillCommand(0, n));
				// Wait random time
				sdController.AddCommand(new PauseCommand(Random.Range(0.4f, 2.2f)));
			}

			// Blink "cmd>" and "cmd>_" forever
			sdController.AddRepeatingCommand(new PauseCommand(0.4f));
			sdController.AddRepeatingCommand(new SetTextCommand("cmd>"));
			sdController.AddRepeatingCommand(new PauseCommand(0.4f));
			sdController.AddRepeatingCommand(new SetTextCommand("cmd>_"));

			// Let this class know when all is done
			sdController.AddCommand(new CallbackCommand(systemReady));

		}

		void brightenUp() {
			// Set display segments "on state" color to maximum red after "BOOT v1.24" have been on screen for a while
			display.OnColor = Color.red;
		}

		void systemReady() {
			// Booted up, "Thingie Ltd" system ready for user input :)
		}

	}

}
