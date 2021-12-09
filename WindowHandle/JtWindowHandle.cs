using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace AkryazTools.WindowHandle
{
    internal class JtWindowHandle : IWin32Window
	{
		#region PRIVATE MEMBERS
		/// <summary>
		/// The HWND
		/// </summary>
		private readonly IntPtr _hwnd;

		#endregion

		#region PUBLIC PROPERTIES

		/// <summary>
		/// Gets the ActiveWindow handle.
		/// </summary>
		public IntPtr Handle { get { return _hwnd; } }

		#endregion

		#region Public Methods

		/// <summary>
		/// Initializes a new instance of the <see cref="JtWindowHandle"/> class.
		/// </summary>
		/// <param name="h">The h.</param>
		public JtWindowHandle(IntPtr h)
		{
			Debug.Assert(IntPtr.Zero != h,
				"expected non-null ActiveWindow handle");

			_hwnd = h;
		}

		#endregion
	}
}
