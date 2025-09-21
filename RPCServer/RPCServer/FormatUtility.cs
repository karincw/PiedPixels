using System.Collections;

namespace RPCServer
{
    public enum DataFormat
    {
        Json,
        Yaml,
    }

    public static class FormatUtility
    {
        public static DataFormat ResolveInput(HttpRequest request)
        {
            var query = request.Query["in"].ToString().ToLower();

            IEnumerable dataformats = Enum.GetValues(typeof(DataFormat));
            foreach (DataFormat format in dataformats)
            {
                if (query.Equals(format.ToString().ToLower()))
                {
                    return format;
                }
            }
            foreach (DataFormat format in dataformats)
            {
                if (request.ContentType?.Contains(format.ToString().ToLower()) == true)
                {
                    return format;
                }
            }

            return DataFormat.Json;
        }

        public static DataFormat ResolveOutput(HttpRequest request)
        {
            var query = request.Query["out"].ToString().ToLower();

            IEnumerable dataformats = Enum.GetValues(typeof(DataFormat));
            foreach (DataFormat format in dataformats)
            {
                if (query.Equals(format.ToString().ToLower()))
                {
                    return format;
                }
            }

            return DataFormat.Json;
        }
    }
}
