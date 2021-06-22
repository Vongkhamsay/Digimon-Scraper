using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Digimon.Scraper.Models;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Digimon.Scraper
{
  internal class Program
  {
    private const string BaseUrl = "https://world.digimoncard.com";

    private static void Main(string[] args)
    {
      MainAsync().Wait();

      Console.WriteLine("Finished!");
      Console.ReadLine();
    }

    private static async Task MainAsync()
    {
      var urlsToScrape = new List<string>
      {
        //BT1
         "/cardlist/?search=true&category=522001",
        //BT2 && BT3
         "/cardlist/?search=true&category=522002",
        //BT4
         "/cardlist/?search=true&category=522003",
        // ST1
        "/cardlist/?search=true&category=522101",
        // ST2
         "/cardlist/?search=true&category=522102",
        // ST3
         "/cardlist/?search=true&category=522103",
        // ST4
         "/cardlist/?search=true&category=522104",
        // ST5
         "/cardlist/?search=true&category=522105",
        // ST6
        "/cardlist/?search=true&category=522106",
        // ST7
        "/cardlist/?search=true&category=522901"
      };

      await Task.WhenAll(urlsToScrape.Select(async url =>
      {
        var document = await BrowsingContext.New(Configuration.Default.WithDefaultLoader()).OpenAsync($"{BaseUrl}{url}");
        var elements = document.QuerySelectorAll(".popup .card_detail_inner");
        var cards = new List<Card>();

        await Task.WhenAll(elements.Select(async element =>
        {
          var html = await BrowsingContext.New(Configuration.Default).OpenAsync(req => req.Content(element.InnerHtml));
          var card = ScrapeCard(html);

          cards.Add(card);
        }));

        // Console.WriteLine(JsonConvert.SerializeObject(cards, Formatting.Indented));
        var index = cards[0].Number.IndexOf('-');
        WriteToFile(cards, cards[0].Number.Substring(0,index));
      }));
    }

    private static bool WriteToFile(List<Card> cardsList, string fileName)
    {
      Int64 x;
      try
      {
        var path = $"C:/Users/jvong/Documents/DigimonExports/ExportedSets/{fileName}/";
        // var path = $"D:/Digimon/ExportedFiles/Sets/{fileName}/";
        // Determine whether the directory exists.
        if (Directory.Exists(path))
        {
          Console.WriteLine("That path exists already.");
          return true;
        }
        
        //Create directory
        DirectoryInfo di = Directory.CreateDirectory(path);

        var logPath = path;
        var logFile = System.IO.File.Create(logPath + "export.json");
        var logWriter = new System.IO.StreamWriter(logFile);
        logWriter.WriteLine(JsonConvert.SerializeObject(cardsList, Formatting.Indented).ToString());
        logWriter.Dispose();

        Console.WriteLine($"Writing to file - {fileName}");
        Console.WriteLine(JsonConvert.SerializeObject(cardsList, Formatting.Indented));
        
        Console.WriteLine($"Finish writing to file for set {fileName}");

        return true;
      }
      catch(Exception e)
      {
        Console.WriteLine("Exception: " + e.Message);
        return false;
      }
      finally
      {
        Console.WriteLine("Executing finally block.");
      }
    }
    private static Card ScrapeCard(IDocument document)
    {
      var card = new Card();

      ScrapeHeadElement(card, document);
      ScrapeTopElement(card, document);
      ScrapeBottomElement(card, document);

      return card;
    }

    private static void ScrapeHeadElement(Card card, IDocument document)
    {
      card.Name = CheckValue(document.QuerySelector(".card_name").TextContent.Trim());
      card.Number = CheckValue(document.QuerySelector(".cardno").TextContent.Trim());
      card.CardType = CheckValue(document.QuerySelector(".cardtype").TextContent.Trim());
      card.Level = CheckValue(document.QuerySelector(".cardlv")?.TextContent.Trim());
      card.IsAlternate = document.QuerySelector(".cardParallel")?.TextContent.Trim() != null ? true : false;
      card.ImageUrl = CheckValue($"{BaseUrl}/{document.QuerySelector(".card_img img").GetAttribute("src").Substring(3)}".Trim());
    }

    private static void ScrapeTopElement(Card card, IDocument document)
    {
      var element = document.QuerySelector(".cardinfo_top_body");
 
      card.Form = CheckValue(element.Children[0].QuerySelector("dd").TextContent.Trim());
      card.Attribute = CheckValue(element.Children[1].QuerySelector("dd").TextContent.Trim());
      card.Type = CheckValue(element.Children[2].QuerySelector("dd").TextContent.Trim());
      card.Dp = CheckValue(element.Children[3].QuerySelector("dd").TextContent.Trim());
      card.PlayCost = CheckValue(element.Children[4].QuerySelector("dd").TextContent.Trim());
      card.Digivolve1Cost = CheckValue(element.Children[5].QuerySelector("dd").TextContent.Trim());
      card.Digivolve2Cost = CheckValue(element.Children[6].QuerySelector("dd").TextContent.Trim());
    }

    private static void ScrapeBottomElement(Card card, IDocument document)
    {
      var element = document.QuerySelector(".cardinfo_bottom");

      card.Effect = CheckValue(element.Children[0].QuerySelector("dd").TextContent.Trim());
      card.DigivolveEffect = CheckValue(element.Children[1].QuerySelector("dd").TextContent.Trim());
      card.SecurityEffect = CheckValue(element.Children[2].QuerySelector("dd").TextContent.Trim());
    }

    private static string CheckValue(string val)
    {
      if (val == "-")
      {
        return null;
      }

      return val;
    }
  }
}
