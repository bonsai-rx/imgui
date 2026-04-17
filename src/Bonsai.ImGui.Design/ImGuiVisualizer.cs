using Bonsai.Design;
using Bonsai.Expressions;
using Hexa.NET.ImGui;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Windows.Forms;

namespace Bonsai.ImGui.Design;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Provides backend infrastructure for displaying the contents of a graphical
/// user interface constructed using Dear ImGui.
/// </summary>
public class ImGuiVisualizer : BufferedVisualizer
{
    ImGuiControl imGuiControl;

    /// <inheritdoc/>
    public override void Load(IServiceProvider provider)
    {
        var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
        var controlBuilder = (ImGuiVisualizerBuilder)ExpressionBuilder.GetVisualizerElement(context.Source).Builder;

        imGuiControl = new ImGuiControl();
        imGuiControl.Dock = DockStyle.Fill;
        imGuiControl.Render += (sender, e) =>
        {
            ImGui.StyleColorsLight();
            var dockspaceId = ImGui.DockSpaceOverViewport(
                dockspaceId: 0,
                ImGui.GetMainViewport(),
                ImGuiDockNodeFlags.AutoHideTabBar | ImGuiDockNodeFlags.NoUndocking);

            if (ImGui.Begin(nameof(ImGuiVisualizer)))
            {
                controlBuilder._Render.OnNext(Unit.Default);
            }

            ImGui.End();

            if (!ImGui.IsWindowDocked() &&
                ImGuiP.DockBuilderGetCentralNode(dockspaceId) is ImGuiDockNodePtr node &&
                !node.IsNull)
            {
                ImGuiP.DockBuilderDockWindow(nameof(ImGuiVisualizer), node.ID);
            }
        };

        var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
        visualizerService?.AddControl(imGuiControl);
    }

    /// <inheritdoc/>
    protected override void ShowBuffer(IList<Timestamped<object>> values)
    {
        imGuiControl?.Invalidate();
    }

    /// <inheritdoc/>
    public override void Show(object value)
    {
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        imGuiControl?.Dispose();
        imGuiControl = null;
    }
}
