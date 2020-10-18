using _6_ora.Enities;
using _6_ora.MnbServiceReference;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace _6_ora
{
	public partial class Form1 : Form
	{
		BindingList<RateData> Rates = new BindingList<RateData>();

		public Form1()
		{
			InitializeComponent();
			//CallService();
			dataGridView1.DataSource = Rates;
			ProcessXML(CallService());
		}

		string CallService()
		{
			var mnbService = new MNBArfolyamServiceSoapClient();

			var request = new GetExchangeRatesRequestBody()
			{
				currencyNames = "EUR",
				startDate = "2020-01-01",
				endDate = "2020-06-30"
			};

			var response = mnbService.GetExchangeRates(request);

			var result = response.GetExchangeRatesResult;
			return result;
		}

		void ProcessXML(string _xml)
		{
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(_xml);

			foreach (XmlElement e in xml)
			{
				RateData rd = new RateData();
				Rates.Add(rd);

				rd.Date = DateTime.Parse(e.GetAttribute("data"));

				var childElement = (XmlElement)e.ChildNodes[0];
				rd.Currency = childElement.GetAttribute("curr");

				var unit = decimal.Parse(childElement.GetAttribute("unit"));
				var value = decimal.Parse(childElement.InnerText);
				if (unit != 0)
					rd.Value = value / unit;

			}
		}
	}
}
