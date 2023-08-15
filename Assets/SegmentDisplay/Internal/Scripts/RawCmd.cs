//    SegmentDisplay - Abstract RawCommand


namespace Leguar.SegmentDisplay {
	
	internal abstract class RawCmd {
		
		internal abstract float runStep(SegmentDisplay segmentDisplay, float deltaTime);
		
		internal abstract bool isFinished(SegmentDisplay segmentDisplay);
		
	}

}
