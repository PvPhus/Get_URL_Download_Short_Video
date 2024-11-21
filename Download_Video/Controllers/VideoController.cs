using System;
using System.Diagnostics;
using System.IO;
using System.Web.Mvc;

namespace VideoDownloader.Controllers
{
    public class VideoController : Controller
    {
        // Hiển thị giao diện nhập URL
        public ActionResult Index()
        {
            return View();
        }

        // Xử lý URL
        [HttpPost]
        public ActionResult ProcessVideo(string videoUrl)
        {
            if (string.IsNullOrEmpty(videoUrl))
            {
                ViewBag.Message = "URL không hợp lệ!";
                return View("Index");
            }

            try
            {
                // Sử dụng yt-dlp để lấy liên kết tải trực tiếp hoặc tải xuống
                string outputPath = GenerateDownload(videoUrl);

                if (string.IsNullOrEmpty(outputPath))
                {
                    ViewBag.Message = "Không thể xử lý video từ URL này!";
                    return View("Index");
                }

                // Trả về thông tin cho người dùng
                ViewBag.DownloadLink = outputPath; // Đường dẫn tải xuống
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Đã xảy ra lỗi: " + ex.Message;
            }

            return View("Index");
        }

        // Hàm xử lý tải video bằng yt-dlp
        private string GenerateDownload(string videoUrl)
        {
            string ytdlpPath = @"D:\Get_Url_Download_Short_Video\Download_Video\Download_Video\yt-dlp.exe";

            // Tạo thư mục lưu trữ video
            string outputDirectory = Server.MapPath("~/Downloads/");
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            string outputFile = Path.Combine(outputDirectory, Guid.NewGuid() + ".mp4");

            // Cấu hình yt-dlp để tải xuống video
            string arguments = $"\"{videoUrl}\" -o \"{outputFile}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ytdlpPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();

                if (process.ExitCode == 0 && System.IO.File.Exists(outputFile))
                {
                    // Trả về đường dẫn tải xuống tương đối
                    return "/Downloads/" + Path.GetFileName(outputFile);
                }
                else
                {
                    // Ghi lỗi từ yt-dlp nếu quá trình thất bại
                    string error = process.StandardError.ReadToEnd();
                    throw new Exception("yt-dlp lỗi: " + error);
                }
            }
        }
    }
}
