namespace CommonLibrary;

/// <summary>
/// 工具类
/// </summary>
public static class CommonFun
{
    public static string GUID => Guid.NewGuid().ToString("N");
    public static bool IsNull(this string s)
    {
        return string.IsNullOrWhiteSpace(s);
    }
    public static bool NotNull(this string s)
    {
        return !string.IsNullOrWhiteSpace(s);
    }
}
