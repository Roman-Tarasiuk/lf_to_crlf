using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace lf_to_crlf
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!CheckArgs(args))
            {
                ShowProgramUsage();
                return;
            }

            var path = Path.GetFullPath(args[0]);

            string extensions = String.Empty;
            if (args.Length >= 2)
            {
                extensions = args[1];
                ProcessFiles(path, extensions);
            }
            else
            {
                ProcessFile(path);
            }
        }

        static bool CheckArgs(string[] args)
        {
            if (args.Length == 0)
            {
                return false;
            }

            try
            {
                FileInfo fi = new FileInfo(args[0]);
                if (fi.Exists)
                {
                    return true;
                }

                DirectoryInfo di = new DirectoryInfo(args[0]);
                if (!di.Exists)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        static void ShowProgramUsage()
        {
            string usage =
@"
lf_to_crlf usage:

lf_to_crlf filename|directory_name [extension_list]

  where extension_list is separated by semicolon (;) list
";

            Console.WriteLine(usage);
        }

        static void ProcessFiles(string path, string extensions)
        {
            DirectoryInfo dir = new DirectoryInfo(path);

            string[] extList = null;

            if ((extensions != null) && (extensions != String.Empty))
            {
                extList = extensions.Split(new char[] { ';' });
            }

            FileInfo[] files = dir.GetFiles();
            foreach (var fi in files)
            {
                bool fileMatches = false;

                if (extList != null)
                {
                    foreach (var e in extList)
                    {
                        if (fi.Extension == ("." + e))
                        {
                            fileMatches = true;
                            break;
                        }
                    }
                }

                if (fileMatches)
                {
                    ProcessFile(fi.FullName);
                }
            }

            DirectoryInfo[] directories = dir.GetDirectories();
            foreach (var di in directories)
            {
                ProcessFiles(di.FullName, extensions);
            }
        }

        static void ProcessFile(string path)
        {
            Console.Write("Processing {0} ...", path);
            try
            {
                string fileContent = null;

                using (StreamReader reader = new StreamReader(path))
                {
                    fileContent = reader.ReadToEnd();
                }

                var newFileContent = fileContent.Replace("\n", "\r\n");
                newFileContent = newFileContent.Replace("\r\r", "\r");

                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.Write(newFileContent);
                }

                Console.WriteLine(" – Complete.");
            }
            catch (Exception)
            {
                Console.WriteLine(" – Failed!");
            }
        }
    }
}
