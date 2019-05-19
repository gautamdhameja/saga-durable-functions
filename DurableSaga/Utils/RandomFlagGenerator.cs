using DurableSaga.Models;
using System;

namespace DurableSaga.Utils
{
    public class RandomFlagGenerator
    {
        public static RandomFlag Generate()
        {
            var random = new Random();
            var num = random.Next(1, 10);
            var flag = num % 2 == 0 ? true : false;
            var msg = flag ? "booked" : "not booked";

            return new RandomFlag { Flag = flag, Message = msg };
        }
    }
}
