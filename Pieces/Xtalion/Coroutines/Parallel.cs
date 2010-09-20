﻿#region Author

//// Yevhen Bobrov, http://blog.xtalion.com 

#endregion

using System;
using System.Threading;

namespace Xtalion.Coroutines
{
	public static class Parallel
	{
		public static IAction Actions(params IAction[] actions)
		{
			return new ParallelAction(actions);
		}

		private class ParallelAction : IAction
		{
			public event EventHandler Completed;

			readonly IAction[] actions;
			int remaining;

			public ParallelAction(IAction[] actions)
			{
				this.actions = actions;
			}

			public void Execute()
			{
				remaining = actions.Length;

				foreach (IAction action in actions)
				{
					action.Completed += delegate
					{
						if (Interlocked.Decrement(ref remaining) == 0)
							SignalCompleted();
					};

					action.Execute();
				}
			}

			void SignalCompleted()
			{
				EventHandler handler = Completed;

				if (handler != null)
					Dispatcher.Current.Invoke(() => handler(this, EventArgs.Empty));
			}
		}
	}
}
