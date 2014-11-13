﻿using System;
using MugenMvvmToolkit.Binding;
using MugenMvvmToolkit.Binding.Interfaces;
using MugenMvvmToolkit.Binding.Interfaces.Models;
using MugenMvvmToolkit.Binding.Models;
using MugenMvvmToolkit.Models;
using Xamarin.Forms;

namespace MugenMvvmToolkit
{
    public class PlatformDataBindingModule : DataBindingModule
    {
        #region Methods

        private static void Register(IBindingMemberProvider memberProvider)
        {
            //VisualElement
            var visibleMember = memberProvider.GetBindingMember(typeof(VisualElement),
                ToolkitExtensions.GetMemberName<VisualElement>(element => element.IsVisible), true, false);
            if (visibleMember != null)
            {
                memberProvider.Register(typeof(VisualElement), "Visible", visibleMember, true);
                memberProvider.Register(AttachedBindingMember.CreateMember<VisualElement, bool>("Hidden",
                    (info, element) => !element.IsVisible, (info, element, arg3) => element.IsVisible = !arg3,
                    (info, element, arg3) => visibleMember.TryObserve(element, arg3)));
            }
            memberProvider.Register(AttachedBindingMember
                .CreateMember<VisualElement, object>(AttachedMemberConstants.Parent, GetParentValue, null, ObserveParentMember));
            memberProvider.Register(AttachedBindingMember
                .CreateMember<VisualElement, object>(AttachedMemberConstants.FindByNameMethod, FindByNameMemberImpl));

            memberProvider.Register(AttachedBindingMember.CreateMember<VisualElement, bool>(AttachedMemberConstants.Focused, (info, element) => element.IsFocused,
                (info, element, arg3) =>
                {
                    if (arg3)
                        element.Focus();
                    else
                        element.Unfocus();
                }, (info, element, arg3) => BindingServiceProvider.WeakEventManager.Subscribe(element, "IsFocused", arg3)));

            var enabledMember = memberProvider.GetBindingMember(typeof(VisualElement),
                ToolkitExtensions.GetMemberName<VisualElement>(element => element.IsEnabled), true, false);
            if (enabledMember != null)
                memberProvider.Register(typeof(VisualElement), AttachedMemberConstants.Enabled, enabledMember, true);
        }

        private static object FindByNameMemberImpl(IBindingMemberInfo bindingMemberInfo, VisualElement target, object[] arg3)
        {
            var name = (string)arg3[0];
            return target.FindByName<object>(name);
        }

        private static object GetParentValue(IBindingMemberInfo bindingMemberInfo, VisualElement target)
        {
            return ParentObserver.GetOrAdd(target).Parent;
        }

        private static IDisposable ObserveParentMember(IBindingMemberInfo bindingMemberInfo, VisualElement o, IEventListener arg3)
        {
            return ParentObserver.GetOrAdd(o).AddWithUnsubscriber(arg3);
        }

        #endregion
    }
}