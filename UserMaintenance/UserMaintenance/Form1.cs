﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace UserMaintenance
{
	public partial class Form1 : Form
	{
		BindingList<User> users = new BindingList<User>();

		public Form1()
		{
			InitializeComponent();
			label1.Text = Properties.Resources.LastName; // label1
			label2.Text = Properties.Resources.FirstName; // label2
			button1.Text = Properties.Resources.Add; // button1

			listBox1.DataSource = users;
			listBox1.ValueMember = "ID";
			listBox1.DisplayMember = "FullName";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var u = new User()
			{
				LastName = textBox1.Text,
				FirstName = textBox2.Text
			};
			users.Add(u);
		}
	}
}
