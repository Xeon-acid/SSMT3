using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMT
{
    public partial class CoreFunctions
    {

        public static string AddLeadingZeros(int a)
        {
            // 固定补到5位数
            return a.ToString("D5");
        }
        public static void GenerateDynamicTextureMod(string DynamicTextureFolderPath, string TextureHash, string TextureSuffix)
        {

            if (!DynamicTextureFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                DynamicTextureFolderPath += Path.DirectorySeparatorChar;
            }

            string ConfigIniPath = Path.Combine(DynamicTextureFolderPath, "Config.ini");
            if (File.Exists(ConfigIniPath))
            {
                File.Delete(ConfigIniPath);
            }

            string[] TextureFileArray = Directory.GetFiles(DynamicTextureFolderPath, "*.dds", SearchOption.TopDirectoryOnly);
            int TextureFileNumber = TextureFileArray.Length;

            List<string> IniLineList = new List<string>();
            IniLineList.Add("[Constants]");
            IniLineList.Add("global $framevar = 0");
            IniLineList.Add("global $active");
            IniLineList.Add("global $fpsvar = 1");
            IniLineList.Add("");

            IniLineList.Add("[Present]");
            IniLineList.Add("post $active = 0");
            IniLineList.Add("");

            IniLineList.Add("if $active == 1 && $fpsvar < 6");
            IniLineList.Add("  $fpsvar = $fpsvar + 6");
            IniLineList.Add("endif");
            IniLineList.Add("");

            IniLineList.Add("if $fpsvar >= 6");
            IniLineList.Add("  $fpsvar = $fpsvar - 6");
            IniLineList.Add("  $framevar = $framevar + 1");
            IniLineList.Add("endif");
            IniLineList.Add("");

            IniLineList.Add(" if $framevar > " + TextureFileNumber.ToString());
            IniLineList.Add("  $framevar = 1");
            IniLineList.Add("endif");
            IniLineList.Add("");

            IniLineList.Add("[TextureOverride_Texture_" + TextureHash + "]");
            IniLineList.Add("hash = " + TextureHash);
            IniLineList.Add("run = CommandlistFrame");
            IniLineList.Add("$active = 1");
            IniLineList.Add("");

            IniLineList.Add("[CommandlistFrame]");
            for (int i = 1; i <= TextureFileNumber; i++)
            {
                string CurrentSuffix = AddLeadingZeros(i);
                if (i == 1)
                {
                    IniLineList.Add("if $framevar == " + CurrentSuffix);
                }
                else
                {
                    IniLineList.Add("else if $framevar == " + CurrentSuffix);
                }

                IniLineList.Add("  this = Resource_Frame_" + CurrentSuffix);


                if (i == TextureFileNumber)
                {
                    IniLineList.Add("endif");
                }
            }
            IniLineList.Add("");



            for (int i = 1; i <= TextureFileNumber; i++)
            {
                string CurrentSuffix = AddLeadingZeros(i);
                IniLineList.Add("[Resource_Frame_" + CurrentSuffix + "]");
                IniLineList.Add("filename = " + "frame_" + CurrentSuffix + TextureSuffix);
                IniLineList.Add("");
            }

            File.WriteAllLines(DynamicTextureFolderPath + "Config.ini", IniLineList);
        }

    }
}
