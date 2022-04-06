using IdentityServer4.Models;

namespace IdentityServer
{
    /// <summary>
    /// 配置
    /// </summary>
    public class ApiConfig
    {
        /// <summary>
        /// 过期秒数
        /// </summary>
        public const int ExpireIn = 36000;

        /// <summary>
        /// 用户Api相关
        /// </summary>
        public static class UserApi
        {
            public static string[] ApiNames = { "net6microservice" };
            public static string ClientId = "user_clientid";
            public static string Secret = "user_secret";
        }

        /// <summary>
        /// ApiScopes
        /// </summary>
        public static List<ApiScope> ApiScopes => UserApi.ApiNames.Select(a => new ApiScope(a)).ToList();
        /// <summary>
        /// 定义ApiResource，这里的资源（Resources）指的就是我们的API
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
                    AccessTokenLifetime = ExpireIn
                }
            };
        }


    }
}
