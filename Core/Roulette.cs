using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
namespace RouletteServices.Core
{
    public class Roulette
    {
        public long idRoulette { get; set; }
        public int status { get; set; }
        public string statusDescription { get; set; }
        public long maxAmount { get; set; }
        public double availableAmount { get; set; }
        public List<Roulette> ListRouletteRedis() {
            try
            {
                Roulette objRoulette = new Roulette();
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Roulette> objRoulettes = client.As<Roulette>();
                    List<Roulette> listRoulette =  objRoulettes.GetAll().ToList();
                    return listRoulette;
                }             
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }
        public List<Roulette> ListRouletteRediswithIdRoulette()
        {
            try
            {
                Roulette obj = new Roulette();
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Roulette> objRoulettes = client.As<Roulette>();
                    List<Roulette> listRoulette = objRoulettes.GetAll().ToList();
                    return listRoulette.Where(tb => tb.idRoulette == idRoulette).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }
        public Roulette SaveRouletteRedis(Roulette objRoulette) {
            try
            {
                using (RedisClient client = new RedisClient("localhost", 6379)) {
                    IRedisTypedClient<Roulette> objRoulettes = client.As<Roulette>();
                    objRoulette.idRoulette = objRoulettes.GetNextSequence();
                    objRoulette.statusDescription = objRoulette.statusDescription + objRoulette.idRoulette;
                    objRoulettes.Store(objRoulette);
                    return objRoulette;
                }
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR:" + ex.Message);
                return null;
            }
        }
        public Roulette SaveRouletteRediswithoutId(Roulette objRoulette)
        {
            try
            {
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Roulette> objRoulettes = client.As<Roulette>();                  
                    objRoulettes.Store(objRoulette);
                    return objRoulette;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                return null;
            }
        }
        public bool DeleteRouletteRedis()
        {
            try
            {
                 
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Roulette> objRoulettes = client.As<Roulette>();
                    objRoulettes.DeleteAll();                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }
        public int OpenRouletteRedis() {
            try
            {
                int response = 0;
                List<Roulette> listRoulette = ListRouletteRedis().ToList();
                DeleteRouletteRedis();

                foreach (var data in listRoulette)
                {
                    if (data.idRoulette == idRoulette) {
                        if (data.status == 1)
                        {
                            data.status = 2;
                            data.statusDescription = "Ruleta abierta";
                            response = 1;
                        }
                        else if (data.status == 2)
                        {
                            response = 2;
                        }
                        else if (data.status == 3) {
                            response = 3;
                        }
                    }                    
                    SaveRouletteRediswithoutId(data);
                }
                    return response;
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
                return 0;
            }
        }
        public bool UpdateBalanceRouletteRedis()
        {
            try
            {
                List<Roulette> listRoulette = ListRouletteRedis().ToList();
                DeleteRouletteRedis();

                foreach (var data in listRoulette)
                {
                    if (data.idRoulette == idRoulette)
                    {
                        data.availableAmount = availableAmount;
                    }
                    SaveRouletteRediswithoutId(data);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }
        public bool UpdateCloseRouletteRedis()
        {
            try
            {
                List<Roulette> listRoulette = ListRouletteRedis().ToList();
                DeleteRouletteRedis();

                foreach (var data in listRoulette)
                {
                    if (data.idRoulette == idRoulette)
                    {
                        data.availableAmount = 0;
                        data.statusDescription = "Ruleta cerrada";
                        data.status = 3;
                        data.maxAmount = 0;
                        data.availableAmount = 0;
                    }
                    SaveRouletteRediswithoutId(data);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }
    }
}
