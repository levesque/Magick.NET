//=================================================================================================
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

using ImageMagick.Drawables;

namespace ImageMagick
{
  ///<summary>
  /// Encapsulation of the DrawableTextInterlineSpacing object.
  ///</summary>
  public sealed class DrawableTextInterlineSpacing : IDrawableTextInterlineSpacing
  {
    ///<summary>
    /// Creates a new DrawableTextInterlineSpacing instance.
    ///</summary>
    ///<param name="spacing">Spacing to use.</param>
    public DrawableTextInterlineSpacing(double spacing)
    {
      Spacing = spacing;
    }

    ///<summary>
    /// Spacing to use.
    ///</summary>
    public double Spacing
    {
      get;
      set;
    }
  }
}