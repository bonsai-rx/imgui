using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Subjects;

namespace Bonsai.ImGui.Design;

/// <summary>
/// Provides an abstract base class for operators that configure an immediate mode
/// visualizer backend using Dear ImGui.
/// </summary>
[WorkflowElementIcon("Bonsai:ElementIcon.Visualizer")]
public abstract class ImGuiMashupVisualizerBuilder : ZeroArgumentExpressionBuilder, INamedElement
{
    internal readonly Subject<Unit> _Render = new();

    /// <summary>
    /// Gets or sets the name of the visualizer.
    /// </summary>
    [Category(nameof(CategoryAttribute.Design))]
    [Description("The name of the visualizer.")]
    public string Name { get; set; }

    /// <inheritdoc/>
    public override Expression Build(IEnumerable<Expression> arguments)
    {
        return Expression.Convert(Expression.Constant(_Render), typeof(IObservable<Unit>));
    }
}
