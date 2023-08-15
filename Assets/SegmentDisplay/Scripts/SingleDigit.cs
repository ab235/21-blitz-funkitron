//    Base class for all segment digits


using UnityEngine;

namespace Leguar.SegmentDisplay {

	/// <summary>
	/// Class representing single digit in display. Digit is made from 7, 14 or 16 segments with optional decimal point segment. Or digit may be colon,
	/// in which case it have only two segments.
	/// </summary>
	public abstract class SingleDigit : MonoBehaviour {

		protected SegmentDisplay parentDisplay;
		protected SingleSegment[] segments;

		/// <summary>
		/// Gets reference to certain segment of this digit. Typically there is no need to control segments separately, but instead you can use methods
		/// in SegmentDisplay to control whole display, or methods in this class to control content of this single digit.
		/// </summary>
		/// <param name="segmentIndex">
		/// Segment index. If this digit is colon, there's just two segments, segment at index 0 is lower dot and 1 is upper dot.
		/// </param>
		public SingleSegment this[int segmentIndex] {
			get {
				return segments[segmentIndex];
			}
		}

		internal enum Mode {
			Digit,
			Colon
		}

		internal Mode mode;

		internal bool decimalPointEnabled;
		internal bool apostropheEnabled;

		/// <summary>
		/// Gets the base segment count. If this digit is part of seven segment display, this returns 7, etc.
		/// </summary>
		/// <value>
		/// The base segment count.
		/// </value>
		public abstract int BaseSegmentCount {
			get;
		}

		/// <summary>
		/// Gets the total number of segments in this digit. Segment count depends on display type (7/14/16 segments). If decimal point or apostrophe
		/// are enabled, they will add one more segment each. If this digit is colon, there's just two segments.
		/// </summary>
		/// <value>
		/// The total segment count.
		/// </value>
		public int TotalSegmentCount {
			get {
				return segments.Length;
			}
		}

		internal void init(SegmentDisplay parentDisplay, Mode mode, bool decimalPointEnabled, bool apostropheEnabled) {

			this.parentDisplay=parentDisplay;
			this.mode=mode;
			this.decimalPointEnabled=decimalPointEnabled;
			this.apostropheEnabled=apostropheEnabled;

			int totalSegmentCount;
			if (mode==Mode.Digit) {
				totalSegmentCount=BaseSegmentCount;
				if (decimalPointEnabled) {
					totalSegmentCount++;
				}
				if (apostropheEnabled) {
					totalSegmentCount++;
				}
			} else {
				totalSegmentCount=2;
			}
			segments=new SingleSegment[totalSegmentCount];

			createSegments();
			Clear();

		}

		internal abstract void createSegments();

		/// <summary>
		/// Clear this digit by setting all segments to off state.
		/// </summary>
		public void Clear() {
			foreach (SingleSegment segment in segments) {
				segment.SetState(false);
			}
		}

		/// <summary>
		/// Fill this digit by setting all segments to on state.
		/// </summary>
		public void Fill() {
			foreach (SingleSegment segment in segments) {
				segment.SetState(true);
			}
		}

