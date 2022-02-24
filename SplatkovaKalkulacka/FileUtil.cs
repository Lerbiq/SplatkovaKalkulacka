using System.IO;

namespace SplatkovaKalkulacka
{
    public class FileUtil
    {
        public static async void WriteToFile(string path, CalcResult result)
        {
            using (StreamWriter sw = new StreamWriter(path, append: true))
            {
                sw.Write("Dluh: {0}; Let: {1}; Urok: {2}; Mesicni splatka: {3}; Rocni splatka: {4}; Celkova splatka: {5};\n",
                    result.owed, result.years, result.interest, result.monthly, result.yearly, result.yearly * result.years);
            }
        }
    }
}