using System;
using System.Collections.Generic;
using System.Text;

namespace Deque
{
	public static class DequeTest
	{
		public static IList<T> GetReverseView<T>(Deque<T> d)
		{
			return d.Reverse();
		}
	}
}
