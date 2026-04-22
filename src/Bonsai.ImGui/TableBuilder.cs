using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that begins drawing a table and provides
/// a sequence of notifications for drawing table contents.
/// </summary>
[Description("Begins drawing a table and provides a sequence of notifications for drawing table contents.")]
public class TableBuilder : ControlBuilderBase<string>
{
    /// <summary>
    /// Gets or sets the number of columns in the table.
    /// </summary>
    [Description("The number of columns in the table.")]
    public int Columns { get; set; } = 1;

    /// <summary>
    /// Gets or sets the table configuration flags.
    /// </summary>
    [Description("The table configuration flags.")]
    public ImGuiTableFlags Flags { get; set; }

    /// <summary>
    /// Gets or sets the explicit outer size of the table.
    /// </summary>
    /// <remarks>
    /// The meaning of this property depends on the table configuration flags.
    /// </remarks>
    [TypeConverter(typeof(NumericRecordConverter))]
    [Description("The explicit outer size of the table.")]
    public Vector2 OuterSize { get; set; }

    /// <summary>
    /// Gets or sets the optional total width to layout columns for horizontal scrolling.
    /// </summary>
    /// <remarks>
    /// This value is ignored when <see cref="ImGuiTableFlags.ScrollX"/> is disabled.
    /// </remarks>
    [Description("The optional total width to layout columns for horizontal scrolling.")]
    public float InnerWidth { get; set; }

    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var label = $"##{Name ?? "Table"}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.BeginTable(label, Columns, Flags, OuterSize, InnerWidth))
                    {
                        observer.OnNext(label);
                        ImGui.EndTable();
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
