using System.Globalization;

namespace Takenet.Textc
{
    /// <summary>
    /// Represents a common context that can be used across multiple input commands processing.
    /// The request context provide values to fill syntaxes tokens.
    /// </summary>
    public interface IRequestContext
    {
        /// <summary>
        /// Gets the context culture.
        /// </summary>
        /// <value>
        /// The culture.
        /// </value>
        CultureInfo Culture { get; }

        /// <summary>
        /// Sets a variable value.
        /// </summary>
        /// <param name="name">The variable name.</param>
        /// <param name="value">The variable value.</param>
        void SetVariable(string name, object value);

        /// <summary>
        /// Gets an existing variable value from the context.
        /// </summary>
        /// <param name="name">The variable name</param>
        /// <returns>If the variable is defined, its value; otherwise null.</returns>
        object GetVariable(string name);

        /// <summary>
        /// Removes an existing variable.
        /// </summary>
        /// <param name="name">The variable name</param>
        void RemoveVariable(string name);

        /// <summary>
        /// Clear all defined variables in the current context.
        /// </summary>
        void Clear();
    }
}