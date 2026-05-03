using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Linq;

namespace Bonsai.ImGui;

internal class DragFlagsConverter : EnumConverter
{
    public DragFlagsConverter(Type type)
        : base(type)
    {
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        return new StandardValuesCollection(
            Enum.GetValues(typeof(ImGuiSliderFlags))
                .Cast<ImGuiSliderFlags>()
                .Where(flag => (flag & ImGuiSliderFlags.InvalidMask) == 0)
                .ToArray());
    }
}
