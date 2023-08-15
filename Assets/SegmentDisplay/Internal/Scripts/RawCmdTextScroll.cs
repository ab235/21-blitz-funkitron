//    SegmentDisplay - RawCommand - Scroll Text


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	internal class RawCmdTextScroll : RawCmd {

		private string text;
		private ScrollTextCommand.Methods method;
		private float secondsPerCharacter;
		private int counter;
		
		internal RawCmdTextScroll(string text, ScrollTextCommand.Methods method, float charactersPerSecond) {
			this.text=text;
			this.method = method;
			secondsPerCharacter=1f/charactersPerSecond;
			counter=0;
		}

		internal override float runStep(SegmentDisplay segmentDisplay, float deltaTime) {

			while (counter==0 || deltaTime>=secondsPerCharacter) {
				if (method==ScrollTextCommand.Methods.StartOutsideMoveLeftUntilOut) {
					segmentDisplay.SetText(text,SegmentDisplay.Alignments.Left,segmentDisplay.DigitCount-counter);
				} else if (method==ScrollTextCommand.Methods.StartOutsideMoveLeftUntilLeftAligned) {
					segmentDisplay.SetText(text,SegmentDisplay.Alignments.Left,segmentDisplay.DigitCount-counter);
				} else if (method==ScrollTextCommand.Methods.StartLeftAlignedMoveLeftUntilOut) {
					segmentDisplay.SetText(text,SegmentDisplay.Alignments.Left,-counter);
				} else if (method==ScrollTextCommand.Methods.StartLeftAlignedMoveLeftUntilRightAligned) {
					segmentDisplay.SetText(text,SegmentDisplay.Alignments.Left,-counter);
				} else if (method==ScrollTextCommand.Methods.StartOutsideMoveRightUntilOut) {
					segmentDisplay.SetText(text,SegmentDisplay.Alignments.Right,-segmentDisplay.DigitCount+counter);
				} else if (method==ScrollTextCommand.Methods.StartOutsideMoveRightUntilRightAligned) {
					segmentDisplay.SetText(text,SegmentDisplay.Alignments.Right,-segmentDisplay.DigitCount+counter);
				} else if (method==ScrollTextCommand.Methods.StartRightAlignedMoveRightUntilOut) {
					segmentDisplay.SetText(text,SegmentDisplay.Alignments.Right,counter);
				} else if (method==ScrollTextCommand.Methods.StartRightAlignedMoveRightUntilLeftAligned) {
					segmentDisplay.SetText(text,SegmentDisplay.Alignments.Right,counter);
				}
				if (counter>0) {
					deltaTime-=secondsPerCharacter;
				}
				counter++;
				if (isFinished(segmentDisplay)) {
					break;
				}
			}

			return deltaTime;

		}
		
		internal override bool isFinished(SegmentDisplay segmentDisplay) {
			if (method == ScrollTextCommand.Methods.StartOutsideMoveLeftUntilOut) {
				return (counter>segmentDisplay.DigitCount+text.Length);
			} else if (method == ScrollTextCommand.Methods.StartOutsideMoveLeftUntilLeftAligned) {
				return (counter>segmentDisplay.DigitCount);
			} else if (method == ScrollTextCommand.Methods.StartLeftAlignedMoveLeftUntilOut) {
				return (counter>text.Length);
			} else if (method == ScrollTextCommand.Methods.StartLeftAlignedMoveLeftUntilRightAligned) {
				return (counter>text.Length-segmentDisplay.DigitCount);
			} else if (method==ScrollTextCommand.Methods.StartOutsideMoveRightUntilOut) {
				return (counter>segmentDisplay.DigitCount+text.Length);
			} else if (method==ScrollTextCommand.Methods.StartOutsideMoveRightUntilRightAligned) {
				return (counter>segmentDisplay.DigitCount);
			} else if (method==ScrollTextCommand.Methods.StartRightAlignedMoveRightUntilOut) {
				return (counter>text.Length);
			} else if (method==ScrollTextCommand.Methods.StartRightAlignedMoveRightUntilLeftAligned) {
				return (counter>text.Length-segmentDisplay.DigitCount);
			} else {
				Debug.LogError("Leguar.SegmentDisplay: RawCmdTextScroll.isFinished(...): Internal error, unknown scroll method");
				return true;
			}
		}
		
	}

}
