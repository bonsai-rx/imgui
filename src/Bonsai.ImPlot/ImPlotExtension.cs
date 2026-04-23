using Bonsai.ImGui;
using Hexa.NET.ImGui;
using Hexa.NET.ImPlot;

namespace Bonsai.ImPlot;
using ImPlot = Hexa.NET.ImPlot.ImPlot;

/// <summary>
/// Provides an interface to initialize and manipulate ImPlot context handles.
/// </summary>
public class ImPlotExtension : IExtensionContext
{
    readonly ImPlotContextPtr plotContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImPlotExtension"/> class.
    /// </summary>
    /// <param name="guiContext">
    /// The handle to the ImGui context to be linked with ImPlot.
    /// </param>
    public ImPlotExtension(ImGuiContextPtr guiContext)
    {
        ImPlot.SetImGuiContext(guiContext);
        plotContext = ImPlot.CreateContext();
        ImPlot.SetCurrentContext(plotContext);
        ImPlot.StyleColorsLight(ImPlot.GetStyle());
    }

    /// <summary>
    /// Releases all resources held by the <see cref="ImPlotExtension"/>.
    /// </summary>
    public void Dispose()
    {
        ImPlot.SetCurrentContext(null);
        ImPlot.DestroyContext(plotContext);
    }

    /// <inheritdoc/>
    public void MakeCurrent(ImGuiContextPtr guiContext)
    {
        ImPlot.SetCurrentContext(plotContext);
        ImPlot.SetImGuiContext(guiContext);
    }

    /// <summary>
    /// Returns the default factory used to create new ImPlot context handles.
    /// </summary>
    public static IExtensionFactory Factory => ImPlotExtensionFactory.Default;

    class ImPlotExtensionFactory() : IExtensionFactory
    {
        internal static readonly ImPlotExtensionFactory Default = new();

        public IExtensionContext CreateContext(ImGuiContextPtr guiContext) => new ImPlotExtension(guiContext);
    }
}
