﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using HtmlAgilityPack;
using System.Threading;

namespace parser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static string LoadPage(string url, Form form)
        {
            Thread.Sleep(50);
            var result = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.
                GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var receiveStream = response.GetResponseStream();
                if (receiveStream != null)
                {
                    StreamReader readStream;
                    if (response.CharacterSet == null)
                        readStream = new StreamReader(receiveStream);
                    else
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    result = readStream.ReadToEnd();
                    readStream.Close();
                }
                response.Close();
            }
            return result;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            richTextBox1.Clear();
            string[] lines = richTextBox2.Lines.ToArray();
            for (int i = 0; i < lines.Count(); i++)
            {
                if (lines[i].Count() < 2)
                    continue;
                progressBar1.Maximum = lines.Count() - 1;
                Thread.Sleep(100);
                var pageContent = LoadPage(lines[i],this);
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(pageContent);
                HtmlNodeCollection links = document.DocumentNode.SelectNodes("//h1");
                foreach (HtmlNode link in links)
                {
                    richTextBox1.AppendText(link.InnerHtml + "\n");
                }
                links = document.DocumentNode.SelectNodes("//li");
                foreach (HtmlNode link in links)
                {

                    if (link.InnerHtml.IndexOf("itm-opts-label") != -1)
                    {
                        string a = link.InnerHtml;
                        a = a.Replace("<div class=\"itm-opts-label\">", " ");
                        a = a.Replace("</div>", " ");
                       
                        if (a.IndexOf("<a") != -1)
                            continue;
                        richTextBox1.AppendText(a + "\n");

                    }
                }
                string ClassToGet = "amount";
                links = document.DocumentNode.SelectNodes("//span[@class='" + ClassToGet + "']");
              
                foreach (HtmlNode link in links)
                {
                    string a = link.InnerHtml;
                    if (a.IndexOf("<i") != -1)
                    {
                        a = a.Replace("<i>", "");
                        a = a.Replace("</i>", "");
                        a = "Цена: " + a;
                       
                    }
                    else
                    {
                        a = "Количество на складе: " + a;
                    
                        
                    }
                 
                        richTextBox1.AppendText(a + "\n");
                
                   
                }
                richTextBox1.AppendText("\n-------------------------------------------\n");
               
   
                if(progressBar1.Value!=progressBar1.Maximum)
                progressBar1.Value += 1;
            }
            progressBar1.Value = progressBar1.Maximum;



        }

        private void button2_Click(object sender, EventArgs e)
        {
            gifts a = new gifts();
            List<string> q =  a.LoadUrlsInCatalog(textBox1.Text, Convert.ToInt32(numericUpDown1.Value));
            richTextBox2.Clear();
            foreach (string current in q)
            {
                richTextBox2.AppendText(current + "\n");
            }
            MessageBox.Show("Успех");

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            label2.Text = ((RichTextBox)sender).Lines.Count().ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            oasisgifts a = new oasisgifts();
            List<string> q = a.LoadUrlsInCatalog(textBox1.Text, Convert.ToInt32(numericUpDown1.Value));
            richTextBox4.Clear();
            foreach (string current in q)
            {
                richTextBox4.AppendText(current + "\n");
            }
            MessageBox.Show("Успех");

        }
    }
}
