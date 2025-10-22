using System;

namespace Sonnenberg.Core.Interfaces
{
    /// <summary>
    /// Manages the state of the context menu and resets it when the menu closes
    /// </summary>
    public interface IContextMenuStateManager : IDisposable
    {
        void SubscribeIfNeeded();
        void ResetState();
    }
}
