using System;
using System.Linq;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using log4net;
using Sonnenberg.Core.Interfaces;

namespace Sonnenberg.Infrastructure.EventHandling
{
    /// <summary>
    /// Manages the state of the context menu and listens for user actions to reset state
    /// when the context menu is closed (via mouse click, keyboard, etc.)
    /// </summary>
    public class ContextMenuStateManager : IContextMenuStateManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ContextMenuStateManager));
        private static readonly int[] CloseKeys = { 127, 27, 24, 8 }; // DEL, ESC, Ctrl+X, Backspace

        private IKeyboardMouseEvents _globalHook;
        private bool _isSubscribed;
        private bool _disposedValue;

        public void SubscribeIfNeeded()
        {
            if (_isSubscribed)
                return;

            try
            {
                _globalHook = Hook.GlobalEvents();
                _globalHook.MouseDownExt += OnMouseDown;
                _globalHook.KeyPress += OnKeyPress;
                _isSubscribed = true;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to subscribe to global mouse/keyboard events", ex);
            }
        }

        public void ResetState()
        {
            // Reset state logic will be implemented by the configuration manager
            Unsubscribe();
        }

        private void OnMouseDown(object sender, MouseEventExtArgs e)
        {
            if (e.IsMouseButtonDown)
            {
                ResetState();
            }
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (CloseKeys.Contains(e.KeyChar))
            {
                ResetState();
            }
        }

        private void Unsubscribe()
        {
            if (_globalHook == null || !_isSubscribed)
                return;

            try
            {
                _globalHook.MouseDownExt -= OnMouseDown;
                _globalHook.KeyPress -= OnKeyPress;
                _globalHook.Dispose();
                _globalHook = null;
                _isSubscribed = false;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to unsubscribe from global events", ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            if (disposing)
            {
                Unsubscribe();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
