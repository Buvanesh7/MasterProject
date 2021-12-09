using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace AkryazTools.WindowHandle
{
	internal class WindowHandler
	{
		/// <summary>
		/// Sets the foreground ActiveWindow.
		/// </summary>
		/// <param name="hWnd">The h WND.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetForegroundWindow(IntPtr hWnd);

		/// <summary>
		/// Sets the ActiveWindow owner.
		/// </summary>
		/// <param name="application">The application.</param>
		/// <param name="window">The ActiveWindow.</param>
		public static void SetWindowOwner(UIApplication application, Window window)
		{
			IWin32Window revitWindow = new JtWindowHandle(Autodesk.Windows.ComponentManager.ApplicationWindow);
			WindowInteropHelper helper = new WindowInteropHelper(window)
			{
				Owner = revitWindow.Handle
			};
		}

		/// <summary>
		/// Gets the ActiveWindow handle.
		/// </summary>
		/// <param name="window">The ActiveWindow.</param>
		/// <returns></returns>
		public static int GetWindowHandle(Window window)
		{
			WindowInteropHelper helper = new WindowInteropHelper(window);
			return helper.Handle.ToInt32();
		}

		/// <summary>
		/// Activates the ActiveWindow.
		/// </summary>
		/// <returns></returns>
		public static bool ActivateWindow()
		{
			IntPtr ptr = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

			if (ptr != IntPtr.Zero) return SetForegroundWindow(ptr);

			return false;
		}
	}
}


