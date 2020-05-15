using System;

#if MODERNFORMS
using Modern.Forms;
#else
using System.Windows.Forms;
#endif

#if MODERNFORMS
namespace CefNet.Modern.Forms
#else
namespace CefNet.Windows.Forms
#endif
{
public class CursorChangeEventArgs : EventArgs
	{
		public CursorChangeEventArgs(Cursor cursor, CefCursorType cursorType)
		{
			this.Cursor = cursor;
			this.CursorType = cursorType;
		}

		public Cursor Cursor { get; }

		public CefCursorType CursorType { get; }
	}
}
