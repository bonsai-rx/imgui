using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that returns the current ImGui input/output state
/// for each notification in the source sequence.
/// </summary>
[WorkflowElementIcon("Bonsai:ElementIcon.Hid")]
[Description("Returns the current ImGui input/output state for each notification in the source sequence.")]
public class GetIOBuilder : ControlBuilder<IOState>
{
    /// <summary>
    /// Returns an observable sequence reporting the current ImGui input/output state,
    /// sampled once for each notification emitted by the <paramref name="source"/> sequence.
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the source sequence.</typeparam>
    /// <param name="source">A sequence of notifications used to sample the IO state.</param>
    /// <returns>
    /// An observable sequence of <see cref="IOState"/> values reflecting the ImGui
    /// input/output state whenever the source sequence emits a notification.
    /// </returns>
    protected override IObservable<IOState> Generate<TSource>(IObservable<TSource> source)
    {
        return source.Select(_ => new IOState(ImGui.GetIO()));
    }
}
