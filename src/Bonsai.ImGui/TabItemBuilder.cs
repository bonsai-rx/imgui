using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that begins drawing a tab item inside a tab bar
/// and provides a sequence of notifications for drawing the tab contents.
/// </summary>
[WorkflowElementIcon("Bonsai:ElementIcon.Visualizer")]
[Description("Begins drawing a tab item inside a tab bar and provides a sequence of notifications for drawing the tab contents.")]
public class TabItemBuilder : TextControlBuilder<string>
{
    /// <summary>
    /// Gets or sets the tab item configuration flags.
    /// </summary>
    [Description("The tab item configuration flags.")]
    public ImGuiTabItemFlags Flags { get; set; }

    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var capturedName = Name;
            var capturedText = Text;
            var idSuffix = $"##{capturedName ?? "TabItem"}";
            var label = capturedText + idSuffix;
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    var text = Text;
                    if (text != capturedText)
                    {
                        capturedText = text;
                        label = text + idSuffix;
                    }
                    if (Visible && ImGui.BeginTabItem(label, Flags))
                    {
                        observer.OnNext(label);
                        ImGui.EndTabItem();
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
