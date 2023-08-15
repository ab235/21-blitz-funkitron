//    SegmentDisplay - RawCommand - Clear


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	internal class RawCmdClear : RawCmd {

		private int mode;
		private int startIndex;
		private int count;
		private ClearCommand.Methods method;
		private float secondsPerCharacter;
		
		private int counter;

		internal RawCmdClear(int mode, int startIndex, int count, ClearCommand.Methods method, float charactersPerSecond) {
			this.mode=mode;
			this.startIndex=startIndex;
			this.count=count;
			this.method=method;
			secondsPerCharacter=1f/charactersPerSecond;
			counter=0;
		}

		internal override float runStep(SegmentDisplay segmentDisplay, float deltaTime) {
			
			if (mode==0) {
				segmentDisplay.Clear();
				return deltaTime;
			}

			if (mode==1) {
				segmentDisplay.Clear(startIndex,count);
				return deltaTime;
			}

			while (counter==0 || deltaTime>=secondsPerCharacter) {

				if (method==ClearCommand.Methods.MoveLeft) {
					segmentDisplay.MoveLeft();
				} else if (method==ClearCommand.Methods.MoveRight) {
					segmentDisplay.MoveRight();
				} else if (method==ClearCommand.Methods.DigitByDigitFromLeft) {
					segmentDisplay[counter].Clear();
				} else if (method==ClearCommand.Methods.DigitByDigitFromRight) {
					segmentDisplay[segmentDisplay.DigitCount-1-counter].Clear();
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
			if (mode==0 || mode==1) {
				return true;
			}
			if (method==ClearCommand.Methods.MoveLeft || method==ClearCommand.Methods.MoveRight) {
				return segmentDisplay.IsEmpty();
			}
			return (counter>=segmentDisplay.DigitCount);
		}
		
	}
	
}
