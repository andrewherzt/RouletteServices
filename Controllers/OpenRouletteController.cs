using Microsoft.AspNetCore.Mvc;
using System;
using RouletteServices.Core;
namespace RouletteServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenRouletteController : Controller
    {
        [HttpGet("{id}")]
        public string Get(int id)
        {
            try
            {
                if (id > 0)
                {
                    Roulette objRoulette = new Roulette();
                    objRoulette.idRoulette = id;
                    return ResponseMessageOpenRoulette(objRoulette.OpenRouletteRedis());                              
                }
                else {
                    return "Numero de ruleta no valido";
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                return null;
            }
        }
        private string ResponseMessageOpenRoulette(int idResp) {
            try
            {
                switch (idResp)
                {
                    case 0:
                        return "Ruleta no encontrada";
                    case 1:
                        return "Ruleta abierta";
                    case 2:
                        return "La ruleta ya estaba abierta";
                    default:
                        return "Ruleta no encontrada";
                }
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
                return "Ruleta no encontrada";
            }
        }
    }
}
