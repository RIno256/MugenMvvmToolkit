#region Copyright

// ****************************************************************************
// <copyright file="ActionBarTemplate.cs">
// Copyright (c) 2012-2015 Vyacheslav Volkov
// </copyright>
// ****************************************************************************
// <author>Vyacheslav Volkov</author>
// <email>vvs0205@outlook.com</email>
// <project>MugenMvvmToolkit</project>
// <web>https://github.com/MugenMvvmToolkit/MugenMvvmToolkit</web>
// <license>
// See license.txt in this solution or http://opensource.org/licenses/MS-PL
// </license>
// ****************************************************************************

#endregion

using System.Collections.Generic;
using System.Xml.Serialization;
using Android.App;
using Android.OS;
using MugenMvvmToolkit.Binding.Builders;
using MugenMvvmToolkit.Binding.Infrastructure;
using MugenMvvmToolkit.Interfaces.Views;
using MugenMvvmToolkit.Models.EventArg;
#if APPCOMPAT
using Android.Support.V7.App;
using MugenMvvmToolkit.AppCompat.Infrastructure;
using ActionBar = Android.Support.V7.App.ActionBar;

namespace MugenMvvmToolkit.AppCompat.Models
#else
using MugenMvvmToolkit.ActionBarSupport.Infrastructure;

