using Microsoft.AspNetCore.Mvc;
using RouletteServices.Core;
using System.Collections.Generic;
namespace RouletteServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListRouletteController : Controller
    {
        [HttpGet]
        public List<Roulette> Get()
        {
            Roulette objRoulette = new Roulette();
            return objRoulette.ListRouletteRedis();
        }
    }
}
