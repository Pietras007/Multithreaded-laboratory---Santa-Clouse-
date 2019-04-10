using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace p3z
{
	class SantaClause
	{
		Random rand = new Random(7);

		public List<(int, string)> decodedLetters = new List<(int, string)>(); //lista par: (identyfikator listu, zdekodowane imię dziecka)
        public List<int> transformingLetters = new List<int>();

        private List<Task> tasks = new List<Task>(); 

		private string DecodeChildName(long number)
		{
			Dictionary<long, string> childrenList = new Dictionary<long, string>() { { 3, "Marek" },
				{ 195421207, "Andrzej" }, { 32416189859, "Agnieszka" }, { 247531, "Kamil" },
				{ 4743953, "Dżesika" }, { 1536839, "Zuzia" }, { 21887587, "Brajanek" },
				{ 1503401, "Ola" }, { 12653723, "Janusz" }, { 522211, "Grażyna" }, {59464991, "Grześ" },
				{ 86281861, "Tomek" } };

			if (childrenList.ContainsKey(number))
				return childrenList[number];
			else
				return "UNKNOWN";
		}

		public void PrepareForChristmas(List<Letter> letters)
		{
			for (int day = 19; day < 24; day++)
			{
                List<Letter> listy = new List<Letter>();
                foreach (var l in letters)
                {
                    if (l.ReceivedDay == day)
                        listy.Add(l);
                }
                tasks.Add(SendLettersToFactory(listy));

				GoSleep();
                if(letters.Count > 0)
                    WhatAbout((letters[rand.Next(letters.Count)]).Id);
			}

			WaitForAllLetters();

			Console.WriteLine("Ho! Ho! Ho! Wesołych świąt!");
		}

        public void WhatAbout(int id)
        {
            lock (decodedLetters)
            {
                foreach (var a in decodedLetters)
                {
                    if (a.Item1 == id)
                    {
                        Console.WriteLine("List " + id + " został przetworzony");
                        return;
                    }
                }
            }
            lock (transformingLetters)
            {
                foreach (var a in transformingLetters)
                {
                    if (a == id)
                    {
                        Console.WriteLine("List " + id + " jest przetwarzany przez fabryke");
                        return;
                    }
                }
            }
            Console.WriteLine("List " + id + " nie dotarł jeszcze do fabryki");
        }

        async public Task SendLettersToFactory(List<Letter> listy)
        {
            if (listy.Count > 0)
            {
                Console.WriteLine("Wysłanie listów do fabryki z " + listy[0].ReceivedDay + " grudnia");
                ChristmasFactory k = new ChristmasFactory();
                foreach(var a in listy)
                {
                    transformingLetters.Add((a.Id));
                }
                var temp = k.ManageLetters(listy);
                await (temp);
                Console.WriteLine("Przetworzono wszystkie listy z " + listy[0].ReceivedDay + " grudnia");
                foreach (var a in temp.Result)
                {
                    lock (decodedLetters)
                    {
                        decodedLetters.Add((a.Item1, DecodeChildName(a.Item2)));
                    }
                }
            }
        }

        public void WaitForAllLetters()
        {
            Task.WaitAll(tasks.ToArray());
        }

        public void GoSleep()
		{
			Console.WriteLine("Mikołaj śpi");
			System.Threading.Thread.Sleep(1000);
		}
	}


    public enum errorCode { IsPrime, MoreThan2Factors };
    class IncorrectFactorizationException : ApplicationException
    {
        public errorCode error { get; }
        public IncorrectFactorizationException(errorCode ec) { error = ec; }
    }

    class ChristmasFactory
    {
        public long NumberFactorization(long liczba)
        {
            try
            {
                if (IsPrimeNumber(liczba) == true)
                    throw new IncorrectFactorizationException(errorCode.IsPrime);
                if (liczba % 2 == 0)
                    return 2;
                for (long i = 3; i <= liczba; i++)
                {
                    if (liczba % i == 0)
                    {
                        if(IsPrimeNumber(liczba/i) == true)
                        {
                            return i;
                        }
                        else
                        {
                            throw new IncorrectFactorizationException(errorCode.MoreThan2Factors);
                        }
                    }
                }
                return -1;
            }
            catch(IncorrectFactorizationException e) when (e.error == errorCode.IsPrime)
            {
                return liczba;
            }
            catch(IncorrectFactorizationException e) when (e.error == errorCode.MoreThan2Factors)
            {
                return 3;
            }
        }

        public long NumberFactorizationParallel(long liczba)
        {
            long wynik = 0;
            try
            {
                if (IsPrimeNumber(liczba) == true)
                    throw new IncorrectFactorizationException(errorCode.IsPrime);
                if (liczba % 2 == 0)
                    return 2;
                Parallel.For(3, (long)Math.Sqrt(liczba) + 1, (i,ls) =>
                 {
                     if (liczba % i == 0)
                     {
                         try
                         {
                             if (IsPrimeNumber(liczba / i) == true)
                             {
                                 wynik = i;
                                 ls.Stop();
                             }
                             else
                             {
                                 throw new IncorrectFactorizationException(errorCode.MoreThan2Factors);
                             }
                         }
                         catch(IncorrectFactorizationException e) when (e.error == errorCode.MoreThan2Factors)
                         {
                             wynik = 3;
                             ls.Stop();
                         }
                     }
                     i++;
                 });
                return wynik;
            }
            catch (IncorrectFactorizationException e) when (e.error == errorCode.IsPrime)
            {
                return liczba;
            }
        }

        public bool IsPrimeNumber(long liczba)
        {
            bool wynik = true;
            long aaa = (long)Math.Sqrt(liczba) + 1;
            if (liczba == 2 || liczba == 3 || liczba == 5 || liczba == 7)
                return true;
            if (liczba % 2 == 0)
                return false;
            for(long i=3;i<=aaa;i = i+2)
            {
                if (liczba % i == 0)
                    return false;
            }
            return wynik;
        }
        
         public async Task<List<(int, long)>> ManageLetters(List<Letter> L)
         {
             List<(int, long)> result = new List<(int, long)>();
             Task<long>[] tasks = new Task<long>[L.Count];
             int i = 0;
             foreach(var l in L)
             {
                //Console.WriteLine("Czy synchroniczne????");
                //tasks[i] = Task.Run(() => NumberFactorization(l.Number));
                tasks[i] = Task.Run(() => NumberFactorizationParallel(l.Number));
                i++;
             }
            i = 0;
             foreach(var t in tasks)
             {
                 var temp = await tasks[i];
                 result.Add((L[i].Id,temp));
                 i++;
             }
             return result;
         }

    }


}