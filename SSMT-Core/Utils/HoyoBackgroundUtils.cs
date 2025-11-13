using Microsoft.UI.Xaml.Media.Imaging;
using SSMT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SSMT_Core.Utils
{
    public class HoyoBackgroundUtils
    {
        public static string ZZZ_CN = "x6znKlJ0xK";
        public static string HSR_CN = "64kMb5iAWu";
        public static string GI_CN = "1Z8W5NHUQb";
        public static string HI3_CN = "osvnlOc0S8";

        public static string ZZZ_GLOBAL = "U5hbdsT9W7";
        public static string HSR_GLOBAL = "4ziysqXOQ8";
        public static string GI_GLOBAL = "gopR6Cufr3";
        public static string HI3_GLOBAL = "5TIVvvcwtM";

        public static string URL_PREFIX_CN = "https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getAllGameBasicInfo?launcher_id=jGHBHlcOq1&language=zh-cn&game_id=";
        public static string URL_PREFIX_GLOBAL = "https://sg-hyp-api.hoyoverse.com/hyp/hyp-connect/api/getAllGameBasicInfo?launcher_id=VYTpXlbWo8&language=zh-cn7D&game_id=";
        


        public static string GetGameId(string LogicNameStr, bool Chinese = true)
        {
            string GameId = "";

            if (LogicNameStr == LogicName.GIMI)
            {
                if (Chinese)
                {
                    GameId = GI_CN;
                }
                else
                {
                    GameId = GI_GLOBAL;
                }
            }
            else if (LogicNameStr == LogicName.SRMI)
            {
                if (Chinese)
                {
                    GameId = HSR_CN;
                }
                else
                {
                    GameId = HSR_GLOBAL;
                }
            }
            else if (LogicNameStr == LogicName.HIMI)
            {
                if (Chinese)
                {
                    GameId = HI3_CN;
                }
                else
                {
                    GameId = HI3_GLOBAL;
                }
            }
            else if (LogicNameStr == LogicName.ZZMI)
            {
                if (Chinese)
                {
                    GameId = ZZZ_CN;
                }
                else
                {
                    GameId = ZZZ_GLOBAL;
                }
            }

            return GameId;
        }



        /// <summary>
        /// 信息来源：
        /// https://github.com/Scighost/Starward/issues/833
        /// 
        /// 新版启动器背景图API
        /// https://hyp-api.mihoyo.com/hyp/hyp-connect/api/getAllGameBasicInfo?launcher_id=jGHBHlcOq1&language=zh-cn&game_id=64kMb5iAWu
        /// 
        /// 国际服
        /// https://sg-hyp-api.hoyoverse.com/hyp/hyp-connect/api/getAllGameBasicInfo?launcher_id=VYTpXlbWo8&language=zh-cn7D&game_id=4ziysqXOQ8
        /// 
        /// 通过替换game_id参数来实现访问不同游戏的背景图数据
        /// 
        /// </summary>
        /// <param name="LogicName"></param>
        /// <param name="Chinese"></param>
        public static string GetBackgroundUrl(string GameId, bool Chinese = true)
        {
            string UrlPrefix = "";
            if (Chinese)
            {
                UrlPrefix = URL_PREFIX_CN;
            }
            else
            {
                UrlPrefix = URL_PREFIX_GLOBAL;
            }

            string TotalUrl = UrlPrefix + GameId;
            return TotalUrl;
        }


        public static async Task<string> DownloadLatestWebpBackground(string AppUrl)
        {
            // 创建 HttpClient
            using HttpClient client = new HttpClient();

            // 添加 User-Agent 头，模拟浏览器请求
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            // 发送 GET 请求
            Console.WriteLine("正在请求API...");
            HttpResponseMessage response = await client.GetAsync(AppUrl);
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API请求成功，开始解析JSON...");

            // 解析 JSON
            using JsonDocument document = JsonDocument.Parse(responseBody);
            JsonElement root = document.RootElement;

            // 获取第一个 background 的 URL
            JsonElement firstBackground = root
                .GetProperty("data")
                .GetProperty("game_info_list")[0]
                .GetProperty("backgrounds")[0]
                .GetProperty("background")
                .GetProperty("url");

            string imageUrl = firstBackground.GetString();
            Console.WriteLine($"获取到图片URL: {imageUrl}");

            // 下载图片
            Console.WriteLine("开始下载图片...");
            HttpResponseMessage imageResponse = await client.GetAsync(imageUrl);
            imageResponse.EnsureSuccessStatusCode();

            // 获取图片数据
            byte[] imageData = await imageResponse.Content.ReadAsByteArrayAsync();

            // 创建保存路径
            string fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);

            string ext = Path.GetExtension(fileName);

            string savePath = Path.Combine(PathManager.Path_GamesFolder, GlobalConfig.CurrentGameName + "\\Background" + ext);

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            // 保存图片
            await File.WriteAllBytesAsync(savePath, imageData);
            Console.WriteLine($"图片已保存到: {savePath}");

            return savePath;
        }


        public static async Task<string> DownloadLatestWebmBackground(string AppUrl)
        {
            // 创建 HttpClient
            using HttpClient client = new HttpClient();

            // 添加 User-Agent 头，模拟浏览器请求
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            // 发送 GET 请求
            LOG.Info("正在请求API...");
            HttpResponseMessage response = await client.GetAsync(AppUrl);
            response.EnsureSuccessStatusCode();

            // 读取响应内容
            string responseBody = await response.Content.ReadAsStringAsync();
            LOG.Info("API请求成功，开始解析JSON...");

            // 解析 JSON
            using JsonDocument document = JsonDocument.Parse(responseBody);
            JsonElement root = document.RootElement;

            // 获取第一个 background 的 URL
            JsonElement firstBackground = root
                .GetProperty("data")
                .GetProperty("game_info_list")[0]
                .GetProperty("backgrounds")[0]
                .GetProperty("video")
                .GetProperty("url");

            string imageUrl = firstBackground.GetString();
            LOG.Info($"获取到图片URL: {imageUrl}");

            // 下载图片
            LOG.Info("开始下载图片...");
            HttpResponseMessage imageResponse = await client.GetAsync(imageUrl);
            imageResponse.EnsureSuccessStatusCode();

            // 获取图片数据
            byte[] imageData = await imageResponse.Content.ReadAsByteArrayAsync();

            // 创建保存路径
            string fileName = Path.GetFileName(new Uri(imageUrl).LocalPath);

            string ext = Path.GetExtension(fileName);

            string savePath = Path.Combine(PathManager.Path_GamesFolder, GlobalConfig.CurrentGameName + "\\Background" + ext);

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            // 保存图片
            await File.WriteAllBytesAsync(savePath, imageData);
            LOG.Info($"图片已保存到: {savePath}");

            return savePath;
        }

    }
}
