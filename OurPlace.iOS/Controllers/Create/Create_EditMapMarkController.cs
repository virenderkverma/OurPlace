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
using Foundation;
using Newtonsoft.Json;
using OurPlace.Common.Models;
using OurPlace.iOS.ViewSources;
using UIKit;

namespace OurPlace.iOS
{
    public partial class Create_EditMapMarkController : Create_EditTaskController
	{
        private List<string> minPickerData;
        private List<string> maxPickerData;

		public Create_EditMapMarkController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            minPickerData = new List<string>();

            for (int i = 1; i <= 10; i++)
            {
                minPickerData.Add(i.ToString());
            }

            minPicker.Model = new MapConfigPickerViewModel(minPickerData.ToArray());

            maxPickerData = new List<string>() { "Unlimited" };

            for (int i = 1; i <= 10; i++)
            {
                maxPickerData.Add(i.ToString());
            }

            maxPicker.Model = new MapConfigPickerViewModel(maxPickerData.ToArray());

            // Check if need to load previous edits
            if (!string.IsNullOrWhiteSpace(thisTask?.JsonData))
            {
                MapMarkerTaskData existingData = JsonConvert.DeserializeObject<MapMarkerTaskData>(thisTask.JsonData);

                if (existingData != null)
                {
                    maxPicker.Select(existingData.MaxNumMarkers, 0, true);
                    minPicker.Select(existingData.MinNumMarkers - 1, 0, true);
                    limitToCurrentLoc.On = existingData.UserLocationOnly;
                }
            }
        }

        protected override void FinishButton_TouchUpInside(object sender, EventArgs e)
        {
            if (UpdateBasicTask())
            {
                int maxNum = (int)maxPicker.SelectedRowInComponent(0);

                MapMarkerTaskData taskData = new MapMarkerTaskData
                {
                    MaxNumMarkers = (int)maxPicker.SelectedRowInComponent(0),
                    MinNumMarkers = ((int)minPicker.SelectedRowInComponent(0)) + 1,
                    UserLocationOnly = limitToCurrentLoc.On
                };

                thisTask.JsonData = JsonConvert.SerializeObject(taskData);

                UpdateActivity();
                Unwind();
            }
        } 
	}
}
