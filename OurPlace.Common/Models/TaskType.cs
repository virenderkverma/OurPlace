﻿#region copyright
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
namespace OurPlace.Common.Models
{
    public class TaskType : Model
    {
        public string DisplayName { get; set; }
        public string IdName { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public bool ReqFileUpload { get; set; }
    }

    public class MapMarkerTaskData
    {
        public int MinNumMarkers { get; set; }
        public int MaxNumMarkers { get; set; }
        public bool UserLocationOnly { get; set; }
    }
}
