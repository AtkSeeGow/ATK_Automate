using Automate.Domain.Interface;
using Automate.Domain.Options;
using System.Diagnostics;

namespace Automate.Console
{
    internal class ConvertEncodingToBeatX: IHandle
    {
        public string Command { get; set; }

        private ConvertEncodingToBeatXOptions convertEncodingToBeatXOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="convertEncodingToBeatXOptions"></param>
        public ConvertEncodingToBeatX(
            string command,
            ConvertEncodingToBeatXOptions convertEncodingToBeatXOptions)
        {
            this.Command = command;
            this.convertEncodingToBeatXOptions = convertEncodingToBeatXOptions;
        }

        public void Execution(string[] args)
        {
            using (StreamWriter streamWriter = new StreamWriter(convertEncodingToBeatXOptions.LogPath, true))
            {
                this.executionJacketImage(streamWriter);
                this.executionVideoEncoding(streamWriter);
            }
        }

        #region ExecutionVideoEncoding

        private void executionVideoEncoding(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("-------------- ExecutionVideoEncoding --------------");

            var filePaths = Directory.GetFiles(this.convertEncodingToBeatXOptions.RootPath, "*", SearchOption.AllDirectories);
            foreach (var filePath in filePaths)
            {
                var extension = Path.GetExtension(filePath);
                if (extension == ".avi")
                {
                    var tulp = this.getVideoEncodingInfo($"-i \"{filePath}\"");

                    if (tulp.Item2.Contains("DX50"))
                    {
                        streamWriter.WriteLine(filePath);

                        this.convertVideoEncoding($"-i \"{filePath}\" -c:v libxvid -b:v 1500k -c:a copy -y \"{filePath}.avi\"");

                        File.Delete(filePath);
                        File.Move($"{filePath}.avi", filePath);
                    }
                }
            }

            streamWriter.WriteLine("----------------------------------------------------");
        }

        private Tuple<string, string> getVideoEncodingInfo(string arguments)
        {
            try
            {
                using (var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = this.convertEncodingToBeatXOptions.FFmpegPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                })
                {
                    process.Start();
                    process.WaitForExit();

                    var output = process.StandardOutput.ReadToEnd();
                    var error = process.StandardError.ReadToEnd();

                    return new Tuple<string, string>(output, error);
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            throw new Exception();
        }

        private void convertVideoEncoding(string arguments)
        {
            try
            {
                using (var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = this.convertEncodingToBeatXOptions.FFmpegPath,
                        Arguments = arguments
                    }
                })
                {
                    process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region ExecutionJacketImage

        private void executionJacketImage(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("--------------- ExecutionJacketImage ---------------");

            var folderPaths = this.getFolderPathsByB2(this.convertEncodingToBeatXOptions.RootPath);
            foreach (var folderPath in folderPaths)
            {
                var filePaths = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
                if (!filePaths.Any(item => item.Contains("-jacket")))
                {
                    streamWriter.WriteLine(folderPath);

                    var imageFilePaths = Array.FindAll(filePaths, isImageFilePath);
                    var largestImage = this.getLargestFile(imageFilePaths);

                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(largestImage);
                    var extension = Path.GetExtension(largestImage);
                    var fileName = $"{fileNameWithoutExtension}-jacket{extension}";

                    fileName = fileName
                        .Replace("-bg", "")
                        .Replace("-BG", "")
                        .Replace("BG", "")
                        .Replace("BG1", "")
                        .Replace("BG2", "");

                    var jacketFilePath = Path.Combine(folderPath, fileName);
                    streamWriter.WriteLine(jacketFilePath);

                    File.Copy(largestImage, jacketFilePath, true);
                }
            }

            streamWriter.WriteLine("----------------------------------------------------");
        }

        private string[] getFolderPathsByB2(string path)
        {
            var result = new HashSet<string>();

            var _paths = Directory.GetDirectories(path);
            foreach (var _item in _paths)
            {
                var __paths = Directory.GetDirectories(_item);
                foreach (var __item in __paths)
                    result.Add(__item);
            }

            return result.ToArray();
        }

        private bool isImageFilePath(string filePath)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png" };
            string extension = Path.GetExtension(filePath);
            return Array.Exists(imageExtensions, e => e.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        private string getLargestFile(string[] files)
        {
            string largestFile = files.Where(item => !item.Contains("Banner")).OrderByDescending(f => new FileInfo(f).Length).First();
            return largestFile;
        }

        #endregion
    }
}