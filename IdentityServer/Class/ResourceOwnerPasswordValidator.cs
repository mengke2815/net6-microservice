namespace IdentityServer.Class;

/// <summary>
/// 验证用户名密码
/// </summary>
public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    public SqlSugarScope _sqlSugar;
    public ResourceOwnerPasswordValidator(SqlSugarScope sqlSugar)
    {
        _sqlSugar = sqlSugar;
    }
    /// <summary>
    /// 数据库验证
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        if (context.UserName == "admin" && context.Password == "123")
        {
            //验证成功
            //自定义用户信息
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123456789"),
                new Claim(ClaimTypes.Name,"张三")
            };
            context.Result = new GrantValidationResult(subject: "admin", authenticationMethod: "custom", claims: claims);
        }
        else
        {
            //验证失败
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
        }
        return Task.CompletedTask;
    }
}