namespace MugenMvvmToolkit.ActionBarSupport.Models
#endif
{
    [XmlRoot("ACTIONBAR")]
    public sealed class ActionBarTemplate
    {
        #region Fields

        private const string SelectedTabIndexKey = "~@tabindex";

        #endregion

        #region Properties

        [XmlAttribute("DATACONTEXT")]
        public string DataContext { get; set; }

        [XmlAttribute("BACKGROUNDDRAWABLE")]
        public string BackgroundDrawable { get; set; }

        [XmlAttribute("CUSTOMVIEW")]
        public string CustomView { get; set; }

        [XmlAttribute("DISPLAYHOMEASUPENABLED")]
        public string DisplayHomeAsUpEnabled { get; set; }

        [XmlAttribute("DISPLAYOPTIONS")]
        public string DisplayOptions { get; set; }

        [XmlAttribute("DISPLAYSHOWCUSTOMENABLED")]
        public string DisplayShowCustomEnabled { get; set; }

        [XmlAttribute("DISPLAYSHOWHOMEENABLED")]
        public string DisplayShowHomeEnabled { get; set; }

        [XmlAttribute("DISPLAYSHOWTITLEENABLED")]
        public string DisplayShowTitleEnabled { get; set; }

        [XmlAttribute("DISPLAYUSELOGOENABLED")]
        public string DisplayUseLogoEnabled { get; set; }

        [XmlAttribute("HOMEBUTTONENABLED")]
        public string HomeButtonEnabled { get; set; }

        [XmlAttribute("HOMEBUTTONCLICK")]
        public string HomeButtonClick { get; set; }

        [XmlAttribute("BACKBUTTONCLICK")]
        public string BackButtonClick
        {
            get { return HomeButtonClick; }
            set { HomeButtonClick = value; }
        }

        [XmlAttribute("ICON")]
        public string Icon { get; set; }

        [XmlAttribute("LOGO")]
        public string Logo { get; set; }

        [XmlAttribute("NAVIGATIONMODE")]
        public string NavigationMode { get; set; }

        [XmlAttribute("SPLITBACKGROUNDDRAWABLE")]
        public string SplitBackgroundDrawable { get; set; }

        [XmlAttribute("STACKEDBACKGROUNDDRAWABLE")]
        public string StackedBackgroundDrawable { get; set; }

        [XmlAttribute("ISSHOWING")]
        public string IsShowing { get; set; }

        [XmlAttribute("SUBTITLE")]
        public string Subtitle { get; set; }

        [XmlAttribute("TITLE")]
        public string Title { get; set; }

        [XmlAttribute("VISIBLE")]
        public string Visible { get; set; }

        [XmlAttribute("SELECTEDITEM")]
        public string SelectedItem { get; set; }

        [XmlAttribute("ITEMSSOURCE")]
        public string ItemsSource { get; set; }

        [XmlElement("RESTORETABSELECTEDINDEX")]
        public string RestoreTabSelectedIndex { get; set; }

        [XmlElement("TABTEMPLATE")]
        public ActionBarTabTemplate TabTemplate { get; set; }

        [XmlElement("TAB")]
        public List<ActionBarTabTemplate> Tabs { get; set; }

        [XmlAttribute("CONTEXTACTIONBARVISIBLE")]
        public string ContextActionBarVisible { get; set; }

        [XmlAttribute("CONTEXTACTIONBARTEMPLATE")]
        public string ContextActionBarTemplate { get; set; }

        #endregion

        #region Methods

        public void Apply(Activity activity)
        {
            PlatformExtensions.ValidateTemplate(ItemsSource, Tabs);
            var actionBar = activity.GetActionBar();

            var setter = new XmlPropertySetter<ActionBarTemplate, ActionBar>(actionBar, activity, new BindingSet());
            setter.SetEnumProperty<ActionBarNavigationMode>(template => template.NavigationMode, NavigationMode);
            setter.SetProperty(template => template.DataContext, DataContext);

            setter.SetProperty(template => template.ContextActionBarTemplate, ContextActionBarTemplate);
            setter.SetBinding(template => template.ContextActionBarVisible, ContextActionBarVisible, false);
            setter.SetProperty(template => template.BackgroundDrawable, BackgroundDrawable);
            setter.SetProperty(template => template.CustomView, CustomView);
            setter.SetEnumProperty<ActionBarDisplayOptions>(template => template.DisplayOptions, DisplayOptions);
            setter.SetBoolProperty(template => template.DisplayHomeAsUpEnabled, DisplayHomeAsUpEnabled);
            setter.SetBoolProperty(template => template.DisplayShowCustomEnabled, DisplayShowCustomEnabled);
            setter.SetBoolProperty(template => template.DisplayShowHomeEnabled, DisplayShowHomeEnabled);
            setter.SetBoolProperty(template => template.DisplayShowTitleEnabled, DisplayShowTitleEnabled);
            setter.SetBoolProperty(template => template.DisplayUseLogoEnabled, DisplayUseLogoEnabled);
            setter.SetBoolProperty(template => template.HomeButtonEnabled, HomeButtonEnabled);
            setter.SetProperty(template => template.Icon, Icon);
            setter.SetProperty(template => template.Logo, Logo);
            setter.SetProperty(template => template.SplitBackgroundDrawable, SplitBackgroundDrawable);
            setter.SetProperty(template => template.StackedBackgroundDrawable, StackedBackgroundDrawable);
            setter.SetBoolProperty(template => template.IsShowing, IsShowing);
            setter.SetStringProperty(template => template.Subtitle, Subtitle);
            setter.SetStringProperty(template => template.Title, Title);
            setter.SetBoolProperty(template => template.Visible, Visible);
            setter.SetBinding("HomeButton.Click", HomeButtonClick, false);

            if (string.IsNullOrEmpty(ItemsSource))
            {
                if (Tabs != null)
                {
                    ActionBar.Tab firstTab = null;
                    for (int index = 0; index < Tabs.Count; index++)
                    {
                        var tab = Tabs[index].CreateTab(actionBar);
                        if (firstTab == null)
                            firstTab = tab;
                        actionBar.AddTab(tab);
                    }
                    TryRestoreSelectedIndex(activity, actionBar);
                }
            }
            else
            {
                ActionBarTabItemsSourceGenerator.Set(actionBar, TabTemplate);
                setter.SetBinding(template => template.ItemsSource, ItemsSource, false);
            }
            setter.SetBinding(template => template.SelectedItem, SelectedItem, false);
            setter.Apply();
        }

        public static void Clear(Activity activity)
        {
            var actionBar = activity.GetActionBar(false);
            if (actionBar == null)
                return;
            for (int i = 0; i < actionBar.TabCount; i++)
                ActionBarTabTemplate.ClearTab(actionBar, actionBar.GetTabAt(i), false);
            actionBar.ClearBindings(true, true);
        }

        private void TryRestoreSelectedIndex(Activity activity, ActionBar actionBar)
        {
            if (actionBar.GetNavigationMode() == ActionBarNavigationMode.Standard)
                return;
            bool result;
            if (bool.TryParse(RestoreTabSelectedIndex, out result) && !result)
                return;
            var activityView = activity as IActivityView;
            if (activityView == null)
                return;
            activityView.Mediator.SaveInstanceState += ActivityViewOnSaveInstanceState;

            var bundle = activityView.Mediator.Bundle;
            if (bundle != null)
            {
                var i = bundle.GetInt(SelectedTabIndexKey, int.MinValue);
                if (i != int.MinValue && i != actionBar.SelectedNavigationIndex)
                    actionBar.SetSelectedNavigationItem(i);
            }
        }

        private static void ActivityViewOnSaveInstanceState(Activity sender, ValueEventArgs<Bundle> args)
        {
            var actionBar = sender.GetActionBar();
            if (actionBar == null)
                return;
            var index = actionBar.SelectedNavigationIndex;
            args.Value.PutInt(SelectedTabIndexKey, index);
        }

        #endregion
    }
}