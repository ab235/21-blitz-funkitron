//    SegmentDisplay - SDController - Abstract Command


namespace Leguar.SegmentDisplay {
	
	public abstract class AbsSDCommand {

		private bool repeat;

		internal AbsSDCommand() {
			repeat=false;
		}

		internal void setRepeat() {
			repeat=true;
		}

		internal bool isRepeat() {
			return repeat;
		}
		
		internal abstract RawCmd getRawCommand();

	}

}
