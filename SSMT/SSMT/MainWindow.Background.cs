using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using SSMT_Core;
using SSMT_Core.InfoItemClass;
using SSMT_Core.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using WinUI3Helper;

namespace SSMT
{
    public partial class MainWindow
    {

        private void ResetBackground()
        {
            try
            {
                // 停止播放并清空媒体
                BackgroundMediaPlayer.Pause();
                BackgroundMediaPlayer.Source = null;
            }
            catch { }

            // 隐藏视频
            BackgroundVideo.Visibility = Visibility.Collapsed;

            // 清空静态图
            MainWindowImageBrush.Visibility = Visibility.Collapsed;
            MainWindowImageBrush.Source = null;
        }

        public void ShowBackgroundVideo(string path,string TargetGameName)
        {
            if (GlobalConfig.CurrentGameName != TargetGameName)
            {
                return;
            }

            ResetBackground();

            BackgroundVideo.Visibility = Visibility.Visible;

            // 刷新 Uri，避免 WinUI3 缓存旧版本
            var uri = new Uri(path + "?t=" + DateTime.Now.Ticks);

            BackgroundMediaPlayer.Source = MediaSource.CreateFromUri(uri);
            BackgroundVideo.SetMediaPlayer(BackgroundMediaPlayer);

            BackgroundMediaPlayer.Play();

            VisualHelper.CreateFadeAnimation(BackgroundVideo);
        }


        public void ShowBackgroundPicture(string path, string TargetGameName)
        {
            if (GlobalConfig.CurrentGameName != TargetGameName)
            {
                return;
            }

            ResetBackground();

            MainWindowImageBrush.Visibility = Visibility.Visible;

            VisualHelper.CreateScaleAnimation(imageVisual);
            VisualHelper.CreateFadeAnimation(imageVisual);

            // 强制刷新图片链接（你之前已有相同逻辑）
            MainWindowImageBrush.Source =
                new BitmapImage(new Uri(path + "?t=" + DateTime.Now.Ticks));
        }


        public async Task InitializeBackground(string TargetGame)
        {

            ResetBackground();

            //来一个支持的后缀名列表，然后依次判断
            List<BackgroundSuffixItem> SuffixList = new List<BackgroundSuffixItem>();
            //这里顺序可有讲究了，在此特别说明
            //首先就是有MP4的情况下优先加载MP4，因为.webm会转换为.mp4格式来作为背景图
            SuffixList.Add(new BackgroundSuffixItem { Suffix = ".mp4", IsVideo = true });
            SuffixList.Add(new BackgroundSuffixItem { Suffix = ".webm", IsVideo = true });
            SuffixList.Add(new BackgroundSuffixItem { Suffix = ".webp", IsPicture = true });
            SuffixList.Add(new BackgroundSuffixItem { Suffix = ".png", IsPicture = true });

            //这里轮着试一遍所有的背景图类型，如果有的话就设置上了
            //如果没有的话就保持刚开始初始化完那种没有的状态了
            string TargetGameFolderPath = Path.Combine(PathManager.Path_GamesFolder, TargetGame + "\\");

            bool BackgroundExists = false;
            foreach (BackgroundSuffixItem SuffixItem in SuffixList)
            {
                string BackgroundFilePath = Path.Combine(TargetGameFolderPath, "Background" + SuffixItem.Suffix);

                if (!File.Exists(BackgroundFilePath))
                {
                    continue;
                }

                if (SuffixItem.IsVideo)
                {
                    ShowBackgroundVideo(BackgroundFilePath,TargetGame);
                    BackgroundExists = true;
                    break;
                }
                else if (SuffixItem.IsPicture)
                {
                    ShowBackgroundPicture(BackgroundFilePath,TargetGame);
                    BackgroundExists = true;
                    break;
                }

            }


            //米的四个游戏保底更新背景图，主要是为了用户第一次拿到手SSMT的时候就能有背景图
            if (!BackgroundExists)
            {
                //只有米的四个游戏会根据游戏名称默认触发保底背景图更新
                try
                {
               


                    if (TargetGame == LogicName.GIMI ||
                        TargetGame == LogicName.SRMI ||
                        TargetGame == LogicName.HIMI ||
                        TargetGame == LogicName.ZZMI
                        )
                    {
                        //_ = SSMTMessageHelper.Show(CurrentLogicName);
                        string PossibleWebpPicture = Path.Combine(TargetGameFolderPath, "Background.webp");
                        string PossiblePngBackgroundPath = Path.Combine(TargetGameFolderPath, "Background.png");


                        if (!File.Exists(PossibleWebpPicture))
                        {
                            if (!File.Exists(PossiblePngBackgroundPath))
                            {
                                //自动加载当前背景图，因为满足LogicName且并未设置背景图
                                await AutoUpdateBackgroundPicture(TargetGame,TargetGame);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }

        }

        public async Task AutoUpdateBackgroundPicture(string TargetGame,string SpecificLogicName = "" )
        {
            ResetBackground();
            string GameId = HoyoBackgroundUtils.GetGameId(SpecificLogicName, GlobalConfig.Chinese);

            if (GameId == "")
            {
                _ = SSMTMessageHelper.Show("当前选择的执行逻辑: " + SpecificLogicName + " 暂不支持自动更新背景图，请手动设置。");
                return;
            }

            string BaseUrl = HoyoBackgroundUtils.GetBackgroundUrl(GameId, GlobalConfig.Chinese);
            //webm不一定存在，所以直接try catch，出错就懒得管了

            bool UseWebmBackground = false;

            //Nico: 注意，绝区零的背景图视频有毛病，虽然都是.webm格式，但是只能在浏览器中播放，无法使用本地的媒体播放器播放
            //这意味着我们必须添加ffmpeg转码，下载下来之后执行转换，变为mp4视频，然后再应用为背景图，因为我实际测试发现WinUI3也是无法播放的
            //因为WinUI3用的就是系统的解码，底层都是一个东西导致的。
            //有点麻烦了，而且动态背景图本身就存在循环播放一瞬间卡顿的问题，而且暂时没法解决
            //综合来说，解决这个问题收益不大，暂时不实现了，随缘等一个热爱ZZZ的开发者提PR实现一下

            // PxncAcd: 您猜怎么着, 解决了


            try
            {
                string NewWebmBackgroundPath = await HoyoBackgroundUtils.DownloadLatestWebmBackground(BaseUrl,TargetGame);

                if (File.Exists(NewWebmBackgroundPath))
                {
                    UseWebmBackground = true;
                }

                string finalVideoPath = NewWebmBackgroundPath;

                if (SpecificLogicName == LogicName.ZZMI)
                {
                    // For ZZZ: cannot decode .webm → must transcode to mp4 via ffmpeg.

                    try
                    {
                        
                        if (!File.Exists(PathManager.Path_Plugin_FFMPEG))
                        {
                            LOG.Info("ffmpeg.exe 不存在，无法进行 ZZZ 背景视频转码");
                            throw new FileNotFoundException("ffmpeg.exe missing");
                        }

                        string mp4Output = Path.ChangeExtension(NewWebmBackgroundPath, ".mp4");

                        //Nico: 这里有个测试得出的BUG，记录在此
                        //必须先删除旧的背景图，然后再去转码。
                        //否则直接调用转码如果目标mp4存在的情况下会转码失败。
                        if (File.Exists(mp4Output))
                        {
                            File.Delete(mp4Output);
                        }


                        var psi = new ProcessStartInfo
                        {
                            FileName = PathManager.Path_Plugin_FFMPEG,
                            Arguments = $"-y -i \"{NewWebmBackgroundPath}\" -c:v libx264 -preset veryfast -pix_fmt yuv420p \"{mp4Output}\"",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true
                        };

                        using (var p = new Process())
                        {
                            p.StartInfo = psi;

                            p.OutputDataReceived += (s, e) =>
                            {
                                if (!string.IsNullOrEmpty(e.Data))
                                    LOG.Info("[ffmpeg stdout] " + e.Data);
                            };

                            p.ErrorDataReceived += (s, e) =>
                            {
                                if (!string.IsNullOrEmpty(e.Data))
                                    LOG.Info("[ffmpeg stderr] " + e.Data);
                            };

                            p.Start();

                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();

                            await p.WaitForExitAsync();

                            if (p.ExitCode != 0 || !File.Exists(mp4Output))
                                throw new Exception($"ffmpeg failed, exit code {p.ExitCode}");

                            LOG.Info("ZZZ 背景视频已成功由 webm 转为 mp4");
                            finalVideoPath = mp4Output;
                            // delete webm
                            try
                            {
                                File.Delete(NewWebmBackgroundPath);
                            }
                            catch (Exception delEx)
                            {
                                LOG.Info("删除中间 webm 文件失败: " + delEx.Message);
                            }

                        }
                    }
                    catch (Exception innerEx)
                    {
                        LOG.Info("ZZZ 背景视频转码失败，fallback handler 触发: " + innerEx.Message);
                        // finalVideoPath = FigFallbackHandler(NewWebmBackgroundPath);
                    }
                }

                ShowBackgroundVideo(finalVideoPath,TargetGame);
                LOG.Info("设置好背景图视频了");
            }
            catch (Exception ex)
            {
                LOG.Info(ex.ToString());
            }






            //如果使用上了视频背景图，那就不用管后面的内容了
            if (UseWebmBackground)
            {
                LOG.Info("用上视频背景图了，后面内容不管了");
                return;
            }

            //否则使用普通背景图
            string NewWebpBackgroundPath = await HoyoBackgroundUtils.DownloadLatestWebpBackground(BaseUrl);
            if (File.Exists(NewWebpBackgroundPath))
            {
                ShowBackgroundPicture(NewWebpBackgroundPath, TargetGame);

            }

        }


    }
}
