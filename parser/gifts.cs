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
        public List<string> GetGoodsInfo(string[] UrlsToGet, IProgress<int> progress, int sleep = 100)
        {
            //progressBar1.Value = 0;
            // richTextBox1.Clear();
            //string[] UrlsToGet = richTextBox2.Lines.ToArray();
            List<string> result = new List<string>();
            int currentRow = 0;
            int doneCount = 0;

            for (int i = 0; i < UrlsToGet.Count(); i++)
            {
                if (UrlsToGet[i].Count() < 2)
                    continue;
                
               // progressBar1.Maximum = UrlsToGet.Count() - 1;
                Thread.Sleep(sleep);
                var pageContent = LoadPage(UrlsToGet[i]);
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(pageContent);
                HtmlNodeCollection links = document.DocumentNode.SelectNodes("//h1");
                foreach (HtmlNode link in links)
                {
                    result.Add(link.InnerHtml + "\n");
                   // result[currentRow] += link.InnerHtml + "\n";
                   // richTextBox1.AppendText(link.InnerHtml + "\n");
                }
                links = document.DocumentNode.SelectNodes("//li");
                foreach (HtmlNode link in links)
                {

                    if (link.InnerHtml.IndexOf("itm-opts-label") != -1)
                    {
                        string goodgInfoRow = link.InnerHtml;
                        goodgInfoRow = goodgInfoRow.Replace("<div class=\"itm-opts-label\">", " ");
                        goodgInfoRow = goodgInfoRow.Replace("</div>", " ");

                        if (goodgInfoRow.IndexOf("<a") != -1)
                            continue;
                        result.Add(goodgInfoRow + "\n");
                        //result[currentRow] += goodgInfoRow + "\n";
                        //richTextBox1.AppendText(goodgInfoRow + "\n");

                    }
                }
                string ClassToGet = "amount";
                links = document.DocumentNode.SelectNodes("//span[@class='" + ClassToGet + "']");

                foreach (HtmlNode link in links)
                {
                    string goodsInfoPriceCount = link.InnerHtml;
                    if (goodsInfoPriceCount.IndexOf("<i") != -1)
                    {
                        goodsInfoPriceCount = goodsInfoPriceCount.Replace("<i>", "");
                        goodsInfoPriceCount = goodsInfoPriceCount.Replace("</i>", "");
                        goodsInfoPriceCount = "Цена: " + goodsInfoPriceCount;

                    }
                    else
                    {
                        goodsInfoPriceCount = "Количество на складе: " + goodsInfoPriceCount;


                    }
                    result.Add(goodsInfoPriceCount + "\n");
                    //result[currentRow] += goodsInfoPriceCount + "\n";
                    //richTextBox1.AppendText(goodsInfoPriceCount + "\n");


                }
                currentRow++;
                //richTextBox1.AppendText("\n-------------------------------------------\n");
                result.Add("\n\n\n\n");


                doneCount++;
                progress.Report(doneCount);
               // if (progressBar1.Value != progressBar1.Maximum)
               //     progressBar1.Value += 1;
            }
            //progressBar1.Value = progressBar1.Maximum;
            return result;


        }

        /// <summary>
        /// Рекурсивная загрузка всех ссылок с каталога и подкаталогов
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns></returns>
        public List<string> LoadUrlsInCatalog(string catalog, IProgress<int> progress,int sleep = 100)
        {
            List<string> URLs = new List<string>();

            int selectedPage = 1;
            int rowsCount = 0;
            
            while (true)
            {
                if (selectedPage == 1)
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
                            rowsCount++;
                            if (rowsCount > 80)
                            {
                                rowsCount = 50;
                            }
                            progress.Report(rowsCount);
                        }
                        else if(hrefValue.IndexOf("catalog/") != -1)
                        {
                            URLs.AddRange(LoadUrlsInCatalog("https://gifts.ru" + hrefValue, progress, sleep));
                        }
                    }
                }
                else
                {
                    try
                    {
                        var test = LoadPage(catalog + "/page" + selectedPage,sleep);
                    }
                    catch
                    {
                        break;
                    }
                    var pageContent = LoadPage(catalog + "/page" + selectedPage,sleep);
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
                            rowsCount++;
                            if(rowsCount>80)
                            {
                                rowsCount = 50;
                            }
                            progress.Report(rowsCount);
                        }
                        else if (hrefValue.IndexOf("catalog/") != -1)
                        {
                            URLs.AddRange(LoadUrlsInCatalog("https://gifts.ru" + hrefValue, progress, sleep));
                        }

                    }
                }
                selectedPage++;
            }
            progress.Report(100);
            return URLs;
        }
    }
}
