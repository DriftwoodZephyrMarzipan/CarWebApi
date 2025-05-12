using Microsoft.AspNetCore.Mvc;

namespace CarWebApi.Controllers;

[ApiController]
public class BaseController : Controller
{
    protected string GetUri(string controllerRoute, int id)
    {
        return $"{this.Request.Scheme}://{this.Request.Host}/{controllerRoute}/{id}";
    }
}
