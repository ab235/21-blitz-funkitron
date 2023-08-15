//    Example: Timer

//    Using controller class to run timer and let code know when it is done


using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_Timer : MonoBehaviour {

		public SegmentDisplay display; // Reference is set in Unity inspector
		public int secondsToRun;

		void Start() {

			// Get controller
			SDController sdController = display.GetSDController();

			// Run timer
			for (int s = secondsToRun; s>=1; s--) {
				sdController.AddCommand(new SetTextCommand(getFormattedTimerText(s)));
				sdController.AddCommand(new PauseCommand(1f));
			}

			// Call method 'timerFinished()' in this class after timer is done
			sdController.AddCommand(new CallbackCommand(timerFinished));

			// Blink "End" for a while
			for (int n = 0; n<5; n++) {
				sdController.AddCommand(new SetTextCommand("End", SegmentDisplay.Alignments.Left));
				sdController.AddCommand(new PauseCommand(0.5f));
				sdController.AddCommand(new ClearCommand());
				sdController.AddCommand(new PauseCommand(0.5f));
			}

		}

		void timerFinished() {
			// Ding! (At this point "End" is blinking on display)
		}

		private string getFormattedTimerText(int totalSeconds) {
			if (totalSeconds<10) {
				return (":0"+totalSeconds);
			}
			if (totalSeconds<60) {
				return (":"+totalSeconds);
			}
			int minutes = totalSeconds/60;
			int seconds = totalSeconds%60;
			return (minutes+":"+(seconds<10 ? "0" : "")+seconds);
		}

	}

}
