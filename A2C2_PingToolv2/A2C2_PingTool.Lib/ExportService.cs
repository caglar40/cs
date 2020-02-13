using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2C2_PingTool.Lib
{
    class ExportService
    {
        string _filename;

        public ExportService(string filename)
        {
            _filename = filename;
        }

        public void Save(List<string> itemsToSave)
        {
            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(_filename);

                foreach (object item in itemsToSave)
                {
                    sw.WriteLine(item.ToString());
                }
            }

            catch (FileNotFoundException)
            {

            }

            finally
            {
                sw.Close();
            }
        }
    }
}
