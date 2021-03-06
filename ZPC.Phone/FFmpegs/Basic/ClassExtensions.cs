using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;


namespace ZPC.Phone
{
    public static class ClassExtensions
    {
        #region Process

        // Suspends all threads of the current process
        public static void Suspend(this Process process)
        {
            IntPtr pOpenThread;

            if (!process.HasExited)
            {
                foreach (ProcessThread pT in process.Threads)
                {
                    pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        SuspendThread(pOpenThread);
                        CloseHandle(pOpenThread);
                    }
                }
            }
        }

        // Resumes all threads of the current process
        public static void Resume(this Process process)
        {
            IntPtr pOpenThread;
            int suspendCount;

            if (!process.HasExited)
            {
                foreach (ProcessThread pT in process.Threads)
                {
                    pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);
                    if (pOpenThread != IntPtr.Zero)
                    {
                        do
                        {
                            suspendCount = ResumeThread(pOpenThread);
                        }
                        while (suspendCount > 0);
                        CloseHandle(pOpenThread);
                    }
                }
            }
        }

        #region Native

        [Flags]
        public enum ThreadAccess : int
        {
            TERMINATE = (0x0001),
            SUSPEND_RESUME = (0x0002),
            GET_CONTEXT = (0x0008),
            SET_CONTEXT = (0x0010),
            SET_INFORMATION = (0x0020),
            QUERY_INFORMATION = (0x0040),
            SET_THREAD_TOKEN = (0x0080),
            IMPERSONATE = (0x0100),
            DIRECT_IMPERSONATION = (0x0200)
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);
        [DllImport("kernel32.dll")]
        static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool CloseHandle(IntPtr handle);

        #endregion

        #endregion

        #region HttpClient

        // Asynchroniously downloads a remote file into a stream, with progress reporting
        public static async Task DownloadAsync(this HttpClient client, string requestUri, Stream destinationStream, IProgress<(float, long, long)> progress)
        {
            using (HttpResponseMessage response = await client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false))
            {
                if (response.Content.Headers.ContentLength == null)
                {
                    throw new Exception("Couldn't determine file size");
                }
                var totalBytes = response.Content.Headers.ContentLength.Value;

                using (Stream downloadStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var relativeProgress = new Progress<long>(currentBytes => progress.Report(((float)currentBytes / totalBytes, currentBytes, totalBytes)));
                    await downloadStream.CopyToAsync(destinationStream, relativeProgress).ConfigureAwait(false);
                    destinationStream.Close(); // Necessary to allow the updater to extract the archive 
                    progress.Report((1f, totalBytes, totalBytes));
                }
            }
        }

        // Asynchroniously copies a stream into another, with progress reporting
        public static async Task CopyToAsync(this Stream source, Stream destination, IProgress<long> progress)
        {
            byte[] buffer = new byte[8192];
            long totalBytesRead = 0;
            int bytesRead;
            Stopwatch sw = new Stopwatch(); 

            sw.Start();
            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead).ConfigureAwait(false);
                totalBytesRead += bytesRead;
                if (sw.ElapsedMilliseconds > 500) // Reports progress only every 500 milliseconds
                {
                    sw.Restart();
                    progress.Report(totalBytesRead);
                }
            }
            sw.Stop();
        }

        #endregion

        public static string ToBytesString(this long l)
        {
            string suffix;
            double readable;

            if (l >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = l >> 30;
            }
            else if (l >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = l >> 20;
            }
            else if (l >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = l >> 10;
            }
            else if (l >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = l;
            }
            else
            {
                return l.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable /= 1024;
            // Return formatted number with suffix
            return readable.ToString("0.## ") + suffix;
        }

        // Formats a timespan to string, with rounding
        public static string ToFormattedString(this TimeSpan t, bool showMilliseconds = false)
        {
            double seconds = t.Seconds + (double)t.Milliseconds / 1000;
            if (showMilliseconds)
            {
                seconds = Math.Round(seconds, 2);
                return $"{t.Hours:00}:{t.Minutes:00}:{seconds:00.00}";
            }
            seconds = Math.Round(seconds, 0);
            return $"{t.Hours:00}:{t.Minutes:00}:{seconds:00}";
        }

        public static void PlayStoryboard(this FrameworkElement fe, string storyboardName)
        {
            Storyboard storyboard = fe.FindResource(storyboardName) as Storyboard;
            storyboard.Begin();
        }

        public static void StopStoryboard(this FrameworkElement fe, string storyboardName)
        {
            Storyboard storyboard = fe.FindResource(storyboardName) as Storyboard;
            storyboard.Stop();
        }
    }
}