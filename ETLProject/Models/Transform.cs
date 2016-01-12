using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    class Transform
    {
        public List<Opinia> listaOpinii = new List<Opinia>();
        public string[] daneUrzadzenia = new string[3];

        public async Task transformation(string htmlCeneo, string htmlSkapiec, string urlCeneo, string urlSkapiec)
        {
            await getDeviceData(htmlCeneo);
            await getAllCommentsCeneo(htmlCeneo, urlCeneo);
            await getAllCommentsSkapiec(htmlSkapiec, urlSkapiec);
            //listaOpinii.AddRange(await transformCeneo(htmlCeneo));
            //listaOpinii.AddRange(await transformSkapiec(htmlSkapiec));
            
        }

        private string SkapiecDateSerialize(string data)
        {
            string dzien = data.Substring(0, 2);
            string rok = data.Substring(data.Length - 4, 4);
            string miesiacrok = data.Substring(dzien.Length + 1);
            string miesiac = miesiacrok.Substring(0, miesiacrok.Length - rok.Length - 1);

            string miesiacliczba = "";

            if (miesiac == "stycznia") miesiacliczba = "01";
            if (miesiac == "lutego") miesiacliczba = "02";
            if (miesiac == "marca") miesiacliczba = "03";
            if (miesiac == "kwietnia") miesiacliczba = "04";
            if (miesiac == "maja") miesiacliczba = "05";
            if (miesiac == "czerwca") miesiacliczba = "06";
            if (miesiac == "lipca") miesiacliczba = "07";
            if (miesiac == "sierpnia") miesiacliczba = "08";
            if (miesiac == "września") miesiacliczba = "09";
            if (miesiac == "października") miesiacliczba = "10";
            if (miesiac == "listopada") miesiacliczba = "11";
            if (miesiac == "grudnia") miesiacliczba = "12";


            return rok + "-" + miesiacliczba + "-" +dzien;
        }

        private string SkapiecRatingSerialize(string rate)
        {
            //width: 80%;
            string liczba = Regex.Match(rate, @"\d+").Value;
            int liczbaint = Int32.Parse(liczba);

            if (liczbaint >= 20) liczba = "1";
            if (liczbaint >= 40) liczba = "2";
            if (liczbaint >= 60) liczba = "3";
            if (liczbaint >= 80) liczba = "4";
            if (liczbaint >= 99) liczba = "5";
            return liczba;
        }


        private async Task<List<Opinia>> transformCeneo(string html)
        {
            List<Opinia> opinieCeneo = new List<Opinia>();
            //listaOpinii.Add(new Opinia();
                        HtmlDocument htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(html);
            

            var comments = htmlDocument.DocumentNode.DescendantsAndSelf("li").Where(o => o.GetAttributeValue("class", null) == "product-review js_product-review");


            foreach (HtmlNode comment in comments)
            {
                var commentheader = comment.Elements("header").FirstOrDefault();
                var commentauthor = commentheader.Elements("div").Where(n => n.GetAttributeValue("class", null) == "reviewer-cell").FirstOrDefault();
                var isRecomended = commentheader.Elements("div").Where(n => n.GetAttributeValue("class", null) == "reviewer-recommendation").FirstOrDefault();

                var commentdivs = comment.Elements("div").FirstOrDefault();
                var score = commentdivs.DescendantsAndSelf("span").Where(n => n.GetAttributeValue("class", null) == "review-score-count").FirstOrDefault();
                var date = commentdivs.Descendants("time").Where(n => n.GetAttributeValue("datetime", null) != null).FirstOrDefault();
                var opinion = commentdivs.DescendantsAndSelf("p").Where(n => n.GetAttributeValue("class", null) == "product-review-body").FirstOrDefault();
                var pros = commentdivs.DescendantsAndSelf("span").Where(n => n.GetAttributeValue("class", null) == "pros-cell").FirstOrDefault();
                var cons = commentdivs.DescendantsAndSelf("span").Where(n => n.GetAttributeValue("class", null) == "cons-cell").FirstOrDefault();
                var useful = commentdivs.DescendantsAndSelf("span").Where(n => n.GetAttributeValue("class", null) == "product-review-usefulness-stats").FirstOrDefault();

                string autor = commentauthor.InnerText.Trim();
                string polecam = isRecomended.InnerText.Trim();
                string ocena = score.InnerText.Trim().Substring(0, 1);
                string data = date.Attributes["datetime"].Value.Substring(0, 10);
                string opinia = opinion.InnerText.Trim();
                string zalety = Regex.Replace(pros.InnerText.Replace("Zalety", "").Trim(), "\\s\\s+", ", ");
                string wady = Regex.Replace(cons.InnerText.Replace("Wady", "").Trim(), "\\s\\s+", ", ");
                string uzytecznosc = Regex.Replace(useful.InnerText, "\\s+", " ");

                 opinieCeneo.Add(new Opinia(zalety, wady, opinia, ocena, autor, data, polecam, uzytecznosc, "Ceneo"));
            }

            //commentsCount = Int32.Parse(htmlDocument.DocumentNode.Descendants("span").Where(o => o.GetAttributeValue("itemprop", null) == "reviewCount").First().InnerText);
            


            return opinieCeneo;         

                
        }

        private async Task<List<Opinia>> transformSkapiec(string html)
        {
            List<Opinia> opinieSkapiec = new List<Opinia>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var commentsall = htmlDocument.DocumentNode.DescendantsAndSelf("ul").Where(n => n.GetAttributeValue("class", null) == "opinion-list");


            foreach (HtmlNode comments in commentsall)
            {
                var comment = comments.Descendants("li").Where(n => n.ParentNode.Attributes[0].Value == "opinion-list");
                foreach (HtmlNode commen in comment)
                {
                    string zalety = "";
                    string wady = "";

                    var commentheader = commen.Elements("div").Where(n => n.GetAttributeValue("class", null) == "opinion-wrapper").FirstOrDefault();
                    var rateszone = commen.Elements("div").Where(n => n.GetAttributeValue("class", null) == "comments-zone").FirstOrDefault();

                    var opinion = commentheader.Elements("div").Where(n => n.GetAttributeValue("class", null) == "opinion-container").FirstOrDefault();

                    var rating = commentheader.Element("span");
                    var author = opinion.Elements("span").Where(n => n.GetAttributeValue("class", null) == "author").FirstOrDefault();
                    var date = opinion.Elements("span").Where(n => n.GetAttributeValue("class", null) == "date").FirstOrDefault();
                    var opiniontext = opinion.Elements("p").First();

                    var opinionratingsall = rateszone.Elements("div").Where(n => n.GetAttributeValue("class", null) == "score tooltip-container").FirstOrDefault();
                    var opinionratingselements = opinionratingsall.Elements("span");


                    try
                    {
                        var prosnconsul = opinion.Elements("ul").Where(n => n.GetAttributeValue("class", null) == "pros-n-cons").FirstOrDefault();
                        var prosncons = prosnconsul.Elements("li");
                        try
                        {
                            zalety = prosncons.ElementAt(0).InnerText.Replace("Zalety ", "");
                        }
                        catch { }
                        try
                        {
                            wady = prosncons.ElementAt(1).InnerText.Replace("Wady ", "");
                        }
                        catch { }

                    }
                    catch { }

                    string ocena = SkapiecRatingSerialize(rating.Attributes["style"].Value);
                    string autor = author.InnerText;
                    string data = SkapiecDateSerialize(date.InnerText);
                    string opinia = opiniontext.InnerText.Trim();
                    //string opinionplus = opinionratingselements.ElementAt(0).InnerText.Replace("&nbsp;Pomocna(","").Trim();
                    //string opinionminus = opinionratingselements.ElementAt(1).InnerText.Trim();

                    string opinionplus = Regex.Match(opinionratingselements.ElementAt(0).InnerText, @"\d+").Value;
                    string opinionminus = Regex.Match(opinionratingselements.ElementAt(1).InnerText, @"\d+").Value;


                    int opinionplusint = Int32.Parse(opinionplus);
                    int opinionminusint = Int32.Parse(opinionminus);

                    int opinioncount = opinionminusint + opinionplusint;
                    double opinionpercent;

                    if (opinioncount > 0)
                    {
                        opinionpercent = (opinionplusint / opinioncount) * 100;
                    }
                    else
                    {
                        opinionpercent = 0;
                    }

                    string uzytecznosc = opinionpercent + "% ( " + opinionplusint + " z " + opinioncount+ " )";

                    opinieSkapiec.Add(new Opinia(zalety, wady, opinia, ocena, autor, data, "", uzytecznosc, "Skąpiec"));
                    }
                
            }

            return opinieSkapiec;
        }

        private async Task getDeviceData(string html)
        {
            string others = string.Empty;
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var producttitle = htmlDocument.DocumentNode.Descendants("h1").Where(n => n.GetAttributeValue("itemprop", null) == "name").First();
            var tytul = producttitle.InnerText;

            var specs = htmlDocument.DocumentNode.Descendants("div").Where(n => n.GetAttributeValue("class", null) == "specs-group").First();
            string producent = specs.Descendants("li").Where(n => n.GetAttributeValue("class", null) == "attr-value").First().InnerText.Trim();

            var otherSpec = specs.Descendants("tr").Skip(1);
            //string inne = otherSpec.InnerHtml;
            foreach (HtmlNode other in otherSpec)
            {
                string otherHeader = other.Element("th").InnerText.Trim();
                string otherAttrib = other.Descendants("li").Where(n => n.GetAttributeValue("class", null) == "attr-value").First().InnerText.Trim();

                others = others + otherHeader + ": " + otherAttrib + "\n";
            }
            //Wynik.Text = "Device: " + tytul + "\n Producent: " + producent + "\nInne:\n" + others;
            daneUrzadzenia[0] = tytul;
            daneUrzadzenia[1] = producent;
            daneUrzadzenia[2] = others;

        }

        private async Task getAllCommentsCeneo(string html, string urlCeneo)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            int pagesCount = 1; 
            try
            {
                int commentsCounter = Convert.ToInt32(htmlDocument.DocumentNode.Descendants("span").Where(o => o.GetAttributeValue("itemprop", null) == "reviewCount").First().InnerText);
                
                if (commentsCounter >= 10)
                {
                    pagesCount = (commentsCounter % 10 == 0) ? commentsCounter / 10 : commentsCounter / 10 + 1;
                }
            }
            catch
            {
                pagesCount = 1;
            }


            for (int i = 1; i <= pagesCount; i++)
            {
                if (i == 1)
                {
                    listaOpinii.AddRange(await transformCeneo(html));
                }
                else
                {
                    Extract t = new Extract();
                    listaOpinii.AddRange(await transformCeneo(await t.ExtractHTML(urlCeneo + "/opinie-" + i)));
                }
            }
        }

        private async Task getAllCommentsSkapiec(string html, string urlSkapiec)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            int pagesCount = 1;

            try
            {
                var commentsDiv = htmlDocument.DocumentNode.Descendants("div").Where(o => o.GetAttributeValue("class", null) == "opinion").FirstOrDefault();
                var comments = commentsDiv.Descendants("a").Where(o => o.GetAttributeValue("href", null) == "#opinie").First().InnerText.Trim();
                int commentsCounter = Convert.ToInt32(Regex.Match(comments, @"\d+").Value);

                if (commentsCounter >= 30)
                {
                    pagesCount = (commentsCounter % 30 == 0) ? commentsCounter / 30 : commentsCounter / 30 + 1;
                }
            }
            catch
            {
                pagesCount = 1;
            }

            for (int i = 1; i <= pagesCount; i++)
            {
                if (i == 1)
                {
                    listaOpinii.AddRange(await transformSkapiec(html));
                }
                else
                {
                    Extract t = new Extract();
                    listaOpinii.AddRange(await transformSkapiec(await t.ExtractHTML(urlSkapiec + "_komentarze/" + i)));
                }
            }
        }
    }
}
