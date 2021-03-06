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

using System.CodeDom.Compiler;
using System.Linq;
using System.Reflection;

namespace Magick.NET.FileGenerator
{
  internal abstract class ConstructorCodeGenerator : CodeGenerator
  {
    private ConstructorInfo[] _Constructors;

    private ConstructorInfo[] Constructors
    {
      get
      {
        if (_Constructors == null)
          _Constructors = Types.GetConstructors(ClassName).ToArray();

        return _Constructors;
      }
    }

    protected string TypeName
    {
      get
      {
        return GetName(Constructors[0]);
      }
    }

    protected abstract string ClassName
    {
      get;
    }

    protected virtual bool WriteEnumerable
    {
      get
      {
        return true;
      }
    }

    protected override void WriteCode(IndentedTextWriter writer)
    {
      writer.Write("private ");
      writer.Write(TypeName);
      writer.Write(" Create");
      writer.Write(ClassName);
      writer.WriteLine("(XmlElement element)");
      WriteStartColon(writer);
      WriteMethod(writer, Constructors);
      WriteEndColon(writer);

      if (!WriteEnumerable)
        return;

      WriteSeparator(writer);
      writer.Write("private ");
      writer.Write("Collection<");
      writer.Write(TypeName);
      writer.Write("> Create");
      writer.Write(ClassName);
      writer.WriteLine("s(XmlElement element)");
      WriteStartColon(writer);
      writer.Write("Collection<");
      writer.Write(TypeName);
      writer.Write("> collection = new Collection<");
      writer.Write(TypeName);
      writer.WriteLine(">();");
      writer.WriteLine("foreach (XmlElement elem in element.SelectNodes(\"*\"))");
      WriteStartColon(writer);
      writer.Write("collection.Add(Create");
      writer.Write(TypeName);
      writer.WriteLine("(elem));");
      WriteEndColon(writer);
      writer.WriteLine("return collection;");
      WriteEndColon(writer);
    }

    public override string Name
    {
      get
      {
        return ClassName;
      }
    }
  }
}
