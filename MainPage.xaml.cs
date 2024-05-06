using Microsoft.Maui.Controls;
using System.Net;
using CommunityToolkit.Maui.Alerts;
using FluentFTP;
using System.Diagnostics;
namespace TestTnRu
{
    public partial class MainPage : ContentPage
    {
        string[] tunisiaFTP = new string[] { "host", "user", "pass" }; // link - user - password 
        string[] russiaFTP = new string[] { "host", "user", "pass" };
        string localFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "downloaded_file");
        string filename = "files/Blogging.pdf";
        public MainPage()
        {
            InitializeComponent();
           _= CheckAndRequestStoragePermission();
        }

        public async Task CheckAndRequestStoragePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            if (status != PermissionStatus.Granted)
            {
                var t = Toast.Make("Permission denied. Can't download the file.");
                t.Show();
            }
        }
        private async void TunisiaButton_Clicked(object sender, EventArgs e)
        {
            using (var client = new AsyncFtpClient(tunisiaFTP[0], tunisiaFTP[1], tunisiaFTP[2]))
            {
                DownloadSpeedCalculator calculator = new DownloadSpeedCalculator();
                long totalBytes = 0;
                calculator.StartMeasurement(totalBytes);
                try
                {
                    DateTime startTime = DateTime.Now;
                    await client.Connect();
                    // Get the file size
                    long fileSize = await client.GetFileSize(filename);

                    // Convert the file size to KB
                    double fileSizeInKB = fileSize / 1024.0;

                    // Display the file size in a label
                    sizeLabel.Text += $"{fileSizeInKB:F2} KB";

                    await client.DownloadFile(localFilePath, filename, FtpLocalExists.Overwrite, FtpVerify.None, new Progress<FtpProgress>(p =>
                     {
                         // Update the total bytes read
                         totalBytes = p.TransferredBytes;

                         // Calculate the progress

                         // Update the progress bar

                         ProgressBar.Progress = p.Progress / 100;
                         // Calculate the download speed
                         double totalSeconds = (DateTime.Now - startTime).TotalSeconds;
                         double speed = totalBytes / totalSeconds;
                         percentLbl.Text = $"{(int)p.Progress}%";
                         // Update the download speed label
                         downloadSpeedlbl.Text = $"{speed / 1024:F2} KB/s";

                     }));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
                finally
                {
                    await client.Disconnect();
                }
                var t = Toast.Make("Download completed!");
                await t.Show();
                double averageSpeed = calculator.CalculateAverageSpeed(totalBytes);
                Dispatcher.Dispatch(() => {
                    percentLbl.Text = "100%";
                    TunisiaSpeedlbl.Text = $"{averageSpeed / 1024:F2} KB/s";
                });
            }
        }

      

        private async void RussiaButton_Clicked(object sender, EventArgs e)
        {
            using (var client = new AsyncFtpClient(russiaFTP[0], russiaFTP[1], russiaFTP[2]))
            {
                DownloadSpeedCalculator calculator = new DownloadSpeedCalculator();
                long totalBytes = 0;
                calculator.StartMeasurement(totalBytes);
                try
                {
                    DateTime startTime = DateTime.Now;
                    await client.Connect();
                    // Get the file size
                    long fileSize = await client.GetFileSize(filename);

                    // Convert the file size to KB
                    double fileSizeInKB = fileSize / 1024.0;

                    // Display the file size in a label
                    sizeLabel.Text += $"{fileSizeInKB:F2} KB";
                    await client.DownloadFile(localFilePath, filename, FtpLocalExists.Overwrite, FtpVerify.None, new Progress<FtpProgress>(p =>
                    {
                        // Update the total bytes read
                        totalBytes = p.TransferredBytes;



                        ProgressBar.Progress = p.Progress / 100;

                        double totalSeconds = (DateTime.Now - startTime).TotalSeconds;
                        double speed = totalBytes / totalSeconds;
                        percentLbl.Text = $"{(int)p.Progress}%";

                        downloadSpeedlbl.Text = $"{speed / 1024:F2} KB/s";

                    }));
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
                finally
                {
                    await client.Disconnect();
                }
                var t = Toast.Make("Download completed!");
                await t.Show();
                double averageSpeed = calculator.CalculateAverageSpeed(totalBytes);
                Dispatcher.Dispatch(() => {
                    percentLbl.Text = "100%";
                    TunisiaSpeedlbl.Text = $"{averageSpeed / 1024:F2} KB/s";
                });
            }
        }
    }
    public class DownloadSpeedCalculator
    {
        private Stopwatch stopwatch;
        private long initialBytes;

        public DownloadSpeedCalculator()
        {
            stopwatch = new Stopwatch();
        }

        public void StartMeasurement(long initialBytes)
        {
            this.initialBytes = initialBytes;
            stopwatch.Start();
        }

        public double CalculateAverageSpeed(long finalBytes)
        {
            stopwatch.Stop();
            long bytesDownloaded = finalBytes - initialBytes;
            double secondsElapsed = stopwatch.Elapsed.TotalSeconds;
            double averageSpeed = bytesDownloaded / secondsElapsed; // Bytes per second

            return averageSpeed;
        }
    }

}
