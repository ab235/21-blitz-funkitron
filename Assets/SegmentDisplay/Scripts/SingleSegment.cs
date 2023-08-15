//    SingleSegment


using UnityEngine;

namespace Leguar.SegmentDisplay {

	/// <summary>
	/// Class representing single segment.
	/// </summary>
	public abstract class SingleSegment {

		protected SegmentDisplay parentDisplay;
		private bool state;

		internal SingleSegment(SegmentDisplay parentDisplay) {
			this.parentDisplay=parentDisplay;
		}

		/// <summary>
		/// Set this segment to on or off state.
		/// </summary>
		/// <param name="on">
		/// If true, segment is set on, if false, segment is set off.
		/// </param>
		public void SetState(bool on) {
			state=on;
			setColorByState();
		}

		/// <summary>
		/// Gets the state of this segment, on or off.
		/// </summary>
		/// <returns>
		/// True if segment is in on state, false if segment is in off state.
		/// </returns>
		public bool GetState() {
			return state;
		}

		internal void doColorRefresh() {
			setColorByState();
		}

		private void setColorByState() {
			setColor(state?parentDisplay.OnColor:parentDisplay.OffColor);
		}

		protected abstract void setColor(Color color);

	}

}
