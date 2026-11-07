using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TM.Common.Utils
{
    public sealed class Convert
    {
        public static TResult? To<TData, TResult>(TData data)
        {
            var result = default(TResult);
            try
            {
                Guard.AgainstNull(data, "Data is null or has invalid content(s).");
                result = JsonConvert.DeserializeObject<TResult>(JsonConvert.SerializeObject(data));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Conversion failed: " + ex.Message, ex);
            }

            return result;
        }
    }
}
