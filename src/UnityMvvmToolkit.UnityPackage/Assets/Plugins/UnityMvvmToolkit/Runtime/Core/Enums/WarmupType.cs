namespace UnityMvvmToolkit.Core.Enums
{
    public enum WarmupType
    {
        /// <summary>
        /// Use this type if you don't specify a converter in the view.
        /// <example>command="PrintIntParameterCommand, 5"</example>
        /// </summary>
        OnlyByType,

        /// <summary>
        /// Use this type if you specify a converter in the view.
        /// <example>command="PrintIntParameterCommand, 5, ParameterToIntConverter"</example>
        /// </summary>
        OnlyByName,

        /// <summary>
        /// Use this type to cover <see cref="OnlyByType"/> and <see cref="OnlyByName"/> scenarios.
        /// </summary>
        ByTypeAndName
    }
}