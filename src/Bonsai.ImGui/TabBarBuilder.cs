using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that begins drawing a tab bar and provides
/// a sequence of notifications for drawing the tab items.
/// </summary>
[WorkflowElementIcon("Bonsai:ElementIcon.Visualizer")]
[Description("Begins drawing a tab bar and provides a sequence of notifications for drawing the tab items.")]
public class TabBarBuilder : ControlBuilder<string>
{
    /// <summary>
    /// Gets or sets the tab bar configuration flags.
    /// </summary>
    [Description("The tab bar configuration flags.")]
    public ImGuiTabBarFlags Flags { get; set; }

    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var label = $"##{Name ?? "TabBar"}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.BeginTabBar(label, Flags))
                    {
                        observer.OnNext(label);
                        ImGui.EndTabBar();
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
