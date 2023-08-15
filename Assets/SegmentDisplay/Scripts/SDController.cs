//    SegmentDisplay - Controller


using UnityEngine;
using System.Collections.Generic;

namespace Leguar.SegmentDisplay {
	
	/// <summary>
	/// Class that runs given commands which then change content on the segment display. Commands added to controller are executed one by one in queue.
	/// 
	/// This class doesn't have public constructor. You can get the instance of this class from SegmentDisplay, using GetSDController() method.
	/// 
	/// Note that you do not necessarily need this controller. If you just want to set text on display, you can use SetText() methods in SegmentDisplay.
	/// This controller is useful if you want to do something that requires changes to display over longer period of time.
	/// </summary>
	public class SDController {
		
		private SegmentDisplay segmentDisplay;

		private List<AbsSDCommand> commands;
		private RawCmd currentRawCommand;
		private float timeToConsume;

		internal SDController(SegmentDisplay segmentDisplay) {
			this.segmentDisplay=segmentDisplay;
			commands=new List<AbsSDCommand>();
			currentRawCommand=null;
			timeToConsume=0f;
		}
		
		internal void update(float deltaTime) {
			// Always run one step
			int cmdCount=commands.Count;
			singleExecute(deltaTime);
			// Run multiple steps if command were instant and queue is getting shorter (so will not get stuck to repeating commands)
			while (currentRawCommand==null && commands.Count<cmdCount) {
				cmdCount=commands.Count;
				singleExecute(0f);
			}
		}

		private void singleExecute(float newTimeToConsume) {
			
			// Get new raw command if nothing is running at the moment
			if (currentRawCommand==null) {
				// If there isn't anything in queue, nothing to do
				if (commands.Count==0) {
					timeToConsume=0f;
					return;
				}
				// Get command
				AbsSDCommand command=commands[0];
				commands.RemoveAt(0);
				currentRawCommand=command.getRawCommand();
				// Possibly add back to queue
				if (command.isRepeat()) {
					commands.Add(command);
				}
			}
			
			// Run current raw command
			timeToConsume+=newTimeToConsume;
			timeToConsume=currentRawCommand.runStep(segmentDisplay,timeToConsume);
			
			// End if finished
			if (currentRawCommand.isFinished(segmentDisplay)) {
				currentRawCommand=null;
			}
			
		}

		/// <summary>
		/// Add new command to this controller queue. If queue is empty, execution of this command will start instantly on this or next Update loop.
		/// Controller will run this command just once and then moving to next command in queue if any.
		/// </summary>
		/// <param name="command">
		/// Command to add to queue.
		/// </param>
		public void AddCommand(AbsSDCommand command) {
			commands.Add(command);
		}

		/// <summary>
		/// Add new command to this controller queue. If queue is empty, execution of this command will start instantly on this or next update loop.
		/// Controller will repeat this command forever. If there are multiple repeating commands, they are all looping in order they were added.
		/// </summary>
		/// <param name="command">
		/// Command to add to queue and repeat.
		/// </param>
		public void AddRepeatingCommand(AbsSDCommand command) {
			command.setRepeat();
			commands.Add(command);
		}

		/// <summary>
		/// Clear the controller commmand queue. Command that is currently being executed, if any, will finish normally.
		/// After finishing current command, controller will be idle unless new commands are added.
		/// </summary>
		public void ClearCommands() {
			commands.Clear();
		}
		
		/// <summary>
		/// Clear the controller commmand queue and stop any possible currently executing command. Controller will be idle immediately after this.
		/// Display is not cleared so whatever is on display when calling this will remain there.
		/// </summary>
		public void ClearCommandsAndStop() {
			commands.Clear();
			currentRawCommand=null;
			timeToConsume=0f;
		}
		
		/// <summary>
		/// Check whatever controller is idle.
		/// </summary>
		/// <returns>
		/// True if controller isn't currently executing any command and there is nothing in queue. False otherwise.
		/// </returns>
		public bool IsIdle() {
			return (currentRawCommand==null && commands.Count==0);
		}
		
	}

}
