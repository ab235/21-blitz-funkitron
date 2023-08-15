//    SegmentDisplay - RawCommand - Fill


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	internal class RawCmdFill : RawCmd {

		private int mode;
		private int startIndex;
		private int count;
		private FillCommand.Methods method;
		private float secondsPerCharacter;
		
		private int counter;

		internal RawCmdFill(int mode, int startIndex, int count, FillCommand.Methods method, float charactersPerSecond) {
			this.mode=mode;
			this.startIndex=startIndex;
			this.count=count;
			this.method=method;
			secondsPerCharacter=1f/charactersPerSecond;
			counter=0;
		}

		internal override float runStep(SegmentDisplay segmentDisplay, float deltaTime) {
			
			if (mode==0) {
				segmentDisplay.Fill();
				return deltaTime;
			}

			if (mode==1) {
				segmentDisplay.Fill(startIndex,count);
				return deltaTime;
			}

			while (counter==0 || deltaTime>=secondsPerCharacter) {
				if (method==FillCommand.Methods.DigitByDigitFromLeft) {
					segmentDisplay[counter].Fill();
				} else {
					segmentDisplay[segmentDisplay.DigitCount-1-counter].Fill();
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
			return (counter>=segmentDisplay.DigitCount);
		}
		
	}
	
}
