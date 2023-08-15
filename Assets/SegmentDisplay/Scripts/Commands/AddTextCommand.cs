//    SegmentDisplay - Controller - Command - AddText


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	/// <summary>
	/// Command to add text on display without changing possible previous content.
	/// </summary>
	public class AddTextCommand : AbsSDCommand {

		private string text;
		private bool withAlignment;
		private SegmentDisplay.Alignments alignment;

		/// <summary>
		/// Create new text command that will add text instantly to display, using display default text alignment.
		/// 
		/// All the digits in display that this new text is not using will stay unchanged.
		/// </summary>
		/// <param name="text">
		/// Text to set on display.
		/// </param>
		public AddTextCommand(string text) {
			this.text=text;
			withAlignment=false;
		}

		/// <summary>
		/// Create new text command that will set text instantly to display, using specified text alignment.
		/// 
		/// All the digits in display that this new text is not using will stay unchanged.
		/// </summary>
		/// <param name="text">
		/// Text to set on display.
		/// </param>
		/// <param name="alignment">
		/// Wanted text alignment.
		/// </param>
		public AddTextCommand(string text, SegmentDisplay.Alignments alignment) {
			this.text=text;
			withAlignment=true;
			this.alignment=alignment;
		}

		internal override RawCmd getRawCommand() {
			if (!withAlignment) {
				return (new RawCmdText(text,false));
			} else {
				return (new RawCmdText(text,alignment,false));
			}
		}

	}

}
