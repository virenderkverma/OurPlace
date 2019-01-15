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
using FFImageLoading;
using Foundation;
using GlobalToast;
using Newtonsoft.Json;
using OurPlace.Common.LocalData;
using OurPlace.Common.Models;
using OurPlace.iOS.Controllers.Create;
using OurPlace.iOS.ViewSources;
using UIKit;

namespace OurPlace.iOS
{
    public partial class Create_ActivityOverviewController : UITableViewController
    {
        public LearningActivity thisActivity;
        public bool editingSubmitted;
        private bool canFinish;

        private int taskToEditIndex;

        public Create_ActivityOverviewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // If we don't have an activity, get the user to enter some basic details
            if (thisActivity == null)
            {
                PerformSegue("EditMeta", this);
            }

            EditMetaButton.TouchUpInside += EditMetaButton_TouchUpInside;

            FooterButton.TouchUpInside += FooterButton_TouchUpInside;

            NavigationItem.Title = "Edit Activity";
            NavigationItem.RightBarButtonItems = new UIBarButtonItem[] {
            new UIBarButtonItem(UIBarButtonSystemItem.Add, AddNewTask),
            new UIBarButtonItem(UIBarButtonSystemItem.Trash, DeleteTask),
                };

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, ClosePressed);

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 180;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            if (thisActivity != null)
            {
                ActivityName.Text = thisActivity.Name;
                ActivityDescription.Text = thisActivity.Description;
                if (!string.IsNullOrWhiteSpace(thisActivity.ImageUrl))
                {
                    ImageService.Instance.LoadFile(
                        AppUtils.GetPathForLocalFile(thisActivity.ImageUrl))
                                .Into(ActivityImage);
                }
                else
                {
                    ImageService.Instance.LoadCompiledResource("AppLogo").Into(ActivityImage);
                }
            }

