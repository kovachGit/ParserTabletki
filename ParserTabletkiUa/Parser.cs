using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using AngleSharp.Css;
using System.Collections.Generic;
using System;
using System.Threading;

namespace ParserTabletkiUa
{
    public class Parser
    {
        public static String Host = @"https://tabletki.ua/uk/";
        public static bool IsFound = false;

        public static List<DrugBlock> ListOfDrugsBlocks;
        public static async void SearchByName(string drugName)
        {
            ListOfDrugsBlocks = new List<DrugBlock>();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var url = Host + drugName + "/";
        

            var doc = await context.OpenAsync(url);
            var title = doc.Title;

            Thread.Sleep(1000);
            var div = doc.QuerySelectorAll("div.card__category");


            //card__category

            IsFound = false;
            foreach (var elem in div)
            {

                DrugBlock drugBlock = new DrugBlock();

                var row = elem.QuerySelector("div.card__category--info");
                if (row != null && row.TextContent != null)
                    drugBlock.Name = row.Text().Trim();

                row = elem.QuerySelector("div.card__category--info-additional");
                if (row != null && row.TextContent != null)
                    drugBlock.Manufacturer = row.Text().Trim();

                row = elem.QuerySelector("div.card__category--price");
                if (row != null && row.TextContent != null)
                    drugBlock.PriceString = row.TextContent;

                row = elem.QuerySelector("div.card__category--info-where-found");
                if (row != null && row.TextContent != null)
                    drugBlock.Stores = row.TextContent;


                var img = elem.QuerySelector("img");
                if (img != null)
                    drugBlock.ImageSource = img.GetAttribute("src");

                 
                var link = elem.QuerySelector("a.mt-2");
                if (link != null)
                    drugBlock.LinkStores = ((IHtmlAnchorElement)link).Href;

                if (!String.IsNullOrEmpty(drugBlock.Name))
                    ListOfDrugsBlocks.Add(drugBlock);

            }
            IsFound = true;

        }

        
    }
}
