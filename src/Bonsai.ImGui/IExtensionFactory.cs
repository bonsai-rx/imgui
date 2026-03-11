using Hexa.NET.ImGui;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an interface for creating a custom ImGui context extension.
/// </summary>
public interface IExtensionFactory
{
    /// <summary>
    /// Creates an ImGui context extension on the specified ImGui context.
    /// </summary>
    /// <param name="guiContext">The pointer to the ImGui context used for rendering.</param>
    /// <returns>An <see cref="IExtensionContext"/> representing the created context extension.</returns>
    IExtensionContext CreateContext(ImGuiContextPtr guiContext);
}
