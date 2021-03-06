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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ImageMagick;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if Q8
using QuantumType = System.Byte;
#elif Q16
using QuantumType = System.UInt16;
#elif Q16HDRI
	using QuantumType = System.Single;
#else
#error Not implemented!
#endif

#if !(NET20)
using System.Windows.Media.Imaging;
#endif

namespace Magick.NET.Tests
{
  [TestClass]
  public class MagickImageTests
  {
    private const string _Category = "MagickImage";

    private static void ShouldNotRaiseWarning(object sender, WarningEventArgs arguments)
    {
      Assert.Fail(arguments.Message);
    }

    private static void ShouldRaiseWarning(object sender, WarningEventArgs arguments)
    {
      Assert.IsNotNull(arguments.Message);
    }

    private static void Test_Clip(bool inside, QuantumType value)
    {
      using (MagickImage image = new MagickImage(Files.InvitationTif))
      {
        image.Alpha(AlphaOption.Transparent);
        image.Clip("Pad A", inside);
        image.Alpha(AlphaOption.Opaque);

        using (MagickImage mask = image.Mask)
        {
          Assert.IsNotNull(mask);
          Assert.AreEqual(false, mask.HasAlpha);

          using (PixelCollection pixels = mask.GetReadOnlyPixels())
          {
            var pixelA = pixels.GetPixel(0, 0).ToColor();
            var pixelB = pixels.GetPixel(pixels.Width - 1, pixels.Height - 1).ToColor();

            Assert.AreEqual(pixelA, pixelB);
            Assert.AreEqual(value, pixelA.R);
            Assert.AreEqual(value, pixelA.G);
            Assert.AreEqual(value, pixelA.B);
            Assert.AreEqual(Quantum.Max, pixelA.A);
          }
        }
      }
    }

    private static void Test_Clone(MagickImage first, MagickImage second)
    {
      Assert.AreEqual(first, second);
      second.Format = MagickFormat.Jp2;
      Assert.AreEqual(first.Format, MagickFormat.Png);
      Assert.AreEqual(second.Format, MagickFormat.Jp2);
      second.Dispose();
      Assert.AreEqual(first.Format, MagickFormat.Png);
    }

    private static void Test_Clone_Area(MagickImage area, MagickImage part)
    {
      Assert.AreEqual(area.Width, part.Width);
      Assert.AreEqual(area.Height, part.Height);

      Assert.AreEqual(0.0, area.Compare(part, ErrorMetric.RootMeanSquared));
    }

    private static void Test_Pixel_Equal(MagickImage image, int x, int y, MagickColor color)
    {
      using (PixelCollection collection = image.GetReadOnlyPixels())
      {
        ColorAssert.AreEqual(color, collection.GetPixel(x, y));
      }
    }

    private static void Test_Pixel_NotEqual(MagickImage image, int x, int y, MagickColor color)
    {
      using (PixelCollection collection = image.GetReadOnlyPixels())
      {
        ColorAssert.AreNotEqual(color, collection.GetPixel(x, y));
      }
    }

    private static void Test_Separate_Composite(MagickImage image, ColorSpace colorSpace, int value)
    {
      Assert.AreEqual(colorSpace, image.ColorSpace);

      using (PixelCollection pixels = image.GetReadOnlyPixels())
      {
        Pixel pixel = pixels.GetPixel(340, 260);
        ColorAssert.AreEqual(Color.FromArgb(value, value, value), pixel.ToColor());
      }
    }

