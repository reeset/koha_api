using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace koha_reference_app
{
    public partial class Form1 : Form
    {
        private const string username = "[your_username]";
        private const string password = "[your_password]";
        private const string host = "[your_host]"; //example "http://marcedit.kohacatalog.com"
        private CookieContainer cookieJar = null;
        koha_api.Bib_Actions objb = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            objb = new koha_api.Bib_Actions();
            objb.Host = host;
            

            if (objb.Authorize(username, password) == true)
            {
                System.Windows.Forms.MessageBox.Show(objb.Debug_Info + "\n" +
                                                     "Authorized");
            }
            else
            {
                System.Windows.Forms.MessageBox.Show(objb.Error_Message);
            }
        }

        
        private string GetRecord(string id)
        {
            if (objb == null)
            {
                System.Windows.Forms.MessageBox.Show("Not Authorized");
                return "";
            }
            else
            {
                return objb.GetRecord(id);
            }

        }


        private bool UpdateRecord(string rec, string id)
        {
            if (objb == null)
            {
                System.Windows.Forms.MessageBox.Show("Not Authorized");
                return false;
            }
            else
            {
                return objb.UpdateRecord(rec, id);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string rec = GetRecord("1");
            textBox1.Text = rec;
            System.Windows.Forms.MessageBox.Show(objb.GetRecordId(rec, "999", "d"));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            UpdateRecord(textBox1.Text, "1");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (objb == null)
            {
                System.Windows.Forms.MessageBox.Show("Not Authorized");
            }
            else
            {
                objb.CreateRecord(textBox1.Text);
            }
        }
    }
}
