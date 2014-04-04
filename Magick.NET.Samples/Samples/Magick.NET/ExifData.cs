﻿//=================================================================================================
// Copyright 2013-2014 Dirk Lemstra <https://magick.codeplex.com/>
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
using ImageMagick;

namespace RootNamespace.Samples.MagickNET
{
	public static class ExifDataSamples
	{
		public static void ReadExifData()
		{
			using (MagickImage image = new MagickImage(SampleFiles.FujiFilmFinePixS1ProJpg))
			{
				ExifProfile profile = image.GetExifProfile();
				foreach (ExifValue value in profile.Values)
				{
					Console.WriteLine("{0}({1}): {2}", value.Tag, value.DataType, value.ToString());
				}
			}
		}

		public static void CreateThumbnailFromExifData()
		{
			using (MagickImage image = new MagickImage(SampleFiles.FujiFilmFinePixS1ProJpg))
			{
				ExifProfile profile = image.GetExifProfile();
				using (MagickImage thumbnail = profile.CreateThumbnail())
				{
					thumbnail.Write(SampleFiles.OutputDirectory + "FujiFilmFinePixS1Pro.thumb.jpg");
				}
			}
		}
	}
}