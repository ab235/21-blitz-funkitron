//    SegmentDisplay - Controller - Command - Pause


namespace Leguar.SegmentDisplay {
	
	/// <summary>
	/// Command to add delay between other commands. Whatever is on display when this command is reached will stay there. Controller continues from next command after this delay.
	/// </summary>
	public class PauseCommand : AbsSDCommand {
		
		private float delaySeconds;
		
		/// <summary>
		/// Creates new pause command.
		/// </summary>
		/// <param name="delaySeconds">
		/// Seconds how long SDController will do nothing.
		/// </param>
		public PauseCommand(float delaySeconds) {
			this.delaySeconds=delaySeconds;
		}
		
		internal override RawCmd getRawCommand() {
			return (new RawCmdDelay(delaySeconds));
		}
		
	}

}
