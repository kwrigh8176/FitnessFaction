using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;


namespace FitnessFaction.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuth : AuthorizeAttribute
    {

       //custom authorization method for every page after login/sign-up
       protected bool AuthorizeCore(HttpContext httpContext)
        {
            
            //if the context hasnt been initialized at all throw exception
            if (httpContext.Session.GetString("username") == null) throw new ArgumentNullException("httpContext");
           
            return true;
        }

       
    }


}