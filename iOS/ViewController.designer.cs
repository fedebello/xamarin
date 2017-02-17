// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SmartFairMVP.iOS
{
    [Register ("ViewController")]
    partial class ViewController
    {
        [Outlet]
        UIKit.UIButton Button { get; set; }


        [Outlet]
        UIKit.UILabel PositionLabel { get; set; }


        [Outlet]
        UIKit.UIButton RandomButton { get; set; }


        [Outlet]
        UIKit.UIButton ResetButton { get; set; }

        [Action ("ResetButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ResetButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (Button != null) {
                Button.Dispose ();
                Button = null;
            }

            if (PositionLabel != null) {
                PositionLabel.Dispose ();
                PositionLabel = null;
            }

            if (RandomButton != null) {
                RandomButton.Dispose ();
                RandomButton = null;
            }

            if (ResetButton != null) {
                ResetButton.Dispose ();
                ResetButton = null;
            }
        }
    }
}