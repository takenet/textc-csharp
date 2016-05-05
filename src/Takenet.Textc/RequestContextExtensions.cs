using Newtonsoft.Json;

namespace Takenet.Textc
{
    public static class RequestContextExtensions
    {
        /// <summary>
        /// Gets an existing variable value from the context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetVariable<T>(this IRequestContext context, string name)
        {
            try
            {
                var stringContent = JsonConvert.SerializeObject(context.GetVariable(name));
                return JsonConvert.DeserializeObject<T>(stringContent);
            }
            catch
            {
                return default(T);
            }
        }
    }
}