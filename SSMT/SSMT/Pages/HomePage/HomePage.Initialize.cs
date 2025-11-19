using Microsoft.UI.Xaml;
using SSMT_Core;
using SSMT_Core.InfoItemClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMT
{
    public partial class HomePage
    {

        /// <summary>
        /// 逻辑名称是固定的，需要手动管理
        /// </summary>
        private void InitializeLogicNameList()
        {
            IsLoading = true;

            ComboBox_LogicName.Items.Clear();

            ComboBox_LogicName.Items.Add(LogicName.GIMI);
            ComboBox_LogicName.Items.Add(LogicName.HIMI);
            ComboBox_LogicName.Items.Add(LogicName.SRMI);
            ComboBox_LogicName.Items.Add(LogicName.ZZMI);
            ComboBox_LogicName.Items.Add(LogicName.WWMI);
            ComboBox_LogicName.Items.Add(LogicName.UnityCPU);
            ComboBox_LogicName.Items.Add(LogicName.CTXMC);
            ComboBox_LogicName.Items.Add(LogicName.IdentityV2);
            ComboBox_LogicName.Items.Add(LogicName.YYSLS);
            ComboBox_LogicName.Items.Add(LogicName.AILIMIT);
            ComboBox_LogicName.Items.Add(LogicName.UnityVS);
            ComboBox_LogicName.Items.Add(LogicName.UnityCS);
            ComboBox_LogicName.Items.Add(LogicName.SnowBreak);
            ComboBox_LogicName.Items.Add(LogicName.HOK);
            ComboBox_LogicName.Items.Add(LogicName.NierR);

            IsLoading = false;
        }

        /// <summary>
        /// 读取数据类型文件夹下的所有游戏名称的文件夹名称，然后放到下拉菜单中供后续选择
        /// </summary>
        private void InitializeGameTypeFolderList()
        {
            IsLoading = true;

            ComboBox_GameTypeFolder.Items.Clear();

            string[] GameTypeFolderPathList = Directory.GetDirectories(PathManager.Path_GameTypeConfigsFolder);

            foreach (string GameTypeFolderPath in GameTypeFolderPathList)
            {
                string GameTypeFolderName = Path.GetFileName(GameTypeFolderPath);
                ComboBox_GameTypeFolder.Items.Add(GameTypeFolderName);
            }

            IsLoading = false;
        }


        /// <summary>
        /// 读取所有创建的游戏名称，然后根据配置文件决定是否显示图标
        /// </summary>
        private void InitializeGameIconItemList()
        {
            IsLoading = true;
            //初始化图标列表
            if (!Directory.Exists(PathManager.Path_GamesFolder))
            {
                return;
            }

            string[] GamesFolderList = Directory.GetDirectories(PathManager.Path_GamesFolder);

            GameIconItemList.Clear();

            foreach (string GameFolderPath in GamesFolderList)
            {
                string GameFolderName = Path.GetFileName(GameFolderPath);

                GameIconConfig gameIconConfig = new GameIconConfig();

                if (!gameIconConfig.GameName_Show_Dict.ContainsKey(GameFolderName))
                {
                    continue;
                }

                if (!gameIconConfig.GameName_Show_Dict[GameFolderName])
                {
                    continue;
                }

                GameIconItem gameIconItem = new GameIconItem();
                gameIconItem.GameName = GameFolderName;
                gameIconItem.GameIconImage = Path.Combine(PathManager.Path_GamesFolder, GameFolderName + "\\Icon.png");
                GameIconItemList.Add(gameIconItem);
            }

            IsLoading = false;
        }


        /// <summary>
        /// 上面读取游戏图标列表只是把图标列表给加载上，但是还没有选中
        /// 这里我们读取了游戏名称列表，放到下拉菜单中，并且读取历史配置来决定选中哪一个
        /// 自动触发游戏名称改变时的方法，然后游戏图标列表也会自动选中到对应的项
        /// 
        /// 注意:不在isLoading中调用会触发游戏改变方法
        /// </summary>
        private void InitializeGameNameList()
        {
            if (!Directory.Exists(PathManager.Path_GamesFolder))
            {
                return;
            }

            string[] GamesFolderList = Directory.GetDirectories(PathManager.Path_GamesFolder);

            ComboBox_GameName.Items.Clear();

            foreach (string GameFolderPath in GamesFolderList)
            {
                string GameFolderName = Path.GetFileName(GameFolderPath);

                ComboBox_GameName.Items.Add(GameFolderName);
            }

            if (ComboBox_GameName.Items.Contains(GlobalConfig.CurrentGameName))
            {
                ComboBox_GameName.SelectedItem = GlobalConfig.CurrentGameName;
            }
            else
            {
                if (ComboBox_GameName.Items.Count > 0)
                {
                    ComboBox_GameName.SelectedIndex = 0;
                }
            }
        }

        private async Task InitializeBackground()
        {

            // 默认：隐藏视频，清空图片
            if (BackgroundVideo != null)
            {
                BackgroundVideo.SetMediaPlayer(null);
                BackgroundVideo.Visibility = Visibility.Collapsed;
            }

            //清空图片内容并且设置为不显示，防止上一个游戏切换过来时，
            //由于缓存导致新切换到的游戏没有背景图时显示上一个游戏的背景图残留
            MainWindowImageBrush.Source = null;
            MainWindowImageBrush.Visibility = Visibility.Collapsed;

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

            bool BackgroundExists = false;
            foreach (BackgroundSuffixItem SuffixItem in SuffixList)
            {
                string BackgroundFilePath = Path.Combine(PathManager.Path_CurrentGamesFolder, "Background" + SuffixItem.Suffix);

                if (!File.Exists(BackgroundFilePath))
                {
                    continue;
                }

                if (SuffixItem.IsVideo)
                {
                    ShowBackgroundVideo(BackgroundFilePath);
                    BackgroundExists = true;
                    break;
                }
                else if (SuffixItem.IsPicture)
                {
                    ShowBackgroundPicture(BackgroundFilePath);
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
                    string CurrentGameName = ComboBox_GameName.SelectedItem.ToString();

                    if (CurrentGameName == LogicName.GIMI ||
                        CurrentGameName == LogicName.SRMI ||
                        CurrentGameName == LogicName.HIMI ||
                        CurrentGameName == LogicName.ZZMI
                        )
                    {
                        //_ = SSMTMessageHelper.Show(CurrentLogicName);
                        string PossibleWebpPicture = Path.Combine(PathManager.Path_CurrentGamesFolder, "Background.webp");
                        string PossiblePngBackgroundPath = Path.Combine(PathManager.Path_CurrentGamesFolder, "Background.png");


                        if (!File.Exists(PossibleWebpPicture))
                        {
                            if (!File.Exists(PossiblePngBackgroundPath))
                            {
                                //自动加载当前背景图，因为满足LogicName且并未设置背景图
                                await AutoUpdateBackgroundPicture(CurrentGameName);
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






    }
}
