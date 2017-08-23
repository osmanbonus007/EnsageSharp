using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TextureTool
{
    class TextureTool
    {
        private static void Main()
        {
            try
            {
                var GetFiles = Directory.GetFiles("./ensage_ui/", "*.png*", SearchOption.AllDirectories);
                foreach (var FullPath in GetFiles)
                {
                    File.WriteAllText(FullPath.Replace(".png", ".vmat"), 
                        "// THIS FILE IS AUTO-GENERATED\n" +
                        "\n" +
                        "Layer0\n" +
                        "{\n" +
                        "\tshader \"ui.vfx\"\n" +
                        "\n" +
                        "\t//---- Color ----\n" +
                        "\tTexture \"materials/" + FullPath.Substring(FullPath.IndexOf("ensage_ui")).Replace("\\", "/") + "\"\n" +
                        "}");
                }
                Task.Delay(2000).ContinueWith(_ =>
                {
                    Environment.Exit(0);
                });
                MessageBox.Show("Готово");
            }
            catch
            {
                Task.Delay(4000).ContinueWith(_ =>
                {
                    Environment.Exit(0);
                });
                MessageBox.Show("не удается найти папку ensage_ui! (папка ensage_ui должен быть рядом TextureTool)");
            }
            
        }
    }
}
