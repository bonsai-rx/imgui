using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Linq;

namespace Bonsai.ImGui;

internal class SliderFlagsConverter : EnumConverter
{
    public SliderFlagsConverter(Type type)
        : base(type)
    {
    }

    public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    {
        return new StandardValuesCollection(
            Enum.GetValues(typeof(ImGuiSliderFlags))
                .Cast<ImGuiSliderFlags>()
                .Where(flag =>
                    (flag & ImGuiSliderFlags.WrapAround) == 0 &&
                    (flag & ImGuiSliderFlags.InvalidMask) == 0)
                .ToArray());
    }
}
