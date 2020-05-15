using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CefNet.WinApi;
using Modern.Forms;

namespace CefNet.Modern.Forms
{
	public static class CefNetModernFormsExtensions
	{
		/// <summary>
		/// Converts the specified <see cref="CefRect"/> to a <see cref="Rectangle"/>.
		/// </summary>
		/// <param name="self">The <see cref="CefRect"/> to be converted.</param>
		/// <returns>The new <see cref="Rectangle"/> that this method creates.</returns>
		public static Rectangle ToRectangle(ref this CefRect self)
		{
			return new Rectangle(self.X, self.Y, self.Width, self.Height);
		}

		/// <summary>
		/// Converts the specified <see cref="Rectangle"/> to a <see cref="CefRect"/>.
		/// </summary>
		/// <param name="self">The <see cref="Rectangle"/> to be converted.</param>
		/// <returns>The new <see cref="CefRect"/> that this method creates.</returns>
		public static CefRect ToCefRect(ref this Rectangle self)
		{
			return new CefRect(self.X, self.Y, self.Width, self.Height);
		}

		/// <summary>
		/// Converts the specified key code to a virtual key code.
		/// </summary>
		/// <param name="key">The key code to be converted.</param>
		/// <returns>The virtual key code value.</returns>
		public static VirtualKeys ToVirtualKey(this Keys key)
		{
			if (key >= Keys.LShiftKey && key <= Keys.RMenu)
				return (VirtualKeys)((key - Keys.LShiftKey) >> 1) | VirtualKeys.ShiftKey; // VK_SHIFT, VK_CONTROL, VK_MENU
			return (VirtualKeys)key;
		}

		/// <summary>
		/// Translates a virtual key to the corresponding native key code for the current keyboard.
		/// </summary>
		/// <param name="isRepeat">The repeat flag.</param>
		/// <param name="eventType">The key event type.</param>
		/// <param name="modifiers">A bitwise combination of the <see cref="CefEventFlags"/> values.</param>
		/// <param name="key">The virtual key.</param>
		/// <param name="isExtended">The extended key flag.</param>
		/// <returns>A native key code for the current keyboard.</returns>
		public static int ToNativeKeyCode(this VirtualKeys key, CefKeyEventType eventType, bool isRepeat, CefEventFlags modifiers, bool isExtended)
		{
			return CefNetApi.GetNativeKeyCode(eventType, isRepeat ? 1 : 0, key, modifiers, isExtended);
		}

	}
}
