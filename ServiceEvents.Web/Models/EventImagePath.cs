namespace ServiceEvents.Web.Models;

public static class EventImagePath
{
    public static string? GetSource(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var path = value.Trim();
        var localPath = path.Split('?', '#')[0];
        var decodedLocalPath = Uri.UnescapeDataString(localPath);
        if (path.Length > 500
            || path.Contains('\\')
            || path.Any(char.IsControl)
            || decodedLocalPath.Contains('\\')
            || decodedLocalPath.Split('/').Any(segment => segment == ".."))
        {
            return null;
        }

        if (Uri.TryCreate(path, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.Scheme == Uri.UriSchemeHttps
                && !string.IsNullOrEmpty(absoluteUri.Host)
                && string.IsNullOrEmpty(absoluteUri.UserInfo)
                    ? path
                    : null;
        }

        if (path.StartsWith("//", StringComparison.Ordinal))
        {
            return null;
        }

        if (path.StartsWith("~/", StringComparison.Ordinal)
            || path.StartsWith("/", StringComparison.Ordinal))
        {
            return path;
        }

        return path.StartsWith("~", StringComparison.Ordinal)
            ? null
            : $"~/{path}";
    }

    public static bool IsValid(string? value)
    {
        return string.IsNullOrWhiteSpace(value) || GetSource(value) is not null;
    }
}
