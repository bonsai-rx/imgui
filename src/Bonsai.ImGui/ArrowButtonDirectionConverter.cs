using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Linq;

namespace Bonsai.ImGui;

internal class ArrowButtonDirectionConverter : EnumConverter
{
    public ArrowButtonDirectionConverter(Type type)
        : base(type)
    {
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        return new StandardValuesCollection(
            Enum.GetValues(typeof(ImGuiDir))
                .Cast<ImGuiDir>()
                .Where(dir => dir != ImGuiDir.None && dir != ImGuiDir.Count)
                .ToArray());
    }
}
