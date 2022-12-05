﻿using UnityEngine;
using UnityEditor;

namespace Suzuryg.FacialExpressionSwitcher.Detail.Localization
{
    public class LocalizationTable : ScriptableObject
    {
        public string MainView_Language = "Language";
        public string MainView_UpdateThumbnails = "Update Thumbnails";

        public string HierarchyView_Title = "HierarchyView";
        public string HierarchyView_RegisteredMenuItemList = "ItemsToRegister";
        public string HierarchyView_UnregisteredMenuItemList = "ItemsNotToRegister";

        public string MenuItemListView_Title = "MenuItemView";
        public string MenuItemListView_UseAnimationNameAsDisplayName = "Use Animation Name As Mode Name";
        public string MenuItemListView_Blinking = "Blinking";
        public string MenuItemListView_LipSync = "Lip Sync";
        public string MenuItemListView_Enable = "Enable";
        public string MenuItemListView_Disable = "Disable";
        public string MenuItemListView_Empty = "This group is empty.";

        public string BranchListView_Title = "BranchView";
        public string BranchListView_UseLeftTrigger = "Use Left Trigger";
        public string BranchListView_UseRightTrigger = "Use Right Trigger";
        public string BranchListView_NotReachableBranch = "This branch is not used.";
        public string BranchListView_LeftTriggerAnimation = "Left Trigger";
        public string BranchListView_RightTriggerAnimation = "Right Trigger";
        public string BranchListView_BothTriggersAnimation = "Both Triggers";
        public string BranchListView_EmptyBranch = "There are no branches.";
        public string BranchListView_EmptyCondition = "There are no conditions.";
        public string BranchListView_Condition = "Conditions";
        public string BranchListView_Left = "LeftHand";
        public string BranchListView_Right = "RightHand";
        public string BranchListView_OneSide = "OneHand";
        public string BranchListView_Either = "EitherHands";
        public string BranchListView_Both = "BothHands";
        public string BranchListView_Neutral = "Neutral";
        public string BranchListView_Fist = "Fist";
        public string BranchListView_HandOpen = "HandOpen";
        public string BranchListView_Fingerpoint = "Fingerpoint";
        public string BranchListView_Victory = "Victory";
        public string BranchListView_RockNRoll = "RockNRoll";
        public string BranchListView_HandGun = "HandGun";
        public string BranchListView_ThumbsUp = "ThumbsUp";
        public string BranchListView_Equals = "Equals";
        public string BranchListView_NotEqual = "NotEqual";

        public string GestureTableView_AddBranch = "Add Branch";
        public string GestureTableView_Neutral = "Neutral";
        public string GestureTableView_Fist = "Fist";
        public string GestureTableView_HandOpen = "HandOpen";
        public string GestureTableView_Fingerpoint = "Fingerpoint";
        public string GestureTableView_Victory = "Victory";
        public string GestureTableView_RockNRoll = "RockNRoll";
        public string GestureTableView_HandGun = "HandGun";
        public string GestureTableView_ThumbsUp = "ThumbsUp";
    }
}
