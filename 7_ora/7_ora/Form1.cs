using _7_ora.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _7_ora
{
	public partial class Form1 : Form
	{
		List<Person> Population = new List<Person>();
		List<BirthProbability> BirthProbabilities = new List<BirthProbability>();
		List<DeathProbability> DeathProbabilities = new List<DeathProbability>();

		Random rng = new Random(1234);

		List<int> menList = new List<int>();
		List<int> womenList = new List<int>();

		public Form1()
		{
			InitializeComponent();

			Population = GetPopulation(textBox1.Text);
			BirthProbabilities = GetBirthProbabilities(@"C:\Temp\születés.csv");
			DeathProbabilities = GetDeathProbabilities(@"C:\Temp\halál.csv");

		}

		public List<Person> GetPopulation(string csvpath)
		{
			List<Person> population = new List<Person>();

			using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine().Split(';');
					population.Add(new Person()
					{
						BirthYear = int.Parse(line[0]),
						Gender = (Gender)Enum.Parse(typeof(Gender), line[1]),
						NbrOfChildren = int.Parse(line[2])
					});
				}
			}

			return population;
		}

		public List<BirthProbability> GetBirthProbabilities(string csvpath)
		{
			List<BirthProbability> BirthProbabilities = new List<BirthProbability>();

			using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine().Split(';');
					BirthProbabilities.Add(new BirthProbability()
					{
						Age = int.Parse(line[0]),
						ChildCount = int.Parse(line[1]),
						P = double.Parse(line[2])
					});
				}
			}

			return BirthProbabilities;
		}

		public List<DeathProbability> GetDeathProbabilities(string csvpath)
		{
			List<DeathProbability> DeathProbabilities = new List<DeathProbability>();

			using (StreamReader sr = new StreamReader(csvpath, Encoding.Default))
			{
				while (!sr.EndOfStream)
				{
					var line = sr.ReadLine().Split(';');
					DeathProbabilities.Add(new DeathProbability()
					{
						Gender = (Gender)Enum.Parse(typeof(Gender), line[0]),
						Age = int.Parse(line[1]),
						P = double.Parse(line[2])
					});
				}
			}

			return DeathProbabilities;
		}

		/// <summary>
		/// Returns 0 - not alive, 1 - man, 2 - woman.
		/// </summary>
		/// <param name="year"></param>
		/// <param name="person"></param>
		/// <returns></returns>
		private int SimStep(int year, Person person)
		{
			if (!person.IsAlive) return 0;

			byte age = (byte)(year - person.BirthYear);

			double pDeath = (from x in DeathProbabilities
							 where x.Gender == person.Gender && x.Age == age
							 select x.P).FirstOrDefault();
			if (rng.NextDouble() <= pDeath)
				person.IsAlive = false;

			if (person.IsAlive && person.Gender == Gender.Female)
			{
				double pBirth = (from x in BirthProbabilities
								 where x.Age == age
								 select x.P).FirstOrDefault();
				if (rng.NextDouble() <= pBirth)
				{
					Person újszülött = new Person();
					újszülött.BirthYear = year;
					újszülött.NbrOfChildren = 0;
					újszülött.Gender = (Gender)(rng.Next(1, 3));
					Population.Add(újszülött);
				}

				return 2;
			}
			else if (person.Gender == Gender.Male)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Simulation();
			DisplayResults();
		}

		void Simulation()
		{
			for (int year = 2005; year <= 2024; year++)
			{
				int men = 0;
				int women = 0;
				for (int i = 0; i < Population.Count; i++)
				{
					int gender = SimStep(year, Population[i]);
					if (gender == 1)
						men++;
					else if (gender == 2)
						women++;					
				}
				menList.Add(men);
				womenList.Add(women);
				

				int nbrOfMales = (from x in Population
								  where x.Gender == Gender.Male && x.IsAlive
								  select x).Count();
				int nbrOfFemales = (from x in Population
									where x.Gender == Gender.Female && x.IsAlive
									select x).Count();
				Console.WriteLine(
					string.Format("Év:{0} Fiúk:{1} Lányok:{2}", year, nbrOfMales, nbrOfFemales));
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.ShowDialog();

		}

		void DisplayResults()
		{
			for (int year = 2005; year <= 2024; year++)
			{
				richTextBox1.Text = "Szimulációs év: " + year + "\n Fiúk: " + menList[year-2005] + "\n Lányok:" + womenList[year-2005];

			}
		}
	}


}
