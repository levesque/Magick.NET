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
using System.Drawing;

#if Q8
using QuantumType = System.Byte;
#elif Q16
using QuantumType = System.UInt16;
#elif Q16HDRI
	using QuantumType = System.Single;
#else
#error Not implemented!
#endif

namespace ImageMagick
{
  ///<summary>
  /// Class that represents a RGB color.
  ///</summary>
  public sealed class ColorRGB : ColorBase
  {
    private ColorRGB(MagickColor color)
      : base(color)
    {
    }

    ///<summary>
    /// Initializes a new instance of the ColorRGB class.
    ///</summary>
    ///<param name="red">Red component value of this color.</param>
    ///<param name="green">Green component value of this color.</param>
    ///<param name="blue">Blue component value of this color.</param>
#if Q16
    [CLSCompliant(false)]
#endif
    public ColorRGB(QuantumType red, QuantumType green, QuantumType blue)
      : base(new MagickColor(red, green, blue))
    {
    }

    ///<summary>
    /// Initializes a new instance of the ColorRGB class using the specified color.
    ///</summary>
    ///<param name="color">The color to use.</param>
    public ColorRGB(Color color)
      : base(new MagickColor(color))
    {
    }

    ///<summary>
    /// Blue component value of this color.
    ///</summary>
#if Q16
    [CLSCompliant(false)]
#endif
    public QuantumType B
    {
      get
      {
        return Value.B;
      }
      set
      {
        Value.B = value;
      }
    }

    ///<summary>
    /// Green component value of this color.
    ///</summary>
#if Q16
    [CLSCompliant(false)]
#endif
    public QuantumType G
    {
      get
      {
        return Value.G;
      }
      set
      {
        Value.G = value;
      }
    }

    ///<summary>
    /// Red component value of this color.
    ///</summary>
#if Q16
    [CLSCompliant(false)]
#endif
    public QuantumType R
    {
      get
      {
        return Value.R;
      }
      set
      {
        Value.R = value;
      }
    }

    ///<summary>
    /// Converts the specified MagickColor to an instance of this type.
    ///</summary>
    public static implicit operator ColorRGB(MagickColor color)
    {
      return FromMagickColor(color);
    }

    ///<summary>
    /// Converts the specified MagickColor to an instance of this type.
    ///</summary>
    public static ColorRGB FromMagickColor(MagickColor color)
    {
      if (color == null)
        return null;

      return new ColorRGB(color);
    }
  }
}