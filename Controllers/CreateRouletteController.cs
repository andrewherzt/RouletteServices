using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouletteServices.Core;
namespace RouletteServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateRouletteController : Controller
    {
        [HttpGet]
        public Roulette Get()
        {
            Roulette objRoulette = new Roulette();
            try
            {
              objRoulette = ResponseRoulette(1, "Ruleta Creada # ", 10000);
                objRoulette.SaveRouletteRedis(objRoulette); //Save in cache
                if (objRoulette.idRoulette <= 0)
                {
                    objRoulette = ResponseRoulette(0, "Fallo creación", 0);
                }

                return objRoulette;
            }
            catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                objRoulette = ResponseRoulette(0, "Fallo creación", 0);

                return objRoulette;
            }         
        }
        private Roulette ResponseRoulette(int status, string statusDescription, long maxAmount) {
            try
            {
                Roulette objRoulette = new Roulette();
                objRoulette.status = status; //1 is created
                objRoulette.statusDescription = statusDescription;//Descripcion status 1
                objRoulette.maxAmount = maxAmount; //for default 
                objRoulette.availableAmount = 0; //creation without betting
                return objRoulette;
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
                return null; 
            }
        }
    }
}