            if (thisActivity == null ||
               thisActivity.LearningTasks == null ||
               thisActivity.LearningTasks.ToList().Count == 0)
            {
                canFinish = false;
                FooterButton.SetTitle("Add tasks by tapping +!", UIControlState.Normal);
            }
            else
            {
                canFinish = true;
                FooterButton.SetTitle("Finish", UIControlState.Normal);
                TableView.Source = new CreateViewSource(thisActivity.LearningTasks.ToList(), EditTask, ManageChildren, true);
            }
        }

        public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);
        }

        private void EditTask(int index)
        {
            taskToEditIndex = index;

            LearningTask toEdit = thisActivity.LearningTasks.ElementAt(index);

            switch (toEdit.TaskType.IdName)
            {
                case "LISTEN_AUDIO":
                    PerformSegue("EditListenAudioTask", this);
                    break;
                case "INFO":
                    PerformSegue("EditInfoTask", this);
                    break;
                case "MULT_CHOICE":
                    PerformSegue("EditMultiChoiceTask", this);
                    break;
                case "MAP_MARK":
                    PerformSegue("EditMapMarkTask", this);
                    break;
                case "LOC_HUNT":
                    PerformSegue("EditLocationHuntTask", this);
                    break;
                case "DRAW_PHOTO":
                case "MATCH_PHOTO":
                    PerformSegue("EditChoosePhotoTask", this);
                    break;
                default:
                    PerformSegue("EditParentTask", this);
                    break;
            }
        }

        private void ManageChildren(int parentIndex)
        {
            taskToEditIndex = parentIndex;
            PerformSegue("EditChildren", this);
        }

        private void AddNewTask(object sender, EventArgs e)
        {
            PerformSegue("ChooseTaskType", this);
        }

        private void ClosePressed(object sender, EventArgs e)
        {
            if (!editingSubmitted)
            {
                NavigationController.DismissViewController(true, null);
                return;
            }

            AppUtils.ShowChoiceDialog(
                this,
                "Cancel editing?",
                "Going back will discard any changes you've made. Are you sure?",
                "Discard changes", (res) =>
                {
                    NavigationController.DismissViewController(true, null);
                },
                "Cancel",
                null, thisActivity, UIAlertActionStyle.Destructive);
        }

        private void DeleteTask(object sender, EventArgs e)
        {
            AppUtils.ShowChoiceDialog(
                this,
                string.Format("Delete '{0}'?", thisActivity.Name),
                "Are you sure you want to delete this activity? This can't be undone.",
                "Delete", (res) =>
                {
                    if (false) // TODO
                    {
                        var suppress = DeleteRemoteActivity();
                    }
                    else
                    {
                        var suppress = DeleteLocalActivity();
                    }

                },
                "Cancel",
                null, thisActivity, UIAlertActionStyle.Destructive);
        }

        private async Task DeleteLocalActivity()
        {
            Storage.DeleteInProgress(thisActivity);
            DatabaseManager dbManager = await Storage.GetDatabaseManager(false);
            List<LearningActivity> unsubmittedActivities = JsonConvert.DeserializeObject<List<LearningActivity>>(dbManager.currentUser.LocalCreatedActivitiesJson);
            unsubmittedActivities.RemoveAll((LearningActivity obj) => obj.Id == thisActivity.Id);
            dbManager.currentUser.LocalCreatedActivitiesJson = JsonConvert.SerializeObject(unsubmittedActivities);
            dbManager.AddUser(dbManager.currentUser);
            Toast.ShowToast("Activity Deleted");
            NavigationController.DismissViewController(true, null);
        }

        private async Task DeleteRemoteActivity()
        {
            //ShowLoadingOverlay();
            //ServerResponse<string> resp = await ServerUtils.Delete<string>("/api/learningactivities?id=" + activity.Id);
            //HideLoadingOverlay();

            //if (resp == null)
            //{
            //    var suppress = AppUtils.SignOut(this);
            //    return;
            //}

            //if (resp.Success)
            //{
            //    dbManager.DeleteCachedActivity(activity);
            //    Toast.ShowToast("Activity Deleted");
            //}
            //else
            //{
            //    Toast.ShowToast("Error connecting to the server");
            //}

            //RefreshFeed();
        }

        private void EditMetaButton_TouchUpInside(object sender, EventArgs e)
        {
            PerformSegue("EditMeta", this);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier.Equals("EditChildren"))
            {
                var viewController = (Create_ChildTasksOverviewController)segue.DestinationViewController;
                viewController.thisActivity = thisActivity;
                viewController.parentTaskIndex = taskToEditIndex;
            }
            else if (segue.Identifier.Equals("EditMeta"))
            {
                var viewController = (Create_ActivityMetaController)segue.DestinationViewController;
                viewController.thisActivity = thisActivity;
            }
            else if (new string[]{
                    "EditParentTask",
                    "EditChoosePhotoTask",
                    "EditLocationHuntTask",
                    "EditMapMarkTask",
                    "EditMultiChoiceTask",
                    "EditInfoTask",
                    "EditListenAudioTask"
                }.Contains(segue.Identifier))
            {
                var viewController = (Create_EditTaskController)segue.DestinationViewController;
                viewController.thisActivity = thisActivity;
                viewController.parentTaskIndex = taskToEditIndex;
                viewController.childTaskIndex = null;
            }
            else if (segue.Identifier.Equals("ChooseTaskType"))
            {
                var viewController = (Create_ChooseTaskTypeController)segue.DestinationViewController;
                viewController.thisActivity = thisActivity;
                viewController.parentTaskIndex = (thisActivity.LearningTasks == null) ? 0 : thisActivity.LearningTasks.Count();
                viewController.childTaskIndex = null;
            }
            else if (segue.Identifier.Equals("FinishCreate"))
            {
                var viewController = (Create_FinishController)segue.DestinationViewController;
                viewController.thisActivity = thisActivity;
            }
        }

        private void FooterButton_TouchUpInside(object sender, EventArgs e)
        {
            if (canFinish)
            {
                PerformSegue("FinishCreate", this);
            }
            else
            {
                PerformSegue("ChooseTaskType", this);
            }
        }

        // All activity editing controllers end up back here. 
        [Action("UnwindToOverview:")]
        public void UnwindToOverview(UIStoryboardSegue segue)
        {
            Console.WriteLine("Unwind to overview");

            var sourceController = segue.SourceViewController as Create_BaseSegueController;

            if (sourceController != null)
            {
                if (sourceController.wasCancelled && thisActivity == null)
                {
                    // If the user cancelled without any info being saved, 
                    // return to the previous screen
                    NavigationController.DismissViewController(true, null);
                    return;
                }

                if (sourceController.thisActivity != null)
                {
                    thisActivity = sourceController.thisActivity;
                    if (TableView.Source != null)
                    {
                        (TableView.Source as CreateViewSource).Rows = (List<LearningTask>)thisActivity.LearningTasks;
                        TableView.ReloadData();
                    }
                }

                var suppress = SaveProgress();
            }
        }

        private async Task SaveProgress()
        {
            DatabaseManager dbManager = await Storage.GetDatabaseManager(false);

            // Add/update this new activity in the user's inprogress cache
            string cacheJson = dbManager.currentUser.LocalCreatedActivitiesJson;

            List<LearningActivity> inProgress = (string.IsNullOrWhiteSpace(cacheJson)) ?
                new List<LearningActivity>() :
                JsonConvert.DeserializeObject<List<LearningActivity>>(cacheJson);

            int existingInd = inProgress.FindIndex((la) => la.Id == thisActivity.Id);

            if (existingInd == -1)
            {
                inProgress.Insert(0, thisActivity);
            }
            else
            {
                inProgress.RemoveAt(existingInd);
                inProgress.Insert(0, thisActivity);
            }

            dbManager.currentUser.LocalCreatedActivitiesJson = JsonConvert.SerializeObject(inProgress);
            dbManager.AddUser(dbManager.currentUser);

        }

    }
}
