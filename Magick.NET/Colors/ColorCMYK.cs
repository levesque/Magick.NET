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
  /// Class that represents a CMYK color.
  ///</summary>
  public sealed class ColorCMYK : ColorBase
  {
    private ColorCMYK(MagickColor color)
      : base(color)
    {
    }

    ///<summary>
    /// Initializes a new instance of the ColorCMYK class.
    ///</summary>
    ///<param name="cyan">Cyan component value of this color.</param>
    ///<param name="magenta">Magenta component value of this color.</param>
    ///<param name="yellow">Yellow component value of this color.</param>
    ///<param name="key">Key (black) component value of this color.</param>
#if Q16
    [CLSCompliant(false)]
#endif
    public ColorCMYK(QuantumType cyan, QuantumType magenta, QuantumType yellow, QuantumType key)
      : base(new MagickColor(cyan, magenta, yellow, key, Quantum.Max))
    {
    }

    ///<summary>
    /// Initializes a new instance of the ColorCMYK class.
    ///</summary>
    ///<param name="cyan">Cyan component value of this color.</param>
    ///<param name="magenta">Magenta component value of this color.</param>
    ///<param name="yellow">Yellow component value of this color.</param>
    ///<param name="key">Key (black) component value of this color.</param>
    ///<param name="alpha">Key (black) component value of this color.</param>
#if Q16
    [CLSCompliant(false)]
#endif
    public ColorCMYK(QuantumType cyan, QuantumType magenta, QuantumType yellow, QuantumType key, QuantumType alpha)
      : base(new MagickColor(cyan, magenta, yellow, key, alpha))
    {
    }

    ///<summary>
    /// Alpha component value of this color.
    ///</summary>
#if Q16
    [CLSCompliant(false)]
#endif
    public QuantumType A
    {
      get
      {
        return Value.A;
      }
      set
      {
        Value.A = value;
      }
    }

    ///<summary>
    /// Cyan component value of this color.
    ///</summary>
#if Q16
    [CLSCompliant(false)]
#endif
    public QuantumType C
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
    /// Key (black) component value of this color.
    ///</summary>
#if Q16
    [CLSCompliant(false)]
#endif
    public QuantumType K
    {
      get
      {
        return Value.K;
      }
      set
      {
        Value.K = value;
      }
    }

    ///<summary>
    /// Magenta component value of this color.
    ///</summary>
#if Q16
    [CLSCompliant(false)]
#endif
    public QuantumType M
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
    /// Yellow component value of this color.
    ///</summary>
#if Q16
    [CLSCompliant(false)]
#endif
    public QuantumType Y
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
    /// Converts the specified MagickColor to an instance of this type.
    ///</summary>
    public static implicit operator ColorCMYK(MagickColor color)
    {
      return FromMagickColor(color);
    }

    ///<summary>
    /// Converts the specified MagickColor to an instance of this type.
    ///</summary>
    public static ColorCMYK FromMagickColor(MagickColor color)
    {

      if (color == null)
        return null;

      return new ColorCMYK(color);
    }
  }
}