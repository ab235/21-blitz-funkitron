//    SegmentDisplay - RawCommand - Callback


using System;

namespace Leguar.SegmentDisplay {
	
	internal class RawCmdCallback : RawCmd {
		
		private Action callback;
		
		internal RawCmdCallback(Action callback) {
			this.callback=callback;
		}
		
		internal override float runStep(SegmentDisplay segmentDisplay, float deltaTime) {
			callback();
			return deltaTime;
		}
		
		internal override bool isFinished(SegmentDisplay segmentDisplay) {
			return true;
		}
		
	}

}
