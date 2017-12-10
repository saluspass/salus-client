using System.Web.Http;

namespace Salus.Rest
{
    public class MyWebAPIController : ApiController
    {
        [AcceptVerbs("GET")]
        public string Help()
        {
            return "This is test WEB API. Supported methods are ../api/MyWebAPI/Help, ../api/MyWebAPI/Square/{number}";
        }
        [AcceptVerbs("GET")]
        public int Square(int id)
        {
            return id * id;
        }
    }
}
