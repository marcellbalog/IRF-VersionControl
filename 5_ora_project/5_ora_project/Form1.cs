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
using _5_ora_project.Entities;

namespace _5_ora_project
{
	public partial class Form1 : Form
	{
		PortfolioEntities context = new PortfolioEntities();
		List<Tick> Ticks;

		List<PortfolioItem> Portfolio = new List<PortfolioItem>();
		List<decimal> Nyereségek = new List<decimal>();

		public Form1()
		{
			InitializeComponent();
			Ticks = context.Ticks.ToList();
			dataGridView1.DataSource = Ticks;

			CreatePortfolio();

			CountVaR();
		}

		private void CreatePortfolio()
		{
			Portfolio.Add(new PortfolioItem() { Index = "OTP", Volume = 10 });
			Portfolio.Add(new PortfolioItem() { Index = "ZWACK", Volume = 10 });
			Portfolio.Add(new PortfolioItem() { Index = "ELMU", Volume = 10 });

			dataGridView2.DataSource = Portfolio;
		}

		private decimal GetPortfolioValue(DateTime date)
		{
			decimal value = 0;
			foreach (var item in Portfolio)
			{
				var last = (from x in Ticks
							where item.Index == x.Index.Trim()
							   && date <= x.TradingDay
							select x)
							.First();
				value += (decimal)last.Price * item.Volume;
			}
			return value;
		}


		void CountVaR()
		{
			int intervalum = 30;
			DateTime kezdőDátum = (from x in Ticks select x.TradingDay).Min();
			DateTime záróDátum = new DateTime(2016, 12, 30);
			TimeSpan z = záróDátum - kezdőDátum;

			for (int i = 0; i < z.Days - intervalum; i++)
			{
				decimal ny = GetPortfolioValue(kezdőDátum.AddDays(i + intervalum))
						   - GetPortfolioValue(kezdőDátum.AddDays(i));
				Nyereségek.Add(ny);
				Console.WriteLine(i + " " + ny);
			}

			var nyereségekRendezve = (from x in Nyereségek
									  orderby x
									  select x)
										.ToList();
			MessageBox.Show(nyereségekRendezve[nyereségekRendezve.Count() / 5].ToString());
		}


		private void button1_Click(object sender, EventArgs e)
		{
			SaveToFile();
		}

		void SaveToFile()
		{
			SaveFileDialog sfd = new SaveFileDialog();

			sfd.DefaultExt = "csv";
			sfd.AddExtension = true;

			if (sfd.ShowDialog() != DialogResult.OK)
				return;


			using (StreamWriter sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
			{
				sw.Write("Időszak; Nyereség");
				for (int i = 0; i < Nyereségek.Count; i++)
				{
					// Egy ciklus iterációban egy sor tartalmát írjuk a fájlba
					// A StreamWriter Write metódusa a WriteLine-al szemben nem nyit új sort
					// Így darabokból építhetjük fel a csv fájl pontosvesszővel elválasztott sorait
					sw.Write(i);
					sw.Write(";");
					sw.Write(Ticks[i].TradingDay);
					sw.Write(";");
					sw.Write(Nyereségek[i]);
					sw.WriteLine(); // Ez a sor az alábbi módon is írható: sr.Write("\n");
				}
			}


		}
	}
}
