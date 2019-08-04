// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Xunit.Sdk;

namespace NuGet.Tests.Apex
{
    public sealed class TakeScreenshotAttribute : BeforeAfterTestAttribute
    {
        private static readonly Lazy<DirectoryInfo> ScreenshotsDirectory = new Lazy<DirectoryInfo>(GetScreenshotsDirectory);

        public override void Before(MethodInfo methodUnderTest)
        {
        }

        public override void After(MethodInfo methodUnderTest)
        {
            try
            {
                if (ScreenshotsDirectory.Value == null)
                {
                    return;
                }

                string fileName = $"{DateTime.UtcNow.ToString("yyyyMMdd_HHmmss.fffffffZ")}_{methodUnderTest.DeclaringType.FullName}.{methodUnderTest.Name}.png";
                string filePath = Path.Combine(ScreenshotsDirectory.Value.FullName, fileName);

                using (Bitmap screenshot = TakeScreenshot())
                {
                    screenshot.Save(filePath, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                // Do not fail the test if we cannot take a screenshot.
            }
        }

        private static Bitmap TakeScreenshot()
        {
            Rectangle screenBounds = SystemInformation.VirtualScreen;

            var bitmap = new Bitmap(screenBounds.Width, screenBounds.Height);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(
                    screenBounds.X,
                    screenBounds.Y,
                    destinationX: 0,
                    destinationY: 0,
                    bitmap.Size,
                    CopyPixelOperation.SourceCopy);
            }

            return bitmap;
        }

        private static DirectoryInfo GetScreenshotsDirectory()
        {
            var value = Environment.GetEnvironmentVariable("ScreenshotsDirectoryPath");

            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var directoryInfo = new DirectoryInfo(value);

            directoryInfo.Create();

            return directoryInfo;
        }
    }
}
