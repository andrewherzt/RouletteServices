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
    public class BetRouletteController : Controller
    {
        [HttpPost]
        public string SetBet([FromBody] Bet form)
        {
            try
            {
                Bet objBet = new Bet();
                return objBet.SaveBetRedis(form); //return Status Bet 
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
                return "Fallo en la apuesta";
            }            
        }                
    }
}