    private static void Test_ToBitmap(MagickImage image, ImageFormat format)
    {
      using (Bitmap bmp = image.ToBitmap(format))
      {
        Assert.AreEqual(format, bmp.RawFormat);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_AddProfile()
    {
      using (MagickImage image = new MagickImage(Files.SnakewarePNG))
      {
        ColorProfile profile = image.GetColorProfile();
        Assert.IsNull(profile);

        image.AddProfile(ColorProfile.SRGB);
        profile = image.GetColorProfile();
        Assert.IsNotNull(profile);
        Assert.AreEqual(3144, profile.ToByteArray().Length);

        image.AddProfile(ColorProfile.AppleRGB, false);
        profile = image.GetColorProfile();
        Assert.IsNotNull(profile);
        Assert.AreEqual(3144, profile.ToByteArray().Length);

        image.AddProfile(ColorProfile.AppleRGB);
        profile = image.GetColorProfile();
        Assert.IsNotNull(profile);
        Assert.AreEqual(552, profile.ToByteArray().Length);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Artifact()
    {
      using (MagickImage image = new MagickImage(Files.SnakewarePNG))
      {
        Assert.IsNull(image.GetArtifact("test"));

        image.SetArtifact("test", "");
        Assert.AreEqual(null, image.GetArtifact("test"));

        image.SetArtifact("test", "123");
        Assert.AreEqual("123", image.GetArtifact("test"));

        image.SetAttribute("foo", "bar");

        IEnumerable<string> names = image.ArtifactNames;
        Assert.AreEqual(1, names.Count());
        Assert.AreEqual("test", string.Join(",", (from name in names
                                                  orderby name
                                                  select name).ToArray()));
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Attribute()
    {
      using (MagickImage image = new MagickImage(Files.ImageMagickJPG))
      {
        Assert.IsNull(image.GetAttribute("test"));

        IEnumerable<string> names = image.AttributeNames;
        Assert.AreEqual(4, names.Count());

        image.SetAttribute("test", "");
        Assert.AreEqual(null, image.GetAttribute("test"));

        image.SetAttribute("test", "123");
        Assert.AreEqual("123", image.GetAttribute("test"));

        image.SetArtifact("foo", "bar");

        names = image.AttributeNames;
        Assert.AreEqual(5, names.Count());
        Assert.AreEqual("date:create,date:modify,jpeg:colorspace,jpeg:sampling-factor,test", string.Join(",", (from name in names
                                                                                                               orderby name
                                                                                                               select name).ToArray()));
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_BackgroundColor()
    {
      using (MagickImage image = new MagickImage("xc:red", 1, 1))
      {
        ColorAssert.AreEqual(new MagickColor("White"), image.BackgroundColor);
      }

      MagickColor red = new MagickColor("Red");

      using (MagickImage image = new MagickImage(red, 1, 1))
      {
        ColorAssert.AreEqual(red, image.BackgroundColor);

        image.Read(new MagickColor("Purple"), 1, 1);

        ColorAssert.AreEqual(red, image.BackgroundColor);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_BitDepth()
    {
      using (MagickImage image = new MagickImage(Files.RoseSparkleGIF))
      {
        Assert.AreEqual(8, image.BitDepth());

        image.Threshold((Percentage)50);
        Assert.AreEqual(1, image.BitDepth());
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Bitmap()
    {
      using (Bitmap bitmap = new Bitmap(400, 400, PixelFormat.Format24bppRgb))
      {
        using (MagickImage image = new MagickImage(bitmap))
        {
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Channels()
    {
      PixelChannel[] rgb = new PixelChannel[]
      {
        PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue
      };

      PixelChannel[] rgba = new PixelChannel[]
      {
        PixelChannel.Red, PixelChannel.Green, PixelChannel.Blue, PixelChannel.Alpha
      };

      PixelChannel[] gray = new PixelChannel[]
      {
        PixelChannel.Gray
      };

      PixelChannel[] grayAlpha = new PixelChannel[]
      {
        PixelChannel.Gray, PixelChannel.Alpha
      };

      PixelChannel[] cmyk = new PixelChannel[]
      {
        PixelChannel.Cyan, PixelChannel.Magenta, PixelChannel.Yellow, PixelChannel.Black
      };

      PixelChannel[] cmyka = new PixelChannel[]
      {
        PixelChannel.Cyan, PixelChannel.Magenta, PixelChannel.Yellow, PixelChannel.Black, PixelChannel.Alpha
      };

      using (MagickImage image = new MagickImage(Files.RoseSparkleGIF))
      {
        CollectionAssert.AreEqual(rgba, image.Channels.ToArray());

        image.Alpha(AlphaOption.Off);

        CollectionAssert.AreEqual(rgb, image.Channels.ToArray());
      }

      using (MagickImage image = new MagickImage(Files.SnakewarePNG))
      {
        CollectionAssert.AreEqual(grayAlpha, image.Channels.ToArray());

        using (MagickImage redChannel = image.Separate(Channels.Red).First())
        {
          CollectionAssert.AreEqual(gray, redChannel.Channels.ToArray());

          redChannel.Alpha(AlphaOption.On);

          CollectionAssert.AreEqual(grayAlpha, redChannel.Channels.ToArray());
        }
      }

      using (MagickImage image = new MagickImage(Files.SnakewarePNG))
      {
        image.ColorSpace = ColorSpace.CMYK;

        CollectionAssert.AreEqual(cmyka, image.Channels.ToArray());

        image.Alpha(AlphaOption.Off);

        CollectionAssert.AreEqual(cmyk, image.Channels.ToArray());
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Clip()
    {
      Test_Clip(false, Quantum.Max);
      Test_Clip(true, 0);
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Clone()
    {
      using (MagickImage first = new MagickImage(Files.SnakewarePNG))
      {
        using (MagickImage second = first.Clone())
        {
          Test_Clone(first, second);
        }

        using (MagickImage second = new MagickImage(first))
        {
          Test_Clone(first, second);
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Clone_Area()
    {
      using (MagickImage icon = new MagickImage(Files.MagickNETIconPNG))
      {
        using (MagickImage area = icon.Clone())
        {
          area.Crop(64, 64, Gravity.Southeast);
          area.RePage();
          Assert.AreEqual(64, area.Width);
          Assert.AreEqual(64, area.Height);

          area.Crop(64, 32, Gravity.North);

          Assert.AreEqual(64, area.Width);
          Assert.AreEqual(32, area.Height);

          using (MagickImage part = icon.Clone(new MagickGeometry(64, 64, 64, 32)))
          {
            Test_Clone_Area(area, part);
          }

          using (MagickImage part = icon.Clone(64, 64, 64, 32))
          {
            Test_Clone_Area(area, part);
          }
        }

        using (MagickImage area = icon.Clone())
        {
          area.Crop(32, 64, Gravity.Northwest);

          Assert.AreEqual(32, area.Width);
          Assert.AreEqual(64, area.Height);

          using (MagickImage part = icon.Clone(32, 64))
          {
            Test_Clone_Area(area, part);
          }
        }

        using (MagickImage area = icon.Clone(4, 2))
        {
          Assert.AreEqual(4, area.Width);
          Assert.AreEqual(2, area.Height);

          ExceptionAssert.Throws<MagickMissingDelegateErrorException>(delegate ()
          {
            area.ToByteArray();
          });

#if Q8
          Assert.AreEqual(32, area.ToByteArray(MagickFormat.Rgba).Length);
#elif Q16 || Q16HDRI
          Assert.AreEqual(64, area.ToByteArray(MagickFormat.Rgba).Length);
#else
#error Not implemented!
#endif
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_ColorAlpha()
    {
      using (MagickImage image = new MagickImage(Files.MagickNETIconPNG))
      {
        MagickColor purple = new MagickColor("purple");

        image.ColorAlpha(purple);

        using (PixelCollection pixels = image.GetReadOnlyPixels())
        {
          ColorAssert.AreNotEqual(purple, pixels.GetPixel(45, 75).ToColor());
          ColorAssert.AreEqual(purple, pixels.GetPixel(100, 60).ToColor());
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Constructor()
    {
      ExceptionAssert.Throws<ArgumentException>(delegate ()
      {
        new MagickImage(new byte[0]);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        new MagickImage((byte[])null);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        new MagickImage((Bitmap)null);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        new MagickImage((FileInfo)null);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        new MagickImage((Stream)null);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        new MagickImage((string)null);
      });

      ExceptionAssert.Throws<ArgumentException>(delegate ()
      {
        new MagickImage(Files.Missing);
      });

      using (Bitmap bitmap = new Bitmap(Files.SnakewarePNG))
      {
        using (MagickImage image = new MagickImage(bitmap))
        {
          Assert.AreEqual(MagickFormat.Png, image.Format);
        }
      }

      using (Bitmap bitmap = new Bitmap(100, 100, PixelFormat.Format24bppRgb))
      {
        using (MagickImage image = new MagickImage(bitmap))
        {
          Assert.AreEqual(MagickFormat.Bmp3, image.Format);
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_ColorType()
    {
      using (MagickImage image = new MagickImage(Files.WireframeTIF))
      {
        Assert.AreEqual(ColorType.TrueColor, image.ColorType);
        using (MemoryStream memStream = new MemoryStream())
        {
          image.Write(memStream);
          memStream.Position = 0;
          using (MagickImage result = new MagickImage(memStream))
          {
            Assert.AreEqual(ColorType.Grayscale, image.ColorType);
          }
        }
      }

      using (MagickImage image = new MagickImage(Files.WireframeTIF))
      {
        Assert.AreEqual(ColorType.TrueColor, image.ColorType);
        image.PreserveColorType();
        using (MemoryStream memStream = new MemoryStream())
        {
          image.Write(memStream);
          memStream.Position = 0;
          using (MagickImage result = new MagickImage(memStream))
          {
            Assert.AreEqual(ColorType.TrueColor, image.ColorType);
          }
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Compare()
    {
      MagickImage first = new MagickImage(Files.SnakewarePNG);
      MagickImage second = first.Clone();

      MagickErrorInfo same = first.Compare(second);
      Assert.IsNotNull(same);
      Assert.AreEqual(0, same.MeanErrorPerPixel);

      double distortion = first.Compare(second, ErrorMetric.Absolute);
      Assert.AreEqual(0, distortion);

      first.Threshold(new Percentage(50));
      MagickErrorInfo different = first.Compare(second);
      Assert.IsNotNull(different);
      Assert.AreNotEqual(0, different.MeanErrorPerPixel);

      distortion = first.Compare(second, ErrorMetric.Absolute);
      Assert.AreNotEqual(0, distortion);

      MagickImage difference = new MagickImage();
      distortion = first.Compare(second, ErrorMetric.RootMeanSquared, difference);
      Assert.AreNotEqual(0, distortion);
      Assert.AreNotEqual(first, difference);
      Assert.AreNotEqual(second, difference);
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Composite_ChangeMask()
    {
      using (MagickImage background = new MagickImage("xc:red", 100, 100))
      {
        background.BackgroundColor = Color.White;
        background.Extent(200, 100);

        IDrawable[] drawables = new IDrawable[]
        {
          new DrawablePointSize(50),
          new DrawableText(135, 70, "X")
        };

        using (MagickImage image = background.Clone())
        {
          image.Draw(drawables);
          image.Composite(background, Gravity.Center, CompositeOperator.ChangeMask);

          using (MagickImage result = new MagickImage(MagickColor.Transparent, 200, 100))
          {
            result.Draw(drawables);
            Assert.AreEqual(0.073, result.Compare(image, ErrorMetric.RootMeanSquared), 0.001);
          }
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Composite_Blur()
    {
      using (MagickImage image = new MagickImage(Files.Builtin.Logo))
      {
        using (MagickImage blur = new MagickImage(new MagickColor("#000"), image.Width, image.Height))
        {
          image.Warning += ShouldNotRaiseWarning;
          image.Composite(blur, Gravity.Center, CompositeOperator.Blur, "3");
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_CopyPixels()
    {
      using (MagickImage source = new MagickImage(Color.White, 100, 100))
      {
        using (MagickImage destination = new MagickImage(Color.Black, 50, 50))
        {
          ExceptionAssert.Throws<MagickOptionErrorException>(delegate ()
          {
            destination.CopyPixels(source, new MagickGeometry(51, 50), new Coordinate(0, 0));
          });

          ExceptionAssert.Throws<MagickOptionErrorException>(delegate ()
          {
            destination.CopyPixels(source, new MagickGeometry(50, 51), new Coordinate(0, 0));
          });

          ExceptionAssert.Throws<MagickOptionErrorException>(delegate ()
          {
            destination.CopyPixels(source, new MagickGeometry(50, 50), new Coordinate(1, 0));
          });

          ExceptionAssert.Throws<MagickOptionErrorException>(delegate ()
          {
            destination.CopyPixels(source, new MagickGeometry(50, 50), new Coordinate(0, 1));
          });

          destination.CopyPixels(source, new MagickGeometry(25, 25), new Coordinate(25, 25));

          Test_Pixel_Equal(destination, 0, 0, Color.Black);
          Test_Pixel_Equal(destination, 24, 24, Color.Black);
          Test_Pixel_Equal(destination, 25, 25, Color.White);
          Test_Pixel_Equal(destination, 49, 49, Color.White);
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Define()
    {
      using (MagickImage image = new MagickImage(Files.Builtin.Logo))
      {
        string option = "optimize-coding";

        image.SetDefine(MagickFormat.Jpg, option, true);
        Assert.AreEqual("true", image.GetDefine(MagickFormat.Jpg, option));
        Assert.AreEqual("true", image.GetDefine(MagickFormat.Jpeg, option));

        image.RemoveDefine(MagickFormat.Jpeg, option);
        Assert.AreEqual(null, image.GetDefine(MagickFormat.Jpg, option));

        image.SetDefine(MagickFormat.Jpeg, option, "test");
        Assert.AreEqual("test", image.GetDefine(MagickFormat.Jpg, option));
        Assert.AreEqual("test", image.GetDefine(MagickFormat.Jpeg, option));

        image.RemoveDefine(MagickFormat.Jpg, option);
        Assert.AreEqual(null, image.GetDefine(MagickFormat.Jpeg, option));

        image.SetDefine(MagickFormat.Unknown, "profile:skip", "ICC");
        Assert.AreEqual("ICC", image.GetDefine(MagickFormat.Unknown, "profile:skip"));
      }
    }

    [TestMethod, TestCategory(_Category)]
    [ExpectedException(typeof(ObjectDisposedException))]
    public void Test_Dispose()
    {
      MagickImage image = new MagickImage();
      image.Dispose();
      image.Verbose = true;
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Drawable()
    {
      using (MagickImage image = new MagickImage(Color.Red, 10, 10))
      {
        MagickColor yellow = Color.Yellow;
        image.Draw(new DrawableFillColor(yellow), new DrawableRectangle(0, 0, 10, 10));
        Test_Pixel_Equal(image, 5, 5, yellow);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Extent()
    {
      using (MagickImage image = new MagickImage())
      {
        image.Read(Files.RedPNG);
        image.Resize(new MagickGeometry(100, 100));
        Assert.AreEqual(100, image.Width);
        Assert.AreEqual(33, image.Height);

        image.BackgroundColor = MagickColor.Transparent;
        image.Extent(100, 100, Gravity.Center);
        Assert.AreEqual(100, image.Width);
        Assert.AreEqual(100, image.Height);

        Test_Pixel_Equal(image, 0, 0, MagickColor.Transparent);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_FillColor()
    {
      using (MagickImage image = new MagickImage(MagickColor.Transparent, 100, 100))
      {
        image.FillColor = null;

        Pixel pixelA;
        image.FillColor = Color.Red;
        image.Read("caption:Magick.NET");
        using (PixelCollection pixels = image.GetReadOnlyPixels())
        {
          pixelA = pixels.GetPixel(69, 6);
        }

        Pixel pixelB;
        image.FillColor = Color.Yellow;
        image.Read("caption:Magick.NET");
        using (PixelCollection pixels = image.GetReadOnlyPixels())
        {
          pixelB = pixels.GetPixel(69, 6);
        }

        ColorAssert.AreNotEqual(pixelA, pixelB);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_FontTypeMetrics()
    {
      using (MagickImage image = new MagickImage(MagickColor.Transparent, 100, 100))
      {
        image.Font = "Arial";
        image.FontPointsize = 15;
        TypeMetric typeMetric = image.FontTypeMetrics("Magick.NET");
        Assert.IsNotNull(typeMetric);
        Assert.AreEqual(17, typeMetric.TextHeight);
        Assert.AreEqual(82.64, typeMetric.TextWidth, 0.01);

        image.FontPointsize = 150;
        typeMetric = image.FontTypeMetrics("Magick.NET");
        Assert.IsNotNull(typeMetric);
        Assert.AreEqual(168, typeMetric.TextHeight);
        Assert.AreEqual(810.48, typeMetric.TextWidth, 0.01);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_FormatExpression()
    {
      using (MagickImage image = new MagickImage(Files.RedPNG))
      {
        ExceptionAssert.Throws<ArgumentNullException>(delegate ()
        {
          image.FormatExpression(null);
        });

        Assert.AreEqual("FOO", image.FormatExpression("FOO"));
        Assert.AreEqual("OO", image.FormatExpression("%FOO"));
        image.Warning += ShouldRaiseWarning;
        Assert.AreEqual(null, image.FormatExpression("%FOO"));
        image.Warning -= ShouldRaiseWarning;

        Assert.AreEqual("a48a7f2fdc26e9ccf75b0c85a254c958f004cc182d0ca8c3060c1df734645367", image.FormatExpression("%#"));
      }

      using (MagickImage image = new MagickImage(Files.InvitationTif))
      {
        Assert.AreEqual("sRGB IEC61966-2.1", image.FormatExpression("%[profile:icc]"));
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_FormatInfo()
    {
      using (MagickImage image = new MagickImage(Files.SnakewarePNG))
      {
        MagickFormatInfo info = image.FormatInfo;

        Assert.IsNotNull(info);
        Assert.AreEqual(MagickFormat.Png, info.Format);
        Assert.AreEqual("image/png", info.MimeType);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Frame()
    {
      int frameSize = 100;

      using (MagickImage image = new MagickImage(Files.MagickNETIconPNG))
      {
        int expectedWidth = frameSize + image.Width + frameSize;
        int expectedHeight = frameSize + image.Height + frameSize;

        image.Frame(frameSize, frameSize);
        Assert.AreEqual(expectedWidth, image.Width);
        Assert.AreEqual(expectedHeight, image.Height);
      }

      using (MagickImage image = new MagickImage(Files.MagickNETIconPNG))
      {
        int expectedWidth = frameSize + image.Width + frameSize;
        int expectedHeight = frameSize + image.Height + frameSize;

        image.Frame(frameSize, frameSize, 6, 6);
        Assert.AreEqual(expectedWidth, image.Width);
        Assert.AreEqual(expectedHeight, image.Height);
      }

      ExceptionAssert.Throws<MagickOptionErrorException>(delegate ()
      {
        using (MagickImage image = new MagickImage(Files.MagickNETIconPNG))
        {
          image.Frame(6, 6, frameSize, frameSize);
        }
      });
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Gamma()
    {
      MagickImage first = new MagickImage(Files.InvitationTif);
      first.GammaCorrect(2.0);

      MagickImage second = new MagickImage(Files.InvitationTif);
      second.GammaCorrect(2.0, 1.0, 0.5);

      Assert.AreNotEqual(first, second);

      first.Dispose();
      second.Dispose();
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_HasClippingPath()
    {
      using (MagickImage noPath = new MagickImage(Files.MagickNETIconPNG))
      {
        Assert.IsFalse(noPath.HasClippingPath);
      }

      using (MagickImage hasPath = new MagickImage(Files.InvitationTif))
      {
        Assert.IsTrue(hasPath.HasClippingPath);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Histogram()
    {
      MagickImage image = new MagickImage(Files.RedPNG);
      Dictionary<MagickColor, int> histogram = image.Histogram();

      Assert.IsNotNull(histogram);
      Assert.AreEqual(3, histogram.Count);

      MagickColor red = new MagickColor(Quantum.Max, 0, 0);
      MagickColor alphaRed = new MagickColor(Quantum.Max, 0, 0, 0);
      MagickColor halfAlphaRed = new MagickColor("#FF000080");

      foreach (MagickColor color in histogram.Keys)
      {
        if (color == red)
          Assert.AreEqual(50000, histogram[color]);
        else if (color == alphaRed)
          Assert.AreEqual(30000, histogram[color]);
        else if (color == halfAlphaRed)
          Assert.AreEqual(40000, histogram[color]);
        else
          Assert.Fail("Invalid color: " + color.ToString());
      }

      image.Dispose();
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_IComparable()
    {
      MagickImage first = new MagickImage(Color.Red, 10, 5);

      Assert.AreEqual(0, first.CompareTo(first));
      Assert.AreEqual(1, first.CompareTo(null));
      Assert.IsFalse(first < null);
      Assert.IsFalse(first <= null);
      Assert.IsTrue(first > null);
      Assert.IsTrue(first >= null);
      Assert.IsTrue(null < first);
      Assert.IsTrue(null <= first);
      Assert.IsFalse(null > first);
      Assert.IsFalse(null >= first);

      MagickImage second = new MagickImage(Color.Green, 5, 5);

      Assert.AreEqual(1, first.CompareTo(second));
      Assert.IsFalse(first < second);
      Assert.IsFalse(first <= second);
      Assert.IsTrue(first > second);
      Assert.IsTrue(first >= second);

      second = new MagickImage(Color.Red, 5, 10);

      Assert.AreEqual(0, first.CompareTo(second));
      Assert.IsFalse(first == second);
      Assert.IsFalse(first < second);
      Assert.IsTrue(first <= second);
      Assert.IsFalse(first > second);
      Assert.IsTrue(first >= second);

      first.Dispose();
      second.Dispose();
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_IEquatable()
    {
      MagickImage first = new MagickImage(Color.Red, 10, 10);

      Assert.IsFalse(first == null);
      Assert.IsFalse(first.Equals(null));
      Assert.IsTrue(first.Equals(first));
      Assert.IsTrue(first.Equals((object)first));

      MagickImage second = new MagickImage(Color.Red, 10, 10);

      Assert.IsTrue(first == second);
      Assert.IsTrue(first.Equals(second));
      Assert.IsTrue(first.Equals((object)second));

      second = new MagickImage(Color.Green, 10, 10);

      Assert.IsTrue(first != second);
      Assert.IsFalse(first.Equals(second));

      first.Dispose();
      second.Dispose();

      first = null;
      Assert.IsTrue(first == null);
      Assert.IsFalse(first != null);
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_IsOpaque()
    {
      using (MagickImage image = new MagickImage(Files.MagickNETIconPNG))
      {
        Assert.IsFalse(image.IsOpaque);
        image.ColorAlpha(Color.Purple);
        Assert.IsTrue(image.IsOpaque);
      }

      using (MagickImage image = new MagickImage(Files.Builtin.Logo))
      {
        Assert.IsTrue(image.IsOpaque);
        image.Opaque(Color.White, MagickColor.Transparent);
        Assert.IsFalse(image.IsOpaque);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Level()
    {
      using (MagickImage first = new MagickImage(Files.MagickNETIconPNG))
      {
        first.Level(new Percentage(50.0), new Percentage(10.0));

        using (MagickImage second = new MagickImage(Files.MagickNETIconPNG))
        {
          Assert.AreNotEqual(first, second);
          Assert.AreNotEqual(first.Signature, second.Signature);

          second.Level((QuantumType)(Quantum.Max * 0.5), (QuantumType)(Quantum.Max * 0.1));

          Assert.AreEqual(0.0, first.Compare(second, ErrorMetric.RootMeanSquared));

          Assert.AreEqual(first, second);
          Assert.AreEqual(first.Signature, second.Signature);
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_LiquidRescale()
    {
      using (MagickImage image = new MagickImage(Files.MagickNETIconPNG))
      {
        MagickGeometry geometry = new MagickGeometry(128, 64);
        geometry.IgnoreAspectRatio = true;

        image.LiquidRescale(geometry);
        Assert.AreEqual(128, image.Width);
        Assert.AreEqual(64, image.Height);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Opaque()
    {
      using (MagickImage image = new MagickImage(Color.Red, 10, 10))
      {
        Test_Pixel_Equal(image, 0, 0, Color.Red);

        image.Opaque(Color.Red, Color.Yellow);
        Test_Pixel_Equal(image, 0, 0, Color.Yellow);

        image.InverseOpaque(Color.Yellow, Color.Red);
        Test_Pixel_Equal(image, 0, 0, Color.Yellow);

        image.InverseOpaque(Color.Red, Color.Red);
        Test_Pixel_Equal(image, 0, 0, Color.Red);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Ping()
    {
      using (MagickImage image = new MagickImage())
      {
        image.Ping(Files.FujiFilmFinePixS1ProJPG);

        ExceptionAssert.Throws<InvalidOperationException>(delegate ()
        {
          image.GetReadOnlyPixels();
        });

        ImageProfile profile = image.Get8BimProfile();
        Assert.IsNotNull(profile);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Profile()
    {
      using (MagickImage image = new MagickImage(Files.FujiFilmFinePixS1ProJPG))
      {
        ImageProfile profile = image.GetIptcProfile();
        Assert.IsNotNull(profile);
        image.RemoveProfile(profile.Name);
        profile = image.GetIptcProfile();
        Assert.IsNull(profile);

        using (MemoryStream memStream = new MemoryStream())
        {
          image.Write(memStream);
          memStream.Position = 0;

          using (MagickImage newImage = new MagickImage(memStream))
          {
            profile = newImage.GetIptcProfile();
            Assert.IsNull(profile);
          }
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_ProfileNames()
    {
      using (MagickImage image = new MagickImage(Files.FujiFilmFinePixS1ProJPG))
      {
        IEnumerable<string> names = image.ProfileNames;
        Assert.IsNotNull(names);
        Assert.AreEqual(5, names.Count());
        Assert.AreEqual("8bim,exif,icc,iptc,xmp", string.Join(",", (from name in names
                                                                    orderby name
                                                                    select name).ToArray()));
      }

      using (MagickImage image = new MagickImage(Files.RedPNG))
      {
        IEnumerable<string> names = image.ProfileNames;
        Assert.IsNotNull(names);
        Assert.AreEqual(0, names.Count());
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Quantize()
    {
      QuantizeSettings settings = new QuantizeSettings();
      settings.Colors = 8;

      Assert.AreEqual(DitherMethod.Riemersma, settings.DitherMethod);
      settings.DitherMethod = null;
      Assert.AreEqual(null, settings.DitherMethod);
      settings.DitherMethod = DitherMethod.No;
      Assert.AreEqual(DitherMethod.No, settings.DitherMethod);

      using (MagickImage image = new MagickImage(Files.FujiFilmFinePixS1ProJPG))
      {
        image.Quantize(settings);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Read()
    {
      MagickImage image = new MagickImage();

      ExceptionAssert.Throws<ArgumentException>(delegate ()
      {
        image.Read(new byte[0]);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        image.Read((byte[])null);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        image.Read((Bitmap)null);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        image.Read((Stream)null);
      });

      ExceptionAssert.Throws<ArgumentNullException>(delegate ()
      {
        image.Read((string)null);
      });

      ExceptionAssert.Throws<ArgumentException>(delegate ()
      {
        image.Read(Files.Missing);
      });

      ExceptionAssert.Throws<ArgumentException>(delegate ()
      {
        image.Read("png:" + Files.Missing);
      });

      image.Read(File.ReadAllBytes(Files.SnakewarePNG));

      using (Bitmap bitmap = new Bitmap(Files.SnakewarePNG))
      {
        image.Read(bitmap);
        Assert.AreEqual(MagickFormat.Png, image.Format);
      }

      using (Bitmap bitmap = new Bitmap(100, 100, PixelFormat.Format24bppRgb))
      {
        image.Read(bitmap);
        Assert.AreEqual(MagickFormat.Bmp3, image.Format);
      }

      using (FileStream fs = File.OpenRead(Files.SnakewarePNG))
      {
        image.Read(fs);
      }

      image.Read(Files.SnakewarePNG);

      image.Read(Files.Builtin.Rose);

      image.Read(Files.RoseSparkleGIF);
      Assert.AreEqual("RöseSparkle.gif", Path.GetFileName(image.FileName));

      image.Read("png:" + Files.SnakewarePNG);

      MagickColor red = new MagickColor("red");

      image.Read(red, 50, 50);
      Assert.AreEqual(50, image.Width);
      Assert.AreEqual(50, image.Height);
      Test_Pixel_Equal(image, 10, 10, red);

      image.Read("xc:red", 50, 50);
      Assert.AreEqual(50, image.Width);
      Assert.AreEqual(50, image.Height);
      Test_Pixel_Equal(image, 5, 5, red);

      image.Dispose();

      ExceptionAssert.Throws<ObjectDisposedException>(delegate ()
      {
        image.Read(Files.Builtin.Logo);
      });
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Resize()
    {
      using (MagickImage image = new MagickImage())
      {
        image.Read(Files.MagickNETIconPNG);
        image.Resize(new MagickGeometry(64, 64));
        Assert.AreEqual(64, image.Width);
        Assert.AreEqual(64, image.Height);

        image.Read(Files.MagickNETIconPNG);
        image.Resize((Percentage)200);
        Assert.AreEqual(256, image.Width);
        Assert.AreEqual(256, image.Height);

        image.Read(Files.MagickNETIconPNG);
        image.Resize(32, 32);
        Assert.AreEqual(32, image.Width);
        Assert.AreEqual(32, image.Height);

        image.Read(Files.MagickNETIconPNG);
        image.Resize(new MagickGeometry("5x10!"));
        Assert.AreEqual(5, image.Width);
        Assert.AreEqual(10, image.Height);

        image.Read(Files.MagickNETIconPNG);
        image.Resize(new MagickGeometry("32x32<"));
        Assert.AreEqual(128, image.Width);
        Assert.AreEqual(128, image.Height);

        image.Read(Files.MagickNETIconPNG);
        image.Resize(new MagickGeometry("256x256<"));
        Assert.AreEqual(256, image.Width);
        Assert.AreEqual(256, image.Height);

        image.Read(Files.MagickNETIconPNG);
        image.Resize(new MagickGeometry("32x32>"));
        Assert.AreEqual(32, image.Width);
        Assert.AreEqual(32, image.Height);

        image.Read(Files.MagickNETIconPNG);
        image.Resize(new MagickGeometry("256x256>"));
        Assert.AreEqual(128, image.Width);
        Assert.AreEqual(128, image.Height);

        image.Read(Files.SnakewarePNG);
        image.Resize(new MagickGeometry("4096@"));
        Assert.IsTrue((image.Width * image.Height) < 4096);

        Percentage percentage = new Percentage(-0.5);
        ExceptionAssert.Throws<ArgumentException>(delegate ()
        {
          image.Resize(percentage);
        });
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Resample()
    {
      using (MagickImage image = new MagickImage("xc:red", 100, 100))
      {
        image.Resample(new PointD(300));

        Assert.AreEqual(300, image.ResolutionX);
        Assert.AreEqual(300, image.ResolutionY);
        Assert.AreNotEqual(100, image.Width);
        Assert.AreNotEqual(100, image.Height);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Resolution()
    {
      using (MagickImage image = new MagickImage(Files.EightBimTIF))
      {
        Assert.AreEqual(Resolution.PixelsPerInch, image.ResolutionUnits);
        Assert.AreEqual(72, image.ResolutionX);
        Assert.AreEqual(72, image.ResolutionY);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Scale()
    {
      using (MagickImage image = new MagickImage(Files.CirclePNG))
      {
        MagickColor color = Color.FromArgb(159, 255, 255, 255);
        using (PixelCollection pixels = image.GetReadOnlyPixels())
        {
          ColorAssert.AreEqual(color, pixels.GetPixel(image.Width / 2, image.Height / 2).ToColor());
        }

        image.Scale((Percentage)400);

        using (PixelCollection pixels = image.GetReadOnlyPixels())
        {
          ColorAssert.AreEqual(color, pixels.GetPixel(image.Width / 2, image.Height / 2).ToColor());
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Separate()
    {
      using (MagickImage rose = new MagickImage(Files.Builtin.Rose))
      {
        int i = 0;
        foreach (MagickImage image in rose.Separate())
        {
          i++;
          image.Dispose();
        }

        Assert.AreEqual(3, i);

        i = 0;
        foreach (MagickImage image in rose.Separate(Channels.Red | Channels.Green))
        {
          i++;
          image.Dispose();
        }

        Assert.AreEqual(2, i);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Separate_Composite()
    {
      using (MagickImage logo = new MagickImage(Files.Builtin.Logo))
      {
        using (MagickImage blue = logo.Separate(Channels.Blue).First())
        {
          Test_Separate_Composite(blue, ColorSpace.Gray, 146);

          using (MagickImage green = logo.Separate(Channels.Green).First())
          {
            Test_Separate_Composite(green, ColorSpace.Gray, 62);

            blue.Composite(green, CompositeOperator.Multiply);

            Test_Separate_Composite(blue, ColorSpace.sRGB, 35);
          }
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Shadow()
    {
      using (MagickImage image = new MagickImage())
      {
        image.BackgroundColor = MagickColor.Transparent;
        image.FontPointsize = 60;
        image.Read("label:Magick.NET");

        int width = image.Width;
        int height = image.Height;

        image.Shadow(2, 2, 1, new ImageMagick.Percentage(50), Color.Red);

        Assert.AreEqual(width + 4, image.Width);
        Assert.AreEqual(height + 4, image.Height);

        using (PixelCollection pixels = image.GetReadOnlyPixels())
        {
          Pixel pixel = pixels.GetPixel(90, 9);
          Assert.AreEqual(0, pixel.ToColor().A);
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_SparseColors()
    {
      MagickReadSettings settings = new MagickReadSettings();
      settings.Width = 600;
      settings.Height = 60;

      using (MagickImage image = new MagickImage("xc:", settings))
      {
        ExceptionAssert.Throws<ArgumentNullException>(delegate ()
        {
          image.SparseColor(Channels.Red, SparseColorMethod.Barycentric, null);
        });

        List<SparseColorArg> args = new List<SparseColorArg>();

        ExceptionAssert.Throws<ArgumentException>(delegate ()
        {
          image.SparseColor(Channels.Blue, SparseColorMethod.Barycentric, args);
        });

        using (PixelCollection pixels = image.GetReadOnlyPixels())
        {
          ColorAssert.AreEqual(pixels.GetPixel(0, 0), pixels.GetPixel(599, 59));
        }

        ExceptionAssert.Throws<ArgumentNullException>(delegate ()
        {
          args.Add(new SparseColorArg(0, 0, null));
        });

        args.Add(new SparseColorArg(0, 0, new MagickColor("skyblue")));
        args.Add(new SparseColorArg(-600, 60, new MagickColor("skyblue")));
        args.Add(new SparseColorArg(600, 60, new MagickColor("black")));

        image.SparseColor(SparseColorMethod.Barycentric, args);

        using (PixelCollection pixels = image.GetReadOnlyPixels())
        {
          ColorAssert.AreNotEqual(pixels.GetPixel(0, 0), pixels.GetPixel(599, 59));
        }

        ExceptionAssert.Throws<ArgumentException>(delegate ()
        {
          image.SparseColor(Channels.Black, SparseColorMethod.Barycentric, args);
        });
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_SubImageSearch()
    {
      using (MagickImageCollection images = new MagickImageCollection())
      {
        images.Add(new MagickImage(Color.Green, 2, 2));
        images.Add(new MagickImage(Color.Red, 2, 2));

        using (MagickImage combined = images.AppendHorizontally())
        {
          using (MagickSearchResult searchResult = combined.SubImageSearch(new MagickImage(Color.Red, 0, 0), ErrorMetric.RootMeanSquared))
          {
            Assert.IsNotNull(searchResult);
            Assert.IsNotNull(searchResult.SimilarityImage);
            Assert.IsNotNull(searchResult.BestMatch);
            Assert.AreEqual(0.0, searchResult.SimilarityMetric);
            Assert.AreEqual(2, searchResult.BestMatch.X);
            Assert.AreEqual(0, searchResult.BestMatch.Y);
          }
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_ToBitmap()
    {
      using (MagickImage image = new MagickImage(Color.Red, 10, 10))
      {
        ExceptionAssert.Throws<NotSupportedException>(delegate ()
        {
          image.ToBitmap(ImageFormat.Exif);
        });

        Bitmap bitmap = image.ToBitmap();
        Assert.AreEqual(ImageFormat.MemoryBmp, bitmap.RawFormat);
        ColorAssert.AreEqual(Color.Red, bitmap.GetPixel(0, 0));
        ColorAssert.AreEqual(Color.Red, bitmap.GetPixel(5, 5));
        ColorAssert.AreEqual(Color.Red, bitmap.GetPixel(9, 9));
        bitmap.Dispose();

        Test_ToBitmap(image, ImageFormat.Bmp);
        Test_ToBitmap(image, ImageFormat.Gif);
        Test_ToBitmap(image, ImageFormat.Icon);
        Test_ToBitmap(image, ImageFormat.Jpeg);
        Test_ToBitmap(image, ImageFormat.Png);
        Test_ToBitmap(image, ImageFormat.Tiff);
      }

      using (MagickImage image = new MagickImage(new MagickColor(0, Quantum.Max, Quantum.Max, 0), 10, 10))
      {
        Bitmap bitmap = image.ToBitmap();
        Assert.AreEqual(ImageFormat.MemoryBmp, bitmap.RawFormat);
        Color color = Color.FromArgb(0, 0, 255, 255);
        ColorAssert.AreEqual(color, bitmap.GetPixel(0, 0));
        ColorAssert.AreEqual(color, bitmap.GetPixel(5, 5));
        ColorAssert.AreEqual(color, bitmap.GetPixel(9, 9));
        bitmap.Dispose();
      }
    }
#if !(NET20)
    [TestMethod, TestCategory(_Category)]
    public void Test_ToBitmapSource()
    {
      byte[] pixels = new byte[600];

      using (MagickImage image = new MagickImage(Color.Red, 10, 10))
      {
        BitmapSource bitmap = image.ToBitmapSource();
        bitmap.CopyPixels(pixels, 60, 0);

        Assert.AreEqual(255, pixels[0]);
        Assert.AreEqual(0, pixels[1]);
        Assert.AreEqual(0, pixels[2]);

        image.ColorSpace = ColorSpace.CMYK;

        bitmap = image.ToBitmapSource();
        bitmap.CopyPixels(pixels, 60, 0);

        Assert.AreEqual(0, pixels[0]);
        Assert.AreEqual(255, pixels[1]);
        Assert.AreEqual(255, pixels[2]);
        Assert.AreEqual(0, pixels[3]);

        image.AddProfile(ColorProfile.USWebCoatedSWOP);
        image.AddProfile(ColorProfile.SRGB);

        bitmap = image.ToBitmapSource();
        bitmap.CopyPixels(pixels, 60, 0);

        Assert.AreEqual(237, pixels[0]);
        Assert.AreEqual(28, pixels[1]);
        Assert.AreEqual(36, pixels[2]);
      }
    }
#endif

    [TestMethod, TestCategory(_Category)]
    public void Test_Threshold()
    {
      using (MagickImage image = new MagickImage(Files.ImageMagickJPG))
      {
        using (MemoryStream memStream = new MemoryStream())
        {
          image.Threshold(new Percentage(80));
          image.CompressionMethod = CompressionMethod.Group4;
          image.Format = MagickFormat.Pdf;
          image.Write(memStream);
        }
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Thumbnail()
    {
      using (MagickImage image = new MagickImage(Files.SnakewarePNG))
      {
        image.Thumbnail(100, 100);
        Assert.AreEqual(100, image.Width);
        Assert.AreEqual(23, image.Height);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Transparent()
    {
      MagickColor red = new MagickColor("red");
      MagickColor transparentRed = new MagickColor("red");
      transparentRed.A = 0;

      using (MagickImage image = new MagickImage(Files.RedPNG))
      {
        Test_Pixel_Equal(image, 0, 0, red);

        image.Transparent(red);

        Test_Pixel_Equal(image, 0, 0, transparentRed);
        Test_Pixel_NotEqual(image, image.Width - 1, 0, transparentRed);
      }

      using (MagickImage image = new MagickImage(Files.RedPNG))
      {
        Test_Pixel_Equal(image, 0, 0, red);

        image.InverseTransparent(red);

        Test_Pixel_NotEqual(image, 0, 0, transparentRed);
        Test_Pixel_Equal(image, image.Width - 1, 0, transparentRed);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_TransformColorSpace()
    {
      using (MagickImage image = new MagickImage(Files.MagickNETIconPNG))
      {
        Assert.AreEqual(ColorSpace.sRGB, image.ColorSpace);

        image.TransformColorSpace(ColorProfile.USWebCoatedSWOP, ColorProfile.USWebCoatedSWOP);
        Assert.AreEqual(ColorSpace.sRGB, image.ColorSpace);

        image.TransformColorSpace(ColorProfile.SRGB, ColorProfile.USWebCoatedSWOP);
        Assert.AreEqual(ColorSpace.CMYK, image.ColorSpace);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Warning()
    {
      int count = 0;
      EventHandler<WarningEventArgs> warningDelegate = delegate (object sender, WarningEventArgs arguments)
      {
        Assert.IsNotNull(sender);
        Assert.IsNotNull(arguments);
        Assert.IsNotNull(arguments.Message);
        Assert.IsNotNull(arguments.Exception);
        Assert.AreNotEqual("", arguments.Message);

        count++;
      };

      using (MagickImage image = new MagickImage())
      {
        image.Warning += warningDelegate;
        image.Read(Files.EightBimTIF);

        Assert.AreNotEqual(0, count);

        int expectedCount = count;
        image.Warning -= warningDelegate;
        image.Read(Files.EightBimTIF);

        Assert.AreEqual(expectedCount, count);
      }
    }

    [TestMethod, TestCategory(_Category)]
    public void Test_Write()
    {
      using (MagickImage image = new MagickImage(Files.SnakewarePNG))
      {
        using (MemoryStream memStream = new MemoryStream())
        {
          image.Write(memStream);

          Assert.AreEqual(image.FileSize, memStream.Length);
        }
      }
    }
  }
}
