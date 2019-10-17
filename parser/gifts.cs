using System;
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
    class gifts
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


        public gifts()
        {

        }

        /// <summary>
        /// Загрузка страницы по URL
        /// </summary>
        /// <param name="url">URL страницы для загрузки</param>
        /// <returns></returns>
        static string LoadPage(string url,int sleep = 100)
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

        /// <summary>
        /// Рекурсивная загрузка всех ссылок с каталога и подкаталогов
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public List<string> LoadUrlsInCatalog(string catalog,int sleep = 100)
        {
            List<string> URLs = new List<string>();

            int i = 1;
            while (true)
            {
                if (i == 1)
                {
                    var pageContent = LoadPage(catalog,sleep);
                    var document = new HtmlAgilityPack.HtmlDocument();
                    document.LoadHtml(pageContent);
                    string ClassToGet = "itm-list-link";
                    HtmlNodeCollection links = document.DocumentNode.SelectNodes("//a[@class='" + ClassToGet + "']");
                    foreach (HtmlNode link in links)
                    {
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        if (hrefValue.IndexOf("id/") != -1)
                        {
                            URLs.Add("https://gifts.ru" + hrefValue);
                        }else if(hrefValue.IndexOf("catalog/") != -1)
                        {
                            URLs.AddRange(LoadUrlsInCatalog("https://gifts.ru" + hrefValue,sleep));
                        }
                    }
                }
                else
                {
                    try
                    {
                        var test = LoadPage(catalog + "/page" + i,sleep);
                    }
                    catch
                    {
                        break;
                    }
                    var pageContent = LoadPage(catalog + "/page" + i,sleep);
                    var document = new HtmlAgilityPack.HtmlDocument();
                    document.LoadHtml(pageContent);
                    string ClassToGet = "itm-list-link";
                    HtmlNodeCollection links = document.DocumentNode.SelectNodes("//a[@class='" + ClassToGet + "']");
                    foreach (HtmlNode link in links)
                    {
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        if (hrefValue.IndexOf("id/") != -1)
                        {
                            URLs.Add("https://gifts.ru" + hrefValue);
                        }
                        else if (hrefValue.IndexOf("catalog/") != -1)
                        {
                            URLs.AddRange(LoadUrlsInCatalog("https://gifts.ru" + hrefValue,sleep));
                        }

                    }
                }
                i++;
            }
            return URLs;
        }
    }
}
