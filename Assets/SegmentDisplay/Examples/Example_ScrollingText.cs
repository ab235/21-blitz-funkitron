//    Example: Scrolling Text

//    Several scrolling text types


using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_ScrollingText : MonoBehaviour {

		public SegmentDisplay segmentDisplayRef;
		private SDController sdController;
		private int counter;

		void Start() {

			sdController = segmentDisplayRef.GetSDController();
			counter = 0;
			setScrollingText();

		}

		void setScrollingText() {

			counter++;

			if (counter == 1) {

				// Simplest scroll ever
				sdController.AddCommand(new ScrollTextCommand("Scrolling pass", ScrollTextCommand.Methods.StartOutsideMoveLeftUntilOut, 2.5f));
				// Slight delay
				sdController.AddCommand(new PauseCommand(3f));
				// Call this method again after previous commands are finished
				sdController.AddCommand(new CallbackCommand(setScrollingText));

			} else if (counter == 2) {

				string textToScroll = "Scrolling stop";
				// Scroll to display and wait
				sdController.AddCommand(new ScrollTextCommand(textToScroll, ScrollTextCommand.Methods.StartOutsideMoveLeftUntilLeftAligned, 5f));
				sdController.AddCommand(new PauseCommand(2.5f));
				// Scroll away from display
				sdController.AddCommand(new ScrollTextCommand(textToScroll, ScrollTextCommand.Methods.StartLeftAlignedMoveLeftUntilOut, 5f));
				// Slight delay before starting next
				sdController.AddCommand(new PauseCommand(3f));
				sdController.AddCommand(new CallbackCommand(setScrollingText));

			} else if (counter == 3) {

				string textToPingPong = "Ping-Ponging too long text".ToUpper();
				// Keep text on display for few seconds
				sdController.AddCommand(new SetTextCommand(textToPingPong));
				sdController.AddCommand(new PauseCommand(2.5f));
				// Scroll left until right side of text is fully visible and stay there for 2 seconds
				sdController.AddRepeatingCommand(new ScrollTextCommand(textToPingPong, ScrollTextCommand.Methods.StartLeftAlignedMoveLeftUntilRightAligned, 4f));
				sdController.AddRepeatingCommand(new PauseCommand(2f));
				// Scroll back to starting position and stay there for 2 seconds again
				sdController.AddRepeatingCommand(new ScrollTextCommand(textToPingPong, ScrollTextCommand.Methods.StartRightAlignedMoveRightUntilLeftAligned, 4f));
				sdController.AddRepeatingCommand(new PauseCommand(2f));
				// Since last 4 commands are added as repeating, this ping-ponging loop will continue forever

			}

		}

	}

}
