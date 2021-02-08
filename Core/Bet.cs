using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RouletteServices.Core;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
namespace RouletteServices.Core
{
    public class Bet
    {
        public long idBet { get; set; } //Key Bets
        public int idRoulette { get; set; }//Key Roulette
        public int numberBet { get; set; } //Number for Bet
        public long amountBet { get; set; } //Amount for Bet
        public string descripcionStatus { get; set; } //Description by Bet: Apuesta realizada - Apuesta cerrada - Apuesta fallida
        public string typeBet { get; set; } //Type Game: color o numero
        public string statusBet { get; set; }//Status in Open or Close Roulette: por jugar, gano, perdio
        public double WinAmount { get; set; } //Amount Won, zero if loser
        public List<Bet> CloseRoulette()
        {
            try
            {
                Bet obj = new Bet();
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Bet> objClientBet = client.As<Bet>();
                    List<Bet> listBet = objClientBet.GetAll().ToList();
                    DeleteBetRedis();
                    UpdateBetClose(listBet, GetNumberWin());
                    Roulette objRoulette = new Roulette();
                    objRoulette.idRoulette = idRoulette;
                    objRoulette.UpdateCloseRouletteRedis();
                    return ListBetRedis();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }
        public string SaveBetRedis(Bet objBet)
        {
            try
            {
                if (NumberValidBet(objBet.numberBet))
                {
                    if (TypeBetValid(objBet.typeBet))
                    {
                        Roulette objRoulette = new Roulette();
                        objRoulette.idRoulette = objBet.idRoulette;
                        List<Roulette> listRoulette = objRoulette.ListRouletteRediswithIdRoulette();
                        return CreatedBet(listRoulette,objRoulette,objBet);
                    }
                    else
                    {
                        objBet = loadBet("Fallo el intento de apuesta");
                        return objBet.descripcionStatus;
                    }
                }
                else
                {
                    objBet = loadBet("Fallo el intento de apuesta");
                    return objBet.descripcionStatus;
                }            
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                return "Fallo el intento de apuesta";
            }
        }
        public int GetNumberWin()
        {
            try
            {
                Random win = new Random();
                return win.Next(1, 36);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return 0;
            }
        }
        public bool DeleteBetRedis()
        {
            try
            {
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Bet> objClientBet = client.As<Bet>();
                    objClientBet.DeleteAll();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }
        public Bet SaveBetRediswithoutId(Bet ObjBet)
        {
            try
            {
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Bet> objClientBet = client.As<Bet>();
                    objClientBet.Store(ObjBet);
                    return ObjBet;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:" + ex.Message);
                return null;
            }
        }
        public List<Bet> ListBetRedis()
        {
            try
            {
                Bet objBet = new Bet();
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Bet> objRoulettes = client.As<Bet>();
                    //List<Bet> listRoulette = objRoulettes.GetAll().Where(tb => tb.idRoulette == idRoulette).ToList();
                    List<Bet> listRoulette = objRoulettes.GetAll().ToList();
                    return listRoulette;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }
        public bool NumberValidBet(int number)
        {
            try
            {
                if (number >= 0 && number < 37)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }
        public bool TypeBetValid(string type)
        {
            try
            {
                switch (type)
                {
                    case "color":
                        return true;
                    case "numero":
                        return true;
                    default:
                        return false;
                }
        }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
}
        private void UpdateBetClose(List<Bet> listBet, int numberWin)
        {
            try
            {
                Bet objBet = new Bet();
                foreach (var data in listBet)
                {
                    if (data.idRoulette == idRoulette)
                    {
                        objBet = ValidationWinner(data,numberWin);
                        SaveBetRediswithoutId(objBet);
                    }
                    else {
                        SaveBetRediswithoutId(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
        private Bet ValidationWinner(Bet data,int numberWin) {
            try
            {
                data.descripcionStatus = "Apuesta cerrada";
                if (data.typeBet.Equals("numero"))
                {
                    if (data.numberBet == numberWin)
                    {
                        data.WinAmount = data.amountBet * 5;
                        data.statusBet = "gano";
                    }
                    else
                    {
                        data.WinAmount = 0;
                        data.statusBet = "perdio";
                    }
                }
                else
                {
                    int pair = data.numberBet % 2;
                    int pairwin = numberWin % 2;
                    if (pair == pairwin)
                    {
                        data.WinAmount = data.amountBet * 1.8;
                        data.statusBet = "gano";
                    }
                    else
                    {
                        data.WinAmount = 0;
                        data.statusBet = "perdio";
                    }
                }

                return data;
            }
            catch (Exception ex) {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }

        }
        private Bet loadBet(string descripcionStatus)
        {
            try
            {
                Bet objBet = new Bet();
                objBet.idBet = 0;
                objBet.amountBet = 0;
                objBet.descripcionStatus = descripcionStatus;
                objBet.typeBet = "Sin apuesta";
                objBet.statusBet = "Sin apuesta";
                return objBet;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }

        }
        private String CreatedBet(List<Roulette> listRoulette, Roulette objRoulette, Bet objBet)
        {
            try
            {
                foreach (var data in listRoulette)
                {
                    if (data.status == 2)
                    {
                        if (data.maxAmount > data.availableAmount + objBet.amountBet)
                        {
                            objRoulette.availableAmount = data.availableAmount + objBet.amountBet;
                            if (objRoulette.UpdateBalanceRouletteRedis())
                            {
                                using (RedisClient client = new RedisClient("localhost", 6379))
                                {
                                    IRedisTypedClient<Bet> objClientBet = client.As<Bet>();
                                    objBet.idBet = objClientBet.GetNextSequence();
                                    objBet.descripcionStatus = "Apuesta realizada";
                                    objBet.statusBet = "Por apostar";
                                    objClientBet.Store(objBet);
                                    return objBet.descripcionStatus;
                                }
                            }
                            else
                            {
                                objBet = loadBet("Fallo actualizacion de la ruleta");
                                return objBet.descripcionStatus;
                            }
                        }
                        else
                        {
                            objBet = loadBet("Valor apostado superior al permitido");
                            return objBet.descripcionStatus;
                        }
                    }
                    else if (data.status == 1)
                    {
                        objBet = loadBet("La Ruleta no esta abierta");
                        return objBet.descripcionStatus;
                    }
                    else if (data.status == 3)
                    {
                        objBet = loadBet("La Ruleta ya esta cerrada");
                        return objBet.descripcionStatus;
                    }
                    else
                    {
                        objBet = loadBet("allo el intento de apuesta");
                        return objBet.descripcionStatus;
                    }
                }
                objBet = loadBet("allo el intento de apuesta");
                return objBet.descripcionStatus;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                objBet = loadBet("allo el intento de apuesta");
                return objBet.descripcionStatus;
            }
        }
    }
}
