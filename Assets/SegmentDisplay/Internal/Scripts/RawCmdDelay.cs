//    SegmentDisplay - RawCommand - Delay


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	internal class RawCmdDelay : RawCmd {
		
		private float delay;
		private bool finished;

		internal RawCmdDelay(float delay) {
			this.delay=delay;
			finished=false;
		}
		
		internal override float runStep(SegmentDisplay segmentDisplay, float deltaTime) {
			if (deltaTime>=delay) {
				finished=true;
				return (deltaTime-delay);
			}
			return deltaTime;
		}
		
		internal override bool isFinished(SegmentDisplay segmentDisplay) {
			return finished;
		}
		
	}

}
