using Pipboy.Wallpaper.ViewModels;

namespace Pipboy.Wallpaper.Abstractions;

public interface IDataContextFacade
{
    EffectViewModel EffectData { get; }
    TextViewModel TextData { get; }
}
