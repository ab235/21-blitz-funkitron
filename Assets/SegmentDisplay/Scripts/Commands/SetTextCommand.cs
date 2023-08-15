//    SegmentDisplay - Controller - Command - SetText


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	/// <summary>
	/// Command to set text on display.
	/// </summary>
	public class SetTextCommand : AbsSDCommand {

		private string text;
		private bool withAlignment;
		private SegmentDisplay.Alignments alignment;

		/// <summary>
		/// Create new text command that will set text instantly to display, using display default text alignment.
		/// 
		/// All the digits in display that this new text is not using will be cleared.
		/// </summary>
		/// <param name="text">
		/// Text to set on display.
		/// </param>
		public SetTextCommand(string text) {
			this.text=text;
			withAlignment=false;
		}

		/// <summary>
		/// Create new text command that will set text instantly to display, using specified text alignment.
		/// 
		/// All the digits in display that this new text is not using will be cleared.
		/// </summary>
		/// <param name="text">
		/// Text to set on display.
		/// </param>
		/// <param name="alignment">
		/// Wanted text alignment.
		/// </param>
		public SetTextCommand(string text, SegmentDisplay.Alignments alignment) {
			this.text=text;
			withAlignment=true;
			this.alignment=alignment;
		}

		internal override RawCmd getRawCommand() {
			if (!withAlignment) {
				return (new RawCmdText(text,true));
			} else {
				return (new RawCmdText(text,alignment,true));
			}
		}

	}

}
