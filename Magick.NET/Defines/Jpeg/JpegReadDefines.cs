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

using System.Collections.Generic;
using ImageMagick.Defines;

namespace ImageMagick
{
  ///<summary>
  /// Class for defines that are used when a jpeg image is read.
  ///</summary>
  public sealed class JpegReadDefines : DefineCreator, IReadDefines
  {
    ///<summary>
    /// Initializes a new instance of the JpegReadDefines class.
    ///</summary>
    public JpegReadDefines()
      : base(MagickFormat.Jpeg)
    {
    }

    ///<summary>
    /// Enables or disables block smoothing (jpeg:block-smoothing).
    ///</summary>
    public bool? BlockSmoothing
    {
      get;
      set;
    }

    ///<summary>
    /// Specifies the size desired number of colors (jpeg:colors).
    ///</summary>
    public int? Colors
    {
      get;
      set;
    }

    ///<summary>
    /// Specifies the dtc method that will be used (jpeg:dct-method).
    ///</summary>
    public DctMethod? DctMethod
    {
      get;
      set;
    }

    ///<summary>
    /// Enables or disables fancy upsampling (jpeg:fancy-upsampling).
    ///</summary>
    public bool? FancyUpsampling
    {
      get;
      set;
    }

    ///<summary>
    /// Specifies the size the scale the image to (jpeg:size). The output image won't be exactly
    /// the specified size. More information can be found here: http://jpegclub.org/djpeg/.
    ///</summary>
    public MagickGeometry Size
    {
      get;
      set;
    }

    ///<summary>
    /// Specifies the profile that should be skipped when the image is read (profile:skip).
    ///</summary>
    public ProfileTypes? SkipProfiles
    {
      get;
      set;
    }

    ///<summary>
    /// The defines that should be set as an define on an image
    ///</summary>
    public override IEnumerable<IDefine> Defines
    {
      get
      {
        if (BlockSmoothing.HasValue)
          yield return CreateDefine("block-smoothing", BlockSmoothing.Value);

        if (Colors.HasValue)
          yield return CreateDefine("colors", Colors.Value);

        if (DctMethod.HasValue)
          yield return CreateDefine("dct-method", DctMethod.Value);

        if (FancyUpsampling.HasValue)
          yield return CreateDefine("fancy-upsampling", FancyUpsampling.Value);

        if (Size != null)
          yield return CreateDefine("size", Size);

        if (SkipProfiles.HasValue)
        {
          string value = EnumHelper.ConvertFlags(SkipProfiles.Value);

          if (!string.IsNullOrEmpty(value))
            yield return new MagickDefine(MagickFormat.Unknown, "profile:skip", value);
        }
      }
    }
  }
}