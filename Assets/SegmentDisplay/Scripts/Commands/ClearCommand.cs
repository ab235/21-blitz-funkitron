//    SegmentDisplay - Controller - Command - Clear


namespace Leguar.SegmentDisplay {
	
	/// <summary>
	/// Command to clear the display. Clearing may be instant or happen with some effect like scrolling all the content away from display,
	/// or clearing display character by character.
	/// </summary>
	public class ClearCommand : AbsSDCommand {
		
		/// <summary>
		/// List of methods how display can be cleared.
		/// </summary>
		public enum Methods {
			/// <summary>Move everything left until display is clear. Note that this method doesn't work correctly in displays that have colons.</summary>
			MoveLeft,
			/// <summary>Move everything right until display is clear. Note that this method doesn't work correctly in displays that have colons.</summary>
			MoveRight,
			/// <summary>Clear display digit by digit starting from left.</summary>
			DigitByDigitFromLeft,
			/// <summary>Clear display digit by digit starting from right.</summary>
			DigitByDigitFromRight
		}

		private int mode;
		private int startIndex;
		private int count;
		private Methods method;
		private float charactersPerSecond;

		/// <summary>
		/// Creates new clear command that will instantly clear the display.
		/// </summary>
		public ClearCommand() {
			mode = 0;
		}

		/// <summary>
		/// Creates new clear command that will instantly clear part of display.
		/// </summary>
		/// <param name="startIndex">
		/// Index of first digit to clear.
		/// </param>
		/// <param name="count">
		/// How many digits to clear.
		/// </param>
		public ClearCommand(int startIndex, int count) {
			mode = 1;
			this.startIndex=startIndex;
			this.count=count;
		}

		/// <summary>
		/// Creates new clear with specified effect.
		/// </summary>
		/// <param name="method">
		/// Method how display is cleared, one of the values from enum Methods
		/// </param>
		/// <param name="charactersPerSecond">
		/// Speed how fast display is cleared, in characters per second
		/// </param>
		public ClearCommand(Methods method, float charactersPerSecond) {
			mode = 2;
			this.method=method;
			this.charactersPerSecond=charactersPerSecond;
		}

		internal override RawCmd getRawCommand() {
			return (new RawCmdClear(mode,startIndex,count,method,charactersPerSecond));
		}
		
	}
	
}
