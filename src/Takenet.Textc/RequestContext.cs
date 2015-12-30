using System;
using System.Collections.Generic;
using System.Globalization;

namespace Takenet.Textc
{
    /// <summary>
    /// Defines a context that uses an internal dictionary to store the variables.
    /// </summary>
    public class RequestContext : IRequestContext
    {
        public RequestContext()
            : this(CultureInfo.InvariantCulture)
        {
            
        }

        public RequestContext(CultureInfo culture)
        {
            if (culture == null) throw new ArgumentNullException(nameof(culture));
            Culture = culture;
            ContextVariableDictionary = new Dictionary<string, object>();
        }

        protected IDictionary<string, object> ContextVariableDictionary { get; }

        public CultureInfo Culture { get; }

        public virtual void SetVariable(string name, object value)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            ContextVariableDictionary.Remove(name);
            ContextVariableDictionary.Add(name, value);
        }

        public virtual object GetVariable(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            object value;
            ContextVariableDictionary.TryGetValue(name, out value);
            return value;
        }

        public virtual void RemoveVariable(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            ContextVariableDictionary.Remove(name);
        }

        public void Clear()
        {
            ContextVariableDictionary.Clear();
        }
    }
}