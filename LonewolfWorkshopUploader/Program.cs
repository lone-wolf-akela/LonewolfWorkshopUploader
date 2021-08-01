using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Ugc;
using Newtonsoft;
using Newtonsoft.Json;

namespace LonewolfWorkshopUploader
{
    class Program
    {
        const uint WORKSHOP_TOOL_ID = 347380;
        static readonly AppId HWRM_APPID = 244160;
        enum Visibility
        {
            Public = 0,
            FriendsOnly = 1,
            Private = 2
        }
        class UploadInfo
        {
            public string Title { get; set; }
            public List<string> Tags { get; set; }
            public ulong WorkshopID { get; set; }
            public string DescriptionPath { get; set; }
            public string ContentFolder { get; set; }
            public string PreviewFile { get; set; }
            public Visibility Visibility { get; set; }
            public string ChangelogPath { get; set; }

        }
        static void PrintItemInfo(Item item)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine($"Title: {item.Title}");
            Console.WriteLine($"IsInstalled: {item.IsInstalled}");
            Console.WriteLine($"IsDownloading: {item.IsDownloading}");
            Console.WriteLine($"IsDownloadPending: {item.IsDownloadPending}");
            Console.WriteLine($"IsSubscribed: {item.IsSubscribed}");
            Console.WriteLine($"NeedsUpdate: {item.NeedsUpdate}");
            Console.WriteLine($"Description: {item.Description}");
            Console.WriteLine("===========================================");
        }
        static async Task<Item> GetItem(UploadInfo info)
        {
            var item = await Item.GetAsync(info.WorkshopID);
            return item.Value;
        }
        static async Task<Result> UploadItem(Editor editor, UploadInfo info)
        {
            var desc = info.DescriptionPath.Length == 0 ? "" :
                File.ReadAllText(info.DescriptionPath, Encoding.UTF8);
            var changelog = info.ChangelogPath.Length == 0 ? "":
                File.ReadAllText(info.ChangelogPath, Encoding.UTF8);
            editor = editor
                .WithTitle(info.Title)
                .WithContent(info.ContentFolder)
                .WithPreviewFile(info.PreviewFile)
                .WithDescription(desc)
                .WithChangeLog(changelog)
                .ForAppId(HWRM_APPID);
            switch (info.Visibility)
            {
                case Visibility.Public:
                    editor = editor.WithPublicVisibility();
                    break;
                case Visibility.FriendsOnly:
                    editor = editor.WithFriendsOnlyVisibility();
                    break;
                case Visibility.Private:
                    editor = editor.WithPrivateVisibility();
                    break;
            }
            foreach(var tag in info.Tags)
            {
                editor = editor.WithTag(tag);
            }
            Console.WriteLine("Begin Uploading...");
            string[] progress_mark = { "-", "\\", "|", "/" };
            int progress_idx = progress_mark.Length - 1;
            var result = await editor.SubmitAsync(new Progress<float>(
                p =>
                {
                    progress_idx = (progress_idx + 1) % progress_mark.Length;
                    Console.Write($"\rUploaded: {p * 100}% [{progress_mark[progress_idx]}]         ");
                }));
            Console.WriteLine("");
            return result.Result;
        }
        static async Task<Editor> getEditor(UploadInfo info)
        {
            if(info.WorkshopID != 0)
            {
                var item = await GetItem(info);
                PrintItemInfo(item);
                return item.Edit();
            }
            else
            {
                return Editor.NewCommunityFile;
            }
        }
        static async Task Main(string[] args)
        {
            if(args.Length != 1 || args[0] == "-h" || args[0] == "--help")
            {
                Console.WriteLine("Usage: LonewolfWorkshopUploader.exe <build config file>");
                Environment.Exit(1);
            }

            try
            {
                SteamClient.Init(WORKSHOP_TOOL_ID);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            Console.WriteLine($"Welcome {SteamClient.Name} (id = {SteamClient.SteamId})");

            var infojson = File.ReadAllText(args[0], Encoding.UTF8);
            var info = JsonConvert.DeserializeObject<UploadInfo>(infojson);
            var editor = await getEditor(info);
            var result = await UploadItem(editor, info);
            Console.WriteLine($"Result: {result}");
            
            SteamClient.Shutdown();
            return;
        }
    }
}
