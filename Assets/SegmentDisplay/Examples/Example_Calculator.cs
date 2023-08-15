//    Example: Calculator

//    Somewhat dummy example code. Main thing is actual display, showing how it is simple to give LCD look for display by adding gray background
//    and setting segment display "On Color" to black or dark gray.


using UnityEngine;
using Leguar.SegmentDisplay;

namespace Leguar.SegmentDisplay.Example {

	public class Example_Calculator : MonoBehaviour {

		private SegmentDisplay display;

		private string currentInput;
		private float timer;

		void Start() {

			// Just another way to get reference to display
			display = GameObject.Find("SegmentDisplay_Calc").GetComponent<SegmentDisplay>();

			currentInput = "";
			timer = 0f;

		}

		void Update() {

			if (display.GetSDController().IsIdle()) { // Check that display isn't showing "-E-" message at the moment

				timer+=Time.deltaTime;

				if (timer>1f) {

					if (currentInput.Length==0) {
						if (Random.value<0.8f) {
							currentInput=""+Random.Range(1, 10);
						} else {
							currentInput="0.";
						}
					} else {
						if (currentInput.IndexOf('.')>=0 || Random.value<0.85f) {
							currentInput += ""+Random.Range(0, 10);
						} else {
							currentInput += ".";
						}
					}

					int inputDigitLength = currentInput.Replace(".", "").Length;

					if (inputDigitLength<=display.DigitCount) {
						display.SetText(addThousandSeparators(currentInput));
					} else {
						// Well, calculators do not usually go to error state when typing too many digits, but this one is special
						display.SetText("-E-", SegmentDisplay.Alignments.Left); // Set "-E-" to display
						display.GetSDController().AddCommand(new PauseCommand(3f)); // Wait for 3 seconds
						display.GetSDController().AddCommand(new SetTextCommand("0")); // Reset showing "0"
						currentInput="";
					}

					timer=0f;

				}

			}

		}

		private string addThousandSeparators(string str) {
			int dp = str.IndexOf('.');
			int ind = (dp>=0 ? dp : str.Length);
			while (ind>3) {
				ind-=3;
				str=str.Substring(0, ind)+'\''+str.Substring(ind);
			}
			return str;
		}

	}

}
