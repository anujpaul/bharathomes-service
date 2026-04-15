using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("")]
public class HomeController
{
    [HttpGet]
    public string Test()
    {
        return "Bharat Homes";
    }
}