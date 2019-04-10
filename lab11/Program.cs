using System;
using System.Collections.Generic;

namespace p3z
{
	class Letter
	{
		public int Id { get; set; }

		public int ReceivedDay { get; set; }

		public long Number { get; set; }
	}


	class Program
	{
		static void Main(string[] args)
		{
			List<long> numbers = new List<long>() { 43644897932034923, 1139975111599073, 19365384954451,
				305483512991, 5582601, 10658679040868657, 150562231216857049, 10964859087037, 32416189859, 185395908650993};

			List<Letter> letters = new List<Letter>();
			letters.Add(new Letter() { Id = 1, Number = numbers[0], ReceivedDay = 19 });
            letters.Add(new Letter() { Id = 2, Number = numbers[1], ReceivedDay = 19 });
            letters.Add(new Letter() { Id = 3, Number = numbers[2], ReceivedDay = 19 });

            letters.Add(new Letter() { Id = 4, Number = numbers[3], ReceivedDay = 21 });
            letters.Add(new Letter() { Id = 5, Number = numbers[4], ReceivedDay = 21 });

            letters.Add(new Letter() { Id = 6, Number = numbers[5], ReceivedDay = 22 });
            letters.Add(new Letter() { Id = 7, Number = numbers[6], ReceivedDay = 22 });
            letters.Add(new Letter() { Id = 8, Number = numbers[7], ReceivedDay = 22 });

            letters.Add(new Letter() { Id = 9, Number = numbers[8], ReceivedDay = 23 });
            letters.Add(new Letter() { Id = 10, Number = numbers[9], ReceivedDay = 23 });

            SantaClause santa = new SantaClause();

			santa.PrepareForChristmas(letters);

			Console.WriteLine();
			Console.WriteLine("Autorzy listów:");
			for (int i = 0; i < santa.decodedLetters.Count; i++)
				Console.WriteLine("Autor listu " + santa.decodedLetters[i].Item1 + " to " + santa.decodedLetters[i].Item2);
		}
	}
}
