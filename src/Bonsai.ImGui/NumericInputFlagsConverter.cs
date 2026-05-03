using Hexa.NET.ImGui;
using System;
using System.ComponentModel;

namespace Bonsai.ImGui;

internal class NumericInputFlagsConverter : EnumConverter
{
    public NumericInputFlagsConverter(Type type)
        : base(type)
    {
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        return new StandardValuesCollection(new[]
        {
            ImGuiInputTextFlags.None,
            ImGuiInputTextFlags.ReadOnly,
            ImGuiInputTextFlags.NoUndoRedo,
            ImGuiInputTextFlags.ParseEmptyRefVal,
            ImGuiInputTextFlags.DisplayEmptyRefVal,
        });
    }
}
