using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for common UI text controls.
/// </summary>
/// <typeparam name="TResult">The type of event notifications emitted by the UI control.</typeparam>
public abstract class TextControlBuilderBase<TResult> : ControlBuilderBase<TResult>, INamedElement
{
    /// <summary>
    /// Gets or sets the text associated with this control.
    /// </summary>
    [Category(nameof(CategoryAttribute.Appearance))]
    [Description("The text associated with this control.")]
    public string Text { get; set; }
}
