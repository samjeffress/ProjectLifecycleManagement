using System;

namespace ProjectManagement.Web.Helpers
{
    // From http://blogs.msdn.com/b/rickandy/archive/2011/05/02/securing-your-asp-net-mvc-3-application.aspx
    // Allows us to tag a method or class as anonymous, and if class level will make all methods the same
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}