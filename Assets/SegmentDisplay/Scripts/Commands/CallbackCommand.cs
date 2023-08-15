//    SegmentDisplay - Controller - Command - Callback


using System;

namespace Leguar.SegmentDisplay {
	
	/// <summary>
	/// Command to call certain action when this command is reached. Useful if you need to get notification when SegmentDisplay have reached certain point in SDController command queue.
	/// </summary>
	public class CallbackCommand : AbsSDCommand {
		
		private Action callback;
		
		/// <summary>
		/// Creates new callback command. When SDController reaches this command, callback is called and SDController continues from next command in queue.
		/// </summary>
		/// <param name="callback">
		/// Action to be called.
		/// </param>
		public CallbackCommand(Action callback) {
			this.callback=callback;
		}
		
		internal override RawCmd getRawCommand() {
			return (new RawCmdCallback(callback));
		}
		
	}

}
