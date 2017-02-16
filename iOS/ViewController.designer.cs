//		
// This file has been generated automatically by MonoDevelop to store outlets and		
// actions made in the Xcode designer. If it is removed, they will be lost.		
// Manual changes to this file may not be handled correctly.		
//		
using Foundation;

namespace SmartFairMVP.iOS
{
	[Register("ViewController")]
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

		void ReleaseDesignerOutlets()
		{
			if (Button != null)
			{
				Button.Dispose();
				Button = null;
			}

			if (PositionLabel != null)
			{
				PositionLabel.Dispose();
				PositionLabel = null;
			}

			if (RandomButton != null)
			{
				RandomButton.Dispose();
				RandomButton = null;
			}

			if (ResetButton != null)
			{
				ResetButton.Dispose();
				ResetButton = null;
			}
		}
	}
}
