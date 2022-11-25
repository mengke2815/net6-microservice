namespace IdentityServer.Class;

/// <summary>
/// 配置
/// </summary>
public class ApiConfig
{
    /// <summary>
    /// 过期秒数
    /// </summary>
    public const int ExpireIn = 3600 * 24;

    /// <summary>
    /// 用户Api相关
    /// </summary>
    public static class UserApi
    {
        public static string[] ApiNames => new string[] { "net7_microservice" };
        public static string ClientId => "net7_clientid";
        public static string Secret => "net7_secret";
    }

    /// <summary>
    /// ApiScopes
    /// </summary>
    public static List<ApiScope> ApiScopes => UserApi.ApiNames.Select(a => new ApiScope(a)).ToList();
    /// <summary>
    /// ApiResource
    /// </summary>
    /// <returns></returns>
    public static List<ApiResource> GetApiResources => UserApi.ApiNames.Select(a => new ApiResource(a, a) { Scopes = { a } }).ToList();

    /// <summary>
    /// 定义受信任的客户端 Client
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Client> GetClients()
    {
        return new[]
        {
            new Client
            {
                ClientId = UserApi.ClientId,//客户端的标识，要是惟一的
                ClientSecrets = new [] { new Secret(UserApi.Secret.Sha256()) },//客户端密码，进行了加密
                AllowedGrantTypes =  GrantTypes.ResourceOwnerPassword,//授权方式
                AllowedScopes= UserApi.ApiNames,//定义这个客户端可以访问的APi资源数组
                AccessTokenLifetime = ExpireIn,
                AllowOfflineAccess = true//如果要获取refresh_tokens，必须把AllowOfflineAccess设置为true
            }
        };
    }
}
