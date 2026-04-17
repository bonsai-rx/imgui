using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Subjects;

namespace Bonsai.ImGui.Design;

/// <summary>
/// Represents an operator that configures an immediate mode visualizer
/// backend using Dear ImGui.
/// </summary>
[TypeVisualizer(typeof(ImGuiVisualizer))]
[Description("Configures an immediate mode visualizer backend using Dear ImGui.")]
public class ImGuiVisualizerBuilder : ZeroArgumentExpressionBuilder
{
    internal readonly Subject<Unit> _Render = new();

    /// <inheritdoc/>
    public override Expression Build(IEnumerable<Expression> arguments)
    {
        return Expression.Convert(Expression.Constant(_Render), typeof(IObservable<Unit>));
    }
}
