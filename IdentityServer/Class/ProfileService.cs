namespace IdentityServer.Class;

/// <summary>
/// 
/// </summary>
public class ProfileService : IProfileService
{
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        try
        {
            //depending on the scope accessing the user data.
            var claims = context.Subject.Claims.ToList();
            //set issued claims to return
            context.IssuedClaims = claims.ToList();
        }
        catch (Exception)
        {
            //log your error
        }
    }
    public async Task IsActiveAsync(IsActiveContext context)
    {
        context.IsActive = true;
    }
}
