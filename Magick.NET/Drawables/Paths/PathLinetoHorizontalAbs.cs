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

using ImageMagick.Drawables.Paths;

namespace ImageMagick
{
  ///<summary>
  /// Encapsulation of the PathLinetoHorizontalAbs object.
  ///</summary>
  public sealed class PathLinetoHorizontalAbs : IPathLinetoHorizontalAbs
  {
    ///<summary>
    /// Initializes a new instance of the PathLinetoHorizontalAbs class.
    ///</summary>
    ///<param name="x">The X coordinate.</param>
    public PathLinetoHorizontalAbs(double x)
    {
      X = x;
    }

    /// <summary>
    /// The X coordinate.
    /// </summary>
    public double X
    {
      get;
      set;
    }
  }
}