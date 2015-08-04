using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NPython
{
    public static class AutoConverter
    {
        private static Dictionary<Type, IConverter> _converters; 


        static AutoConverter()
        {
            _converters = new Dictionary<Type, IConverter>();
            var loadedTypes = Assembly.GetExecutingAssembly().GetTypes();            
            foreach (var converterType in loadedTypes.Where(t => typeof(IConverter).IsAssignableFrom(t)))
            {
                if (converterType == typeof(IConverter) || converterType == typeof(IConverter<>))
                {
                    continue;
                }    

                var convertMethod = converterType.GetMethod("Convert");
                _converters.Add(convertMethod.ReturnType, (IConverter)Activator.CreateInstance(converterType));
            }
        }

        public static TReturnType Convert<TReturnType>(PyObject pyObject)
        {
            var converterKeyValue = _converters.First(c => c.Key.IsAssignableFrom(typeof (TReturnType)));
            //TODO throws exception when no suitable converters
            var converter = (IConverter<TReturnType>) converterKeyValue.Value;
            return converter.Convert(pyObject);
        }
    }
}
