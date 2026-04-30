using Bonsai.Design;
using Bonsai.Expressions;
using Hexa.NET.ImGui;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Bonsai.ImGui.Design;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Provides an abstract base class for composing visualizer infrastructure for displaying
/// graphical user interfaces constructed using Dear ImGui.
/// </summary>
public abstract class ImGuiMashupVisualizer : MashupVisualizer
{
    ImGuiControl imGuiControl;

    internal ImGuiControl Control => imGuiControl;

    /// <summary>
    /// Gets or sets the target interval, in milliseconds, between visualizer updates.
    /// </summary>
    protected virtual int TargetInterval => 1000 / 50;

    /// <inheritdoc/>
    public override void Load(IServiceProvider provider)
    {
        if (provider.GetService(typeof(MashupVisualizer)) is ImGuiMashupVisualizer imGuiVisualizer &&
            imGuiVisualizer.Control is ImGuiControl mashupControl)
        {
            foreach (var extension in GetExtensions())
                mashupControl.Extensions.Add(extension);
        }
        else
        {
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var controlBuilder = (ImGuiMashupVisualizerBuilder)ExpressionBuilder.GetVisualizerElement(context.Source).Builder;
            var windowName = controlBuilder.Name ?? controlBuilder.GetType().Name;

            imGuiControl = new ImGuiControl();
            imGuiControl.Dock = DockStyle.Fill;
            foreach (var extension in GetExtensions())
                imGuiControl.Extensions.Add(extension);
            imGuiControl.Render += (sender, e) =>
            {
                ImGui.StyleColorsLight();
                var dockspaceId = ImGui.DockSpaceOverViewport(
                    dockspaceId: 0,
                    ImGui.GetMainViewport(),
                    ImGuiDockNodeFlags.AutoHideTabBar | ImGuiDockNodeFlags.NoUndocking);

                if (ImGui.Begin(windowName))
                {
                    controlBuilder._Render.OnNext(Unit.Default);
                }

                ImGui.End();

                if (!ImGui.IsWindowDocked() &&
                    ImGuiP.DockBuilderGetCentralNode(dockspaceId) is ImGuiDockNodePtr node &&
                    !node.IsNull)
                {
                    ImGuiP.DockBuilderDockWindow(windowName, node.ID);
                }
            };

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            visualizerService?.AddControl(imGuiControl);
            base.Load(provider);
        }
    }

    /// <summary>
    /// Returns the extensions to initialize with the Dear ImGui backend infrastructure.
    /// </summary>
    /// <returns>
    /// A sequence of <see cref="IExtensionFactory"/> objects used to initialize the
    /// Dear ImGui context. Initialization follows the order of objects in this sequence.
    /// </returns>
    protected abstract IEnumerable<IExtensionFactory> GetExtensions();

    /// <inheritdoc/>
    public override IObservable<object> Visualize(IObservable<IObservable<object>> source, IServiceProvider provider)
    {
        if (provider.GetService(typeof(IDialogTypeVisualizerService)) is not Control visualizerControl)
        {
            return source;
        }

        return Observable.Using(
            () => new Timer(),
            timer =>
            {
                var onError = Observable.FromEventPattern<ErrorEventArgs>(
                    handler => imGuiControl.Error += handler,
                    handler => imGuiControl.Error -= handler)
                    .SelectMany(evt => Observable.Throw<EventPattern<EventArgs>>(
                        new InvalidOperationException(evt.EventArgs.Message)));
                timer.Interval = TargetInterval;
                var timerTick = Observable.FromEventPattern<EventHandler, EventArgs>(
                    handler => timer.Tick += handler,
                    handler => timer.Tick -= handler);
                timer.Start();
                return timerTick
                  .Do(_ => imGuiControl?.Invalidate())
                  .Merge(onError)
                  .Finally(timer.Stop);
            });
    }

    /// <inheritdoc/>
    public override void Show(object value)
    {
    }

    /// <inheritdoc/>
    public override void Unload()
    {
        base.Unload();
        imGuiControl?.Dispose();
        imGuiControl = null;
    }
}
