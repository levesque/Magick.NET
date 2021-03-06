// This file is used by Code Analysis to maintain 
// CA_GLOBAL_SUPPRESS_MESSAGE macros that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "ImageMagick.Wrapper");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "ImageMagick.Wrapper.Drawables.#CreateDrawable(ImageMagick.IDrawable,System.Type)");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "member", Target = "ImageMagick.Wrapper.Drawables.#CreateDrawable(ImageMagick.IDrawable,System.Type)");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Scope = "type", Target = "ImageMagick.Wrapper.MagickImage");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "FrameCount", Scope = "member", Target = "ImageMagick.Wrapper.MagickImage.#CheckSettings(ImageMagick.MagickReadSettings)");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "MagickImage", Scope = "member", Target = "ImageMagick.Wrapper.MagickImage.#CheckSettings(ImageMagick.MagickReadSettings)");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PixelStorage", Scope = "member", Target = "ImageMagick.Wrapper.MagickReader.#Read(std.vector<Magick::Image,std::allocator<Magick::Image> >*,System.Byte[],ImageMagick.Wrapper.MagickReaderSettings)");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "PixelStorage", Scope = "member", Target = "ImageMagick.Wrapper.MagickReader.#Read(std.vector<Magick::Image,std::allocator<Magick::Image> >*,System.String,ImageMagick.Wrapper.MagickReaderSettings)");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "ImageMagick.Wrapper.MagickImageInfo.#HandleException(ImageMagick.MagickException)");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "ImageMagick.Wrapper.Throw.#FormatMessage(System.String,System.Object[])");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ImageMagick.Wrapper.MagickImage.#Draw(System.Collections.Generic.IEnumerable`1<ImageMagick.IDrawable>)");
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ImageMagick.Wrapper.MagickImage.#Statistics()");
