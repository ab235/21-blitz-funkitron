//    SegmentDisplay - Controller - Command - TextScroll


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	/// <summary>
	/// Command to scroll text to/on display.
    /// 
    /// Note that scrolling may act weirdly or scrolling text may jump multiple digits if display have colons enabled, since text can't be positioned freely
    /// to any position in display like that.
	/// </summary>
	public class ScrollTextCommand : AbsSDCommand {

		/// <summary>
		/// List of methods how text is scrolling.
		/// </summary>
		public enum Methods {
			/// <summary>
			/// Default passing scroll. Text will not initially be on display but appear from right side, moving left and continue until it have completely left the display.
			/// </summary>
			StartOutsideMoveLeftUntilOut,
			/// <summary>
			/// Text will not initially be on display but appear from right side, moving left until first character of text have reached left side of the display.
			/// </summary>
			StartOutsideMoveLeftUntilLeftAligned,
			/// <summary>
			/// Text will start from display and move left until it have completely left the display.
			/// </summary>
			StartLeftAlignedMoveLeftUntilOut,
			/// <summary>
			/// Text will start from display and move left until last character of text is on right side of display.
			/// For this to make sense, text have to be longer than what display can hold.
			/// </summary>
			StartLeftAlignedMoveLeftUntilRightAligned,
			/// <summary>
			/// Text will not initially be on display but appear from left side, moving right and continue until it have completely left the display.
			/// </summary>
			StartOutsideMoveRightUntilOut,
			/// <summary>
			/// Text will not initially be on display but appear from left side, moving right until last character of text have reached right side of the display.
			/// </summary>
			StartOutsideMoveRightUntilRightAligned,
			/// <summary>
			/// Text will start from display and move right until it have completely left the display.
			/// </summary>
			StartRightAlignedMoveRightUntilOut,
			/// <summary>
			/// Text will start from display and move right until first character of text is on left side of display.
			/// For this to make sense, text have to be longer than what display can hold.
			/// </summary>
			StartRightAlignedMoveRightUntilLeftAligned
		}

		private string text;
		private Methods method;
		private float charactersPerSecond;

		/// <summary>
		/// Create new text command that will scroll text on to display.
		/// </summary>
		/// <param name="text">
		/// Text to scroll on display.
		/// </param>
		/// <param name="method">
		/// Method how to scroll text, one of the enum Methods.
		/// </param>
		/// <param name="charactersPerSecond">
		/// Speed of scrolling text.
		/// </param>
		public ScrollTextCommand(string text, Methods method, float charactersPerSecond) {
			this.text=text;
			this.method=method;
			this.charactersPerSecond=charactersPerSecond;
		}

		internal override RawCmd getRawCommand() {
			return (new RawCmdTextScroll(text,method,charactersPerSecond));
		}

	}

}