		/// <summary>
		/// Check whatever all segments in this digit are in off state.
		/// </summary>
		/// <returns>
		/// True if all segments are off, false otherwise.
		/// </returns>
		public bool IsEmpty() {
			foreach (SingleSegment segment in segments) {
				if (segment.GetState()) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Check whatever all segments in this digit are in on state.
		/// </summary>
		/// <returns>
		/// True if all segments are on, false otherwise.
		/// </returns>
		public bool IsFull() {
			foreach (SingleSegment segment in segments) {
				if (!segment.GetState()) {
					return false;
				}
			}
			return true;
		}

        internal void doColorRefresh() {
			foreach (SingleSegment segment in segments) {
				segment.doColorRefresh();
			}
		}

		internal void copyState(SingleDigit anotherSegmentDigit) {
			if (anotherSegmentDigit.TotalSegmentCount==this.TotalSegmentCount) {
				// Normal case
				for (int n=0; n<segments.Length; n++) {
					segments[n].SetState(anotherSegmentDigit[n].GetState());
				}
			} else {
				// May happen if display have colons
				Clear();
			}
		}

		/// <summary>
		/// Set character to this digit. Supported characters depends on display style (number of segments).
		/// 
		/// If this digit is colon: Space will set both segment dots off. Colon and semicolon will set both segment dots on.
		/// Period or comma will set only lower segment dot on. Apostrophe will set only upper segment dot on. With any other
		/// characters both segment dots will be off and this method returns false.
		/// </summary>
		/// <param name="chr">
		/// Character to set.
		/// </param>
		/// <returns>
		/// True if character was supported and was set visible. False if character was unsupported and this digit was set empty.
		/// </returns>

		public bool SetChar(char chr) {

			if (mode==Mode.Colon) {

				if (chr==' ') {
					SetColonPointStates(false,false);
					return true;
				}
				if (chr==':' || chr==';') {
					SetColonPointStates(true,true);
					return true;
				}
				if (SegmentDisplay.isDecimalPointCharacter(chr)) {
					SetColonPointStates(true,false);
					return true;
				}
				if (SegmentDisplay.isApostropheCharacter(chr)) {
					SetColonPointStates(false,true);
					return true;
				}

			} else {

				string charMask=getCharMask(chr);

				if (charMask!=null) {
					if (decimalPointEnabled || apostropheEnabled) {
						Clear();
					}
					for (int n=0; n<BaseSegmentCount; n++) {
						segments[n].SetState(charMask[n]=='1');
					}
					return true;
				}

			}

			Clear();
			return false;

		}

		private string getCharMask(char chr) {
			if (BaseSegmentCount == 7) {
				return CharMaskSeven.getCharMask(parentDisplay,chr);
			} else if (BaseSegmentCount == 14) {
				return CharMaskFourteen.getCharMask(parentDisplay,chr);
			} else if (BaseSegmentCount == 16) {
				return CharMaskSixteen.getCharMask(parentDisplay,chr);
			} else {
				Debug.LogError("Leguar.SegmentDisplay: SegmentDigit.getCharMask(): Internal error, unknown BaseSegmentCount ("+BaseSegmentCount+")");
				return null;
			}
		}

		/// <summary>
		/// Check whatever this digit have segment for decimal point.
		/// </summary>
		/// <returns>
		/// True if this digit has decimal point, false otherwise.
		/// </returns>
		public bool IsDecimalPointEnabled() {
			return decimalPointEnabled;
		}

		/// <summary>
		/// Set decimal point on or off without changing any other segments.
		/// This method doesn't do anything if this digit doesn't have decimal point enabled.
		/// </summary>
		/// <param name="state">
		/// Set on if true, set off if false
		/// </param>
		public void SetDecimalPointState(bool on) {
			if (mode==Mode.Digit && decimalPointEnabled) {
				segments[segments.Length-(apostropheEnabled?2:1)].SetState(on);
			}
		}

		/// <summary>
		/// Check whatever this digit have segment for apostrophe.
		/// </summary>
		/// <returns>
		/// True if this digit has apostrophe, false otherwise.
		/// </returns>
		public bool IsApostropheEnabled() {
			return apostropheEnabled;
		}

		/// <summary>
		/// Set apostrophe on or off without changing any other segments.
		/// This method doesn't do anything if this digit doesn't have apostrophe enabled.
		/// </summary>
		/// <param name="state">
		/// Set on if true, set off if false
		/// </param>
		public void SetApostropheState(bool on) {
			if (mode==Mode.Digit && apostropheEnabled) {
				segments[segments.Length-1].SetState(on);
			}
		}

		/// <summary>
		/// Check whatever this digit is colon instead of normal digit.
		/// </summary>
		/// <returns>
		/// True if this digit is colon, false otherwise.
		/// </returns>
		public bool IsColon() {
			return (mode==Mode.Colon);
		}

		/// <summary>
		/// Set colon points on or off. This method doesn't do anything if this digit isn't colon.
		/// </summary>
		/// <param name="lowerOn">
		/// Set lower dot on if true, set off if false.
		/// </param>
		/// <param name="upperOn">
		/// Set upper dot on if true, set off if false.
		/// </param>
		public void SetColonPointStates(bool lowerOn, bool upperOn) {
			if (mode==Mode.Colon) {
				segments[0].SetState(lowerOn);
				segments[1].SetState(upperOn);
			}
		}

	}

}
