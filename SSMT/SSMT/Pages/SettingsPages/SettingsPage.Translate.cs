using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMT
{
    public partial class SettingsPage
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // ✅ 每次进入页面都会执行，适合刷新 UI
            // 因为开启了缓存模式之后，是无法刷新页面语言的，只能在这里执行来刷新
            TranslatePage();
        }

        public void TranslatePage()
        {

            if (GlobalConfig.Chinese)
            {
                TextBlock_SSMTCacheFolder.Text = "SSMT缓存文件存放路径设置";
                Button_ChooseSSMTPackageFolder.Content = "选择缓存文件存放路径";


                ToggleSwitch_Theme.OnContent = "曜石黑";
                ToggleSwitch_Theme.OffContent = "晨曦白";

                ToggleSwitch_Chinese.OnContent = "简体中文";
                ToggleSwitch_Chinese.OffContent = "英语";


                TextBlock_About.Text = "关于";

                HyperlinkButton_SubmitIssueAndFeedback.Content = "提交错误报告与使用反馈建议";
                
                TextBlock_Help.Text = "帮助";
                HyperlinkButton_SSMTDocuments.Content = "SSMT使用文档";
                HyperlinkButton_SSMTPluginTheHerta.Content = "SSMT的Blender插件TheHerta";
                HyperlinkButton_SSMTDiscord.Content = "SSMT Discord交流群";
                HyperlinkButton_SSMTQQGroup.Content = "SSMT QQ公开群 169930474";

                Run_SponsorSupport.Text = "赞助支持";
                HyperlinkButton_SSMTTechCommunity.Content = "SSMT技术社群";
                HyperlinkButton_AFDianNicoMico.Content = "爱发电:NicoMico";

                TextBlock_CheckForUpdates.Text = "检查版本更新";
                Button_AutoUpdate.Content = "自动检查新版本并更新";
                TextBlock_UpdateProgressing.Text = "自动更新下载进度:";

                TextBlock_WindowOpacitySetting.Text = "窗口透明度调整";
                Slider_LuminosityOpacity.Header = "透光度";

                TextBlock_ShowPagesSetting.Text = "页面显示设置";

                
                ToggleSwitch_ShowGameTypePage.OnContent = "显示 数据类型管理页面";
                ToggleSwitch_ShowGameTypePage.OffContent = "隐藏 数据类型管理页面";

                
                ToggleSwitch_ShowModManagePage.OnContent = "显示 Mod 管理页面";
                ToggleSwitch_ShowModManagePage.OffContent = "隐藏 Mod 管理页面";


                ToggleSwitch_ShowTextureToolBoxPage.OnContent = "显示 贴图工具箱页面";
                ToggleSwitch_ShowTextureToolBoxPage.OffContent = "隐藏 贴图工具箱页面";

                SettingsCard_Language.Header = "语言设置";
                SettingsCard_Language.Description = "设置SSMT的界面显示语言，仅支持中文和英文";

                SettingsCard_Theme.Header = "主题颜色设置";
                SettingsCard_Theme.Description = "设置SSMT的界面主题颜色，支持晨曦白和曜石黑两种主题颜色，推荐使用晨曦白";

            }
            else
            {
                TextBlock_SSMTCacheFolder.Text = "SSMT Cache Folder";
                Button_ChooseSSMTPackageFolder.Content = "Choose Cache Folder";



                ToggleSwitch_Theme.OnContent = "Dark";
                ToggleSwitch_Theme.OffContent = "Light";

                ToggleSwitch_Chinese.OnContent = "Chinese(zh-CN)";
                ToggleSwitch_Chinese.OffContent = "English(en-US)";


               

                TextBlock_About.Text = "About";

                HyperlinkButton_SubmitIssueAndFeedback.Content = "Submit Issue And Feedback";

                TextBlock_Help.Text = "Help";
                HyperlinkButton_SSMTDocuments.Content = "SSMT Documents";
                HyperlinkButton_SSMTPluginTheHerta.Content = "SSMT's Blender Plugin: TheHerta";
                HyperlinkButton_SSMTDiscord.Content = "SSMT Discord Server";
                HyperlinkButton_SSMTQQGroup.Content = "SSMT QQGroup 169930474";

                Run_SponsorSupport.Text = "Sponsor Support";
                HyperlinkButton_SSMTTechCommunity.Content = "SSMT Tech Community";
                HyperlinkButton_AFDianNicoMico.Content = "afdian: NicoMico";

                TextBlock_CheckForUpdates.Text = "Check Version Update";
                Button_AutoUpdate.Content = "Auto Update To Latest Version";
                TextBlock_UpdateProgressing.Text = "Auto Update Download Progress:";

                TextBlock_WindowOpacitySetting.Text = "Window Opacity";
                Slider_LuminosityOpacity.Header = "Luminosity Opacity";

                TextBlock_ShowPagesSetting.Text = "Pages Show Setting";

                ToggleSwitch_ShowGameTypePage.OnContent = "Show Game Type Management Page";
                ToggleSwitch_ShowGameTypePage.OffContent = "Hide Game Type Management Page";

                ToggleSwitch_ShowModManagePage.OnContent = "Show Mod Management Page";
                ToggleSwitch_ShowModManagePage.OffContent = "Hide Mod Management Page";


                ToggleSwitch_ShowTextureToolBoxPage.OnContent = "Show Texture Toolbox Page";
                ToggleSwitch_ShowTextureToolBoxPage.OffContent = "Hide Texture Toolbox Page";

                SettingsCard_Language.Header = "Language Setting";
                SettingsCard_Language.Description = "Decide what kind of language ssmt show,only support English and Chinese";

                SettingsCard_Theme.Header = "Theme Setting";
                SettingsCard_Theme.Description = "Decide what kind of theme color ssmt use, support Light and Dark theme, Light is recommended";

            }

        }

    }
}
