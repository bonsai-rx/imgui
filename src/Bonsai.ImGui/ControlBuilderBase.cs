using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for common UI control functionality.
/// </summary>
/// <typeparam name="TResult">The type of event notifications emitted by the control.</typeparam>
[DefaultProperty(nameof(Name))]
public abstract class ControlBuilderBase<TResult> : SingleArgumentExpressionBuilder, INamedElement
{
    /// <summary>
    /// Gets or sets the name of the control.
    /// </summary>
    [Category(nameof(CategoryAttribute.Design))]
    [Description("The name of the control.")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value specifying whether the control and all its child controls
    /// are displayed.
    /// </summary>
    [Category(nameof(CategoryAttribute.Behavior))]
    [Description("Specifies whether the control and all its child controls are displayed.")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Builds the expression tree for configuring and rendering the UI control.
    /// </summary>
    /// <inheritdoc/>
    public override Expression Build(IEnumerable<Expression> arguments)
    {
        var source = arguments.First();
        var parameterType = source.Type.GetGenericArguments()[0];
        return Expression.Call(Expression.Constant(this), nameof(Generate), new[] { parameterType }, source);
    }

    /// <summary>
    /// Generates an observable sequence that draws the UI control and emits any
    /// event notifications.
    /// </summary>
    /// <param name="source">
    /// An observable sequence of notifications used to render the UI control.
    /// </param>
    /// <returns>
    /// An observable sequence of notifications of type <typeparamref name="TResult"/>
    /// emitted by the UI control.
    /// </returns>
    protected abstract IObservable<TResult> Generate<TSource>(IObservable<TSource> source);
}
