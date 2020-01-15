using System.Linq;
using Gma.System.MouseKeyHook;
using System.Windows.Forms;

namespace Sonnenberg.ShellServer.MouseKeyHook
{
    public class MenuWatcher
    {
        private static IKeyboardMouseEvents m_GlobalHook;
        
        internal void Subscribe()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += (o, args) => GlobalMouseEvents(o, args);
            m_GlobalHook.KeyPress += (o, args) => GlobalKeyboardEvents(o, args);
        }

        private void GlobalMouseEvents(object sender, MouseEventExtArgs e)
        {
            if (e.IsMouseButtonDown)
            {
                PublishCustomEvent();
            }
        }
        
        private void GlobalKeyboardEvents(object sender, KeyPressEventArgs e)
        {
            // DEL, ESC, cancel, backspace
            var array = new [] {127, 27, 24, 8};
            var key = e.KeyChar;

            if (array.Contains(key))
            {
                PublishCustomEvent();
            }
        }
        
        private void PublishCustomEvent()
        {
            Helper.ResetSettings();
            Unsubscribe();
        }

        private void Unsubscribe()
        {
            m_GlobalHook.MouseDownExt -= (o, args) => GlobalMouseEvents(o, args);
            m_GlobalHook.KeyPress -= (o, args) => GlobalKeyboardEvents(o, args);
            m_GlobalHook.Dispose();
        }
    }
}