using System.Diagnostics.CodeAnalysis;
using TehPers.Core.Api.Json;

namespace TehPers.FishingFramework.Config
{
    [JsonDescribe]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Local", Justification = "Private setters are called by the JSON deserializer through reflection.")]
    public class DifficultyConfiguration
    {
    }
}