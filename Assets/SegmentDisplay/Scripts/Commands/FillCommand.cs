//    SegmentDisplay - Controller - Command - Fill


namespace Leguar.SegmentDisplay {
	
	/// <summary>
	/// Command to clear the display. Clearing may be instant or happen with some effect like scrolling all the content away from display,
	/// or clearing display character by character.
	/// </summary>
	public class FillCommand : AbsSDCommand {
		
		/// <summary>
		/// List of methods how display can be cleared.
		/// </summary>
		public enum Methods {
			/// <summary>Fill display digit by digit starting from left.</summary>
			DigitByDigitFromLeft,
			/// <summary>Fill display digit by digit starting from right.</summary>
			DigitByDigitFromRight
		}

		private int mode;
		private int startIndex;
		private int count;
		private Methods method;
		private float charactersPerSecond;

		/// <summary>
		/// Creates new fill command that will instantly fill the display.
		/// </summary>
		public FillCommand() {
			mode = 0;
		}

		/// <summary>
		/// Creates new fill command that will instantly fill part of display.
		/// </summary>
		/// <param name="startIndex">
		/// Index of first digit to fill.
		/// </param>
		/// <param name="count">
		/// How many digits to fill.
		/// </param>
		public FillCommand(int startIndex, int count) {
			mode = 1;
			this.startIndex=startIndex;
			this.count=count;
		}

		/// <summary>
		/// Creates new fill with specified effect.
		/// </summary>
		/// <param name="method">
		/// Method how display is filled, one of the values from enum Methods
		/// </param>
		/// <param name="charactersPerSecond">
		/// Speed how fast display is cleared, in characters per second
		/// </param>
		public FillCommand(Methods method, float charactersPerSecond) {
			mode = 2;
			this.method=method;
			this.charactersPerSecond=charactersPerSecond;
		}

		internal override RawCmd getRawCommand() {
			return (new RawCmdFill(mode,startIndex,count,method,charactersPerSecond));
		}
		
	}
	
}
