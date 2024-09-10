namespace Sofar.Common.Helper
{
    public static class UrlHelper
    {
        public static bool IsValidUrl(string url)
        {
            Uri? uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }
    }
}