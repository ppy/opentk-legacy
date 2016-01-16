#region License
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2010 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using OpenTK.Input;

namespace OpenTK.Platform.Windows
{
    abstract class WinInputBase
    {
        #region Fields

        readonly WindowProcedure WndProc;
        
        IntPtr OldWndProc;
        INativeWindow native;

        protected WinWindowInfo Parent;

        static readonly IntPtr Unhandled = new IntPtr(-1);

        #endregion

        #region Constructors

        public WinInputBase()
        {
            WndProc = WndProcHandler;

            Parent = new WinWindowInfo(NativeWindow.OsuWindowHandle, null);
            CreateDrivers();

            // Subclass the window to retrieve the events we are interested in.
            OldWndProc = Functions.SetWindowLong(Parent.Handle, WndProc);
            Debug.Print("Input window attached to {0}", Parent);
        }

        #endregion

        #region Private Members

        #region WndProcHandler

        IntPtr WndProcHandler(
            IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam)
        {
            IntPtr ret =  WindowProcedure(handle, message, wParam, lParam);
            if (ret == Unhandled)
                return Functions.CallWindowProc(OldWndProc, handle, message, wParam, lParam);
            else
                return ret;
        }

        #endregion

        #endregion

        #region Protected Members

        #region WindowProcedure

        protected virtual IntPtr WindowProcedure(
            IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam)
        {
            return Unhandled;
        }

        #endregion

        #region CreateDrivers

        // Note: this method is called through the input thread.
        protected abstract void CreateDrivers();

        #endregion

        #endregion

        #region Public Members

        public abstract IMouseDriver2 MouseDriver { get; }

        public abstract IKeyboardDriver2 KeyboardDriver { get; }

        public abstract IJoystickDriver2 JoystickDriver { get; }

        #endregion

        #region IDisposable Members

        protected bool Disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool manual)
        {
            if (!Disposed)
            {
                Disposed = true;
            }
        }

        ~WinInputBase()
        {
            Debug.Print("[Warning] Resource leaked: {0}.", this);
            Dispose(false);
        }

        #endregion
    }
}
