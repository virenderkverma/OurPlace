#region copyright
/*
    OurPlace is a mobile learning platform, designed to support communities
    in creating and sharing interactive learning activities about the places they care most about.
    https://github.com/GSDan/OurPlace
    Copyright (C) 2018 Dan Richardson

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see https://www.gnu.org/licenses.
*/
#endregion

// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using Google.Places;
using Google.Places.Picker;
using Newtonsoft.Json;
using OurPlace.Common.LocalData;
using OurPlace.Common.Models;
using UIKit;

namespace OurPlace.iOS
{
    public partial class Create_FinishController : UIViewController, IPlacePickerViewControllerDelegate
    {
        public LearningActivity thisActivity;
        private PlacePickerViewController placePicker;
        private Common.Models.Place chosenPlace;

        public Create_FinishController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            finishButton.TouchUpInside += FinishButton_TouchUpInside;
            chooseLocationButton.TouchUpInside += ChooseLocationButton_TouchUpInside;
        }

        private void ChooseLocationButton_TouchUpInside(object sender, EventArgs e)
        {
            PlacePickerConfig config = new PlacePickerConfig(null);
            placePicker = new PlacePickerViewController(config) { Delegate = this };
            PresentViewController(placePicker, true, null);
        }

        public void DidPickPlace(PlacePickerViewController viewController, Google.Places.Place place)
        {
            DismissViewController(true, null);

            if (place == null) return;

            chosenPlace = new Common.Models.Place
            {
                GooglePlaceId = place.Id,
                Latitude = new decimal(place.Coordinate.Latitude),
                Longitude = new decimal(place.Coordinate.Longitude),
                Name = place.Name
            };

            if (place.Types != null && place.Types.Contains("synthetic_geocode"))
            {
                chosenPlace.Name = "An Unknown Location";
            }

            locationLabel.Text = chosenPlace.Name;
            chooseLocationButton.SetTitle("Change Location", UIControlState.Normal);
        }

        [Export("placePickerDidCancel:")]
        void DidCancel(PlacePickerViewController viewController)
        {
            // Dismiss the place picker, as it cannot dismiss itself.
            DismissViewController(true, null);
            Console.WriteLine("No place selected");
        }

        private void FinishButton_TouchUpInside(object sender, EventArgs e)
        {
            AppUtils.ShowChoiceDialog(this,
                "Finish?",
                "Are you sure you're finished creating your activity? You can't edit past this point.",
                "Finish",
                (arg) => { var suppress = SaveAndFinish(); },
                "Cancel",
                null,
                true);
        }

        private async Task SaveAndFinish()
        {
            thisActivity.IsPublic = isPublicSwitch.On;
            thisActivity.RequireUsername = reqNamesSwitch.On;
            thisActivity.CreatedAt = DateTime.UtcNow;

            if (chosenPlace != null)
            {
                thisActivity.Places = new Common.Models.Place[] { chosenPlace };
            }

            var uploadData = await Storage.PrepCreatedActivityForUpload(thisActivity, false);

            NavigationController.PopToRootViewController(true);
        }

    }
}
