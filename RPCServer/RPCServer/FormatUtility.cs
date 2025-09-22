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

        public static DataFormat ResolveInput(HttpContext context)
        {
            var query = context.Request.Query["in"].ToString().ToLower();

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
                if (context.Request.ContentType?.Contains(format.ToString().ToLower()) == true)
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

        public static DataFormat ResolveOutput(HttpContext context)
        {
            var query = context.Request.Query["out"].ToString().ToLower();

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

        public static string GetContentType(DataFormat format)
        {
            switch (format)
            {
                case DataFormat.Json:
                    return "application/json";
                case DataFormat.Yaml:
                    return "application/x-yaml";
                default:
                    return string.Empty;
            }
        }

    }
}
