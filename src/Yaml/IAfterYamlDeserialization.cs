using YamlDotNet.Core;

namespace RoboPhredDev.Shipbreaker.SixAxis.Yaml
{

    /// <summary>
    /// An interface to handle post-deserialization operations.
    /// </summary>
    public interface IAfterYamlDeserialization
    {
        /// <summary>
        /// Validates the newly deserialized object.
        /// </summary>
        /// <param name="start">The start of this object in the yaml file.</param>
        /// <param name="end">The end of this object in the yaml file.</param>
        void AfterDeserialized(Mark start, Mark end);
    }
}
