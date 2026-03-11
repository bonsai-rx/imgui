using Hexa.NET.ImGui;
using System;

namespace Bonsai.ImGui;

/// <summary>
/// Provides a standard interface for custom ImGui context extensions.
/// </summary>
public interface IExtensionContext : IDisposable
{
    /// <summary>
    /// Activates the ImGui context extension with the specified ImGui context.
    /// </summary>
    /// <param name="guiContext">The pointer to the ImGui context used for rendering.</param>
    void MakeCurrent(ImGuiContextPtr guiContext);
}
