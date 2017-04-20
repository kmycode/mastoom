using System;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
using CarouselView.FormsPlugin.iOS;
using Foundation;
using UIKit;

namespace Mastoom.iOS
=======

using Foundation;
using UIKit;

namespace MastoomXF.iOS
>>>>>>> とりあえずXamarin.Formsプロジェクト(PCL)群を追加
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
<<<<<<< HEAD
            CarouselViewRenderer.Init();
=======

>>>>>>> とりあえずXamarin.Formsプロジェクト(PCL)群を追加
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
