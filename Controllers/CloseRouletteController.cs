using Microsoft.AspNetCore.Mvc;
using RouletteServices.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace RouletteServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CloseRouletteController : Controller
    {
        [HttpGet("{id}")]
        public List<Bet> Get(int id)
        {
            Bet objbet = new Bet();
            objbet.idRoulette = id;
            List<Bet> listRoulette = objbet.CloseRoulette(); //Return list Bet with status close
            return listRoulette;
        }
    }
}
