//    SegmentDisplay - RawCommand - Text


using UnityEngine;

namespace Leguar.SegmentDisplay {
	
	internal class RawCmdText : RawCmd {

		private string text;
		private bool withAlignment;
		private SegmentDisplay.Alignments alignment;
		private bool setText;

		internal RawCmdText(string text, bool setText) {
			this.text=text;
			withAlignment=false;
			this.setText=setText;
		}

		internal RawCmdText(string text, SegmentDisplay.Alignments alignment, bool setText) {
			this.text=text;
			withAlignment=true;
			this.alignment=alignment;
			this.setText=setText;
		}

		internal override float runStep(SegmentDisplay segmentDisplay, float deltaTime) {

			if (setText) {
				if (!withAlignment) {
					segmentDisplay.SetText(text);
				} else {
					segmentDisplay.SetText(text,alignment);
				}
			} else {
				if (!withAlignment) {
					segmentDisplay.AddText(text);
				} else {
					segmentDisplay.AddText(text,alignment);
				}
			}
			return deltaTime;

		}
		
		internal override bool isFinished(SegmentDisplay segmentDisplay) {
			return true;
		}
		
	}

}
