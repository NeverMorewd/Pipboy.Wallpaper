using Pipboy.Wallpaper.Utils;
using System.Text.Json.Serialization;
using System.Windows.Media;

namespace Pipboy.Wallpaper.Models;

public class TextOptionsDto
{
    public string Title { get; set; } = "1984";
    public string Content { get; set; } = "BIG BROTHER IS WATCHING YOU";
    public bool ShowDatetime { get; set; } = true;
    public bool ShowFps { get; set; } = true;
    public string FontFamily { get; set; } = "Courier New";
    [JsonPropertyName("ForegroundColor")]
    public string ForegroundColorHex { get; set; } = "#FF00FF00";

    public Int32 ContentFontSize { get; set; } = 24;

    [JsonIgnore]
    public Color ForegroundColor
    {
        get => ColorUtils.HexToColor(ForegroundColorHex);
        set => ForegroundColorHex = ColorUtils.ColorToHex(value);
    }
}
