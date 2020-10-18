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
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;

namespace _6_ora
{
	public partial class Form1 : Form
	{
		BindingList<RateData> Rates = new BindingList<RateData>();

		BindingList<string> Currencies = new BindingList<string>();

		public Form1()
		{
			InitializeComponent();
			//CallService();
			dataGridView1.DataSource = Rates;
			ProcessXML(CallService());
			LoadChart();
		}

		string CallService()
		{
			var mnbService = new MNBArfolyamServiceSoapClient();

			var request = new GetExchangeRatesRequestBody()
			{
				currencyNames = comboBox1.SelectedItem.ToString(),
				startDate = dateTimePicker1.Value.ToString(),
				endDate = dateTimePicker2.Value.ToString()
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

		void LoadChart()
		{
			chartRateData.DataSource = Rates;

			var series = chartRateData.Series[0];
			series.ChartType = SeriesChartType.Line;
			series.XValueMember = "Date";
			series.YValueMembers = "Value";
			series.BorderWidth = 2;

			var legend = chartRateData.Legends[0];
			legend.Enabled = false;

			var chartArea = chartRateData.ChartAreas[0];
			chartArea.AxisX.MajorGrid.Enabled = false;
			chartArea.AxisY.MajorGrid.Enabled = false;
			chartArea.AxisY.IsStartedFromZero = false;

		}

		void RefreshData()
		{
			Rates.Clear();
			LoadChart();
		}

		private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
		{
			RefreshData();
		}

		private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
		{
			RefreshData();
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			RefreshData();
		}
	}
}
