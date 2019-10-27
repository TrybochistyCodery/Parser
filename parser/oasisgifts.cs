﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace parser
{
    class oasisgifts
    {
        public string URL;

        //URL картинки
        public string picture_URL;

        //Наименование товара
        public string name;

        //Размеры товара
        public string size;

        //Материал
        public string material;

        //Размеры упаковки
        public string box;

        //Вес
        public string weight;

        //Объем упаковки
        public string packing_volume;

        //Количество в упаковке
        public string amount_in_a_package;

        //Цена
        public string price;

        //Количество на складе
        public string quantity_in_stock;

        static string LoadPage(string url, int sleep = 100)
        {
            //MessageBox.Show(sleep.ToString());
            Thread.Sleep(sleep);
            var result = "";
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();

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


        public List<string> LoadUrlsInCatalog(string catalog, int sleep = 100)
        {
            List<string> URLs = new List<string>();

            int i = 1;
            while (true)
            {
                if (i == 1)
                {
                    var pageContent = LoadPage(catalog, sleep);
                    var document = new HtmlAgilityPack.HtmlDocument();
                    document.LoadHtml(pageContent);
                    string ClassToGet = "catalog-product mass-item";
                    HtmlNodeCollection links = document.DocumentNode.SelectNodes("//a[@class='" + ClassToGet + "']");
                    foreach (HtmlNode link in links)
                    {
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        if (hrefValue.IndexOf("item/") != -1)
                        {
                            URLs.Add("https://www.oasiscatalog.com" + hrefValue);
                        }
                        else if (hrefValue.IndexOf("catalog/") != -1)
                        {
                            URLs.AddRange(LoadUrlsInCatalog("https://www.oasiscatalog.com" + hrefValue, sleep));
                        }
                    }
                }
                else
                {
                    try
                    {
                        var test = LoadPage(catalog + "?page=" + i, sleep);
                    }
                    catch
                    {
                        break;
                    }
                    var pageContent = LoadPage(catalog + "?page=" + i, sleep);
                    var document = new HtmlAgilityPack.HtmlDocument();
                    document.LoadHtml(pageContent);
                    string ClassToGet = "catalog-product mass-item";
                    HtmlNodeCollection links = document.DocumentNode.SelectNodes("//a[@class='" + ClassToGet + "']");
                    foreach (HtmlNode link in links)
                    {
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        if (hrefValue.IndexOf("item/") != -1)
                        {
                            URLs.Add("https://www.oasiscatalog.com" + hrefValue);
                        }
                        else if (hrefValue.IndexOf("catalog/") != -1)
                        {
                            URLs.AddRange(LoadUrlsInCatalog("https://www.oasiscatalog.com" + hrefValue, sleep));
                        }
                    }
                }
                i++;
            }
            return URLs;
        }
    }
}