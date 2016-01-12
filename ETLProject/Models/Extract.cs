using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ETLProject.Models
{
    class Extract
    {
        public string htmlCeneo { get; private set; }
        public string htmlSkapiec { get; private set; }


        public Extract()
        {
            htmlCeneo = "pusto";
            htmlSkapiec = "pusto";
        }


        public async Task Extraction(string ceneoUrl, string skapiecUrl)
        {
            htmlCeneo = await ExtractHTML(ceneoUrl);

            htmlSkapiec = await ExtractHTML(skapiecUrl);
            
        }

        public async Task<string> ExtractHTML(string url)
        {
            await Task.Delay(10000);
            string html = "CLEAR!";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse x = await req.GetResponseAsync();
                HttpWebResponse res =  (HttpWebResponse)x;

                if (res != null)
                {
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        Stream stream = res.GetResponseStream();
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            html = reader.ReadToEnd();
                            
                        }

                        HtmlDocument htmlDocument = new HtmlDocument();
                        htmlDocument.LoadHtml(html);


                    }
                     res.Dispose();
                }

            }

            catch
            {
               return html = "ERROR!";
            }
            return html;
        }
        
        
        
    }
}
