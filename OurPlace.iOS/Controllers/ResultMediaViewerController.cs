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
using System.IO;
using AVFoundation;
using FFImageLoading;
using Foundation;
using OurPlace.Common.Models;
using UIKit;

namespace OurPlace.iOS
{
    public partial class ResultMediaViewerController : UIViewController
    {
        public string FilePath { get; set; }
        public AppTask Task { get; set; }
        public int ResultIndex { get; set; }
        public Action<AppTask, string, int> DeleteResult { get; set; }

        private AVPlayerItem playerItem;
        private AVQueuePlayer player;
        private AVPlayerLayer playerLayer;
        private AVPlayerLooper playerLooper;

        public ResultMediaViewerController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (string.IsNullOrEmpty(FilePath) || !File.Exists(FilePath))
            {
                NavigationController.PopViewController(true);
            }

            string taskType = Task.TaskType.IdName;

            if (taskType == "LISTEN_AUDIO" || taskType == "REC_AUDIO" || taskType == "TAKE_VIDEO")
            {
                // Load video, play and loop
                playerItem = AVPlayerItem.FromUrl(NSUrl.FromFilename(FilePath));
                player = AVQueuePlayer.FromItems(new AVPlayerItem[] { playerItem });
                playerLayer = AVPlayerLayer.FromPlayer(player);
                playerLooper = AVPlayerLooper.FromPlayer(player, playerItem);

                if (taskType == "TAKE_VIDEO")
                {
                    playerLayer.Frame = View.Frame;
                    View.Layer.AddSublayer(playerLayer);
                }
                else
                {
                    ImageView.Hidden = true;
                    AudioIcon.Hidden = false;
                    DescLabel.Text = Task.Description;
                }

                player.Play();
            }
            else
            {
                // Load image
                ImageService.Instance.LoadFile(FilePath).Error((e) =>
                {
                    Console.WriteLine("ERROR LOADING IMAGE: " + e.Message);
                    NavigationController.PopViewController(true);
                }).Into(ImageView);
            }

            if (DeleteResult != null)
            {
                UIBarButtonItem customButton = new UIBarButtonItem(
                    UIImage.FromFile("ic_delete"),
                    UIBarButtonItemStyle.Plain,
                    (s, e) =>
                    {
                        ConfirmDelete();
                    }
                );

                NavigationItem.RightBarButtonItem = customButton;
            }
        }

        private void ConfirmDelete()
        {
            var alertController = UIAlertController.Create("Confirm Delete", "Are you sure you want to delete this file?", UIAlertControllerStyle.Alert);
            alertController.AddAction(UIAlertAction.Create("Delete", UIAlertActionStyle.Destructive, (obj) =>
            {
                DeleteResult(Task, FilePath, ResultIndex);
                NavigationController.PopViewController(true);
            }));
            alertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, (obj) => { }));
            PresentViewController(alertController, true, null);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            if (player != null)
            {
                player.Pause();
                playerLooper.Dispose();
                player.Dispose();
            }
        }
    }
}
