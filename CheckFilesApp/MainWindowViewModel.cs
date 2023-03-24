using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CheckFilesApp
{
    internal class MainWindowViewModel : INPC
    {
        const string FORB_FILE = "C:\\Users\\gdn\\source\\repos\\CheckFilesApp\\CheckFilesApp\\ForbiddenWords.txt";
        const string COPY_DIRECT_NAME = "Forbidden Files";

        public MainWindowViewModel()
        {
            LoadForbiddenWords();
        }

        string _selectDirectory;

        public string SelectDirectory
        {
            get { return _selectDirectory; }
            set
            {
                _selectDirectory = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> ForbiddenWords { get; set; }

        int _progress = 0;

        public int Progress
        {
            get { return _progress; }
        }

        public bool LoadForbiddenWords()
        {
            ForbiddenWords = new ObservableCollection<string>(File.ReadLines(FORB_FILE).ToList());
            OnPropertyChanged(nameof(ForbiddenWords));

            return true;
        }

        public bool SaveForbiddenWords()
        {
            File.WriteAllLines(FORB_FILE, ForbiddenWords);

            return true;
        }

        public async Task ScanDirectory()
        {
            List<string> files = Directory.GetFiles(_selectDirectory, "*.txt", SearchOption.AllDirectories).ToList();

            foreach (var file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    string line;
                    bool find = false;
                    while ((line = await reader.ReadLineAsync()) != null && !find)
                    {
                        foreach (var word in ForbiddenWords)
                        {
                            if (line.ToLower().Contains(word.ToLower()))
                            {
                                find = true;
                                var fi = new FileInfo(file);
                                File.Copy(file, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\{COPY_DIRECT_NAME}\\{fi.Name}_forbidden");
                                break;
                            }
                        }
                    }
                }
            }
        }

        public async Task ReplaceTextFiles()
        {
            List<string> files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + $"\\{COPY_DIRECT_NAME}").ToList();
            foreach (var file in files)
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    string line;

                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        foreach (var word in ForbiddenWords)
                        {
                            if (word.ToLower().Contains(word.ToLower()))
                            {
                                var replacedLine = Regex.Replace(line, word, new string('*', word.Length), RegexOptions.IgnoreCase);
                                using (StreamWriter writer = new StreamWriter(reader.BaseStream))
                                {
                                    writer.WriteLine(replacedLine);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
