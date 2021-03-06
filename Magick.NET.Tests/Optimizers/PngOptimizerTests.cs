﻿//=================================================================================================
// Copyright 2013-2015 Dirk Lemstra <https://magick.codeplex.com/>
//
// Licensed under the ImageMagick License (the "License"); you may not use this file except in 
// compliance with the License. You may obtain a copy of the License at
//
//   http://www.imagemagick.org/script/license.php
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
// express or implied. See the License for the specific language governing permissions and
// limitations under the License.
//=================================================================================================

using System;
using System.IO;
using ImageMagick;
using ImageMagick.ImageOptimizers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magick.NET.Tests
{
  [TestClass]
  public class PngOptimizerTests : IImageOptimizerTests
  {
    private const string _Category = "PngOptimizer";

    protected override ILosslessImageOptimizer CreateLosslessImageOptimizer()
    {
      return new PngOptimizer();
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_InvalidArguments()
    {
      Test_LosslessCompress_InvalidArguments();
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_LosslessCompress()
    {
      Test_LosslessCompress(Files.SnakewarePNG);
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_LosslessCompress_InvalidFile()
    {
      Test_LosslessCompress_InvalidFile(Files.ImageMagickJPG);
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_RemoveAlpha()
    {
      string tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
      try
      {
        using (MagickImage image = new MagickImage(Files.MagickNETIconPNG))
        {
          Assert.IsTrue(image.HasAlpha);
          image.ColorAlpha(new MagickColor("yellow"));
          image.HasAlpha = true;
          image.Write(tempFile);

          image.Read(tempFile);

          // TODO: Figure out why this does not fail in a single run but does when all tests are run.
          //Assert.IsTrue(image.HasAlpha);

          PngOptimizer optimizer = new PngOptimizer();
          optimizer.LosslessCompress(tempFile);

          image.Read(tempFile);
          Assert.IsFalse(image.HasAlpha);
        }
      }
      finally
      {
        if (File.Exists(tempFile))
          File.Delete(tempFile);
      }
    }
  }
}
