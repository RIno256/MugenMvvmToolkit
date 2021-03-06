#region Copyright

// ****************************************************************************
// <copyright file="ViewFactory.cs">
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

using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Util;
using Android.Views;
using MugenMvvmToolkit.Binding;
using MugenMvvmToolkit.Binding.Interfaces.Models;
using MugenMvvmToolkit.Binding.Models;
using MugenMvvmToolkit.DataConstants;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Models;

namespace MugenMvvmToolkit.Infrastructure
{
    public class ViewFactory : IViewFactory
    {
        #region Fields

        private static readonly int[] BindingAttrIndex;

        #endregion

        #region Constructors

        static ViewFactory()
        {
            BindingAttrIndex = new[] { Resource.Styleable.Binding_Bind, Resource.Styleable.Binding_Bindings };
        }

        #endregion

        #region Implementation of IViewFactory

        /// <summary>
        ///     Creates an instance of <see cref="ViewResult" /> using the view name.
        /// </summary>
        public virtual ViewResult Create(string name, Context context, IAttributeSet attrs)
        {
            Should.NotBeNullOrWhitespace(name, "name");
            Type type = TypeCache<View>.Instance.GetTypeByName(name, false, true);
            return Create(type, context, attrs);
        }

        /// <summary>
        ///     Creates an instance of <see cref="ViewResult" /> using the view type.
        /// </summary>
        public virtual ViewResult Create(Type type, Context context, IAttributeSet attrs)
        {
            Should.NotBeNull(type, "type");
            Should.NotBeNull(context, "context");
            Should.NotBeNull(attrs, "attrs");
            var view = type.CreateView(context, attrs);
            return new ViewResult(view, GetDataContext(view, context, attrs));
        }

        #endregion

        #region Methods

        protected virtual IDataContext GetDataContext(View view, Context context, IAttributeSet attrs)
        {
            var dataContext = new DataContext();
            var strings = ReadStringAttributeValue(context, attrs, Resource.Styleable.Binding, BindingAttrIndex);
            if (strings != null && strings.Count != 0)
                dataContext.Add(ViewFactoryConstants.Bindings, strings);

            SetAttributeValue(view, context, attrs, Resource.Styleable.ItemsControl,
                Resource.Styleable.ItemsControl_ItemTemplate, AttachedMemberConstants.ItemTemplate, dataContext,
                ViewFactoryConstants.ItemTemplateId);

            SetAttributeValue(view, context, attrs, Resource.Styleable.ItemsControl,
                Resource.Styleable.ItemsControl_DropDownItemTemplate, AttachedMemberNames.DropDownItemTemplate,
                dataContext,
                ViewFactoryConstants.DropDownItemTemplateId);

            SetAttributeValue(view, context, attrs, Resource.Styleable.Control,
                Resource.Styleable.Control_ContentTemplate, AttachedMemberConstants.ContentTemplate, dataContext,
                ViewFactoryConstants.ContentTemplateId);

            SetAttributeValue(view, context, attrs, Resource.Styleable.Menu,
                Resource.Styleable.Menu_MenuTemplate, AttachedMemberNames.MenuTemplate, dataContext,
                ViewFactoryConstants.MenuTemplateId);


            SetAttributeValue(view, context, attrs, Resource.Styleable.Menu,
                Resource.Styleable.Menu_PopupMenuTemplate, AttachedMemberNames.PopupMenuTemplate, dataContext,
                ViewFactoryConstants.PopupMenuTemplateId);

            strings = ReadStringAttributeValue(context, attrs, Resource.Styleable.Menu,
                new[] { Resource.Styleable.Menu_PopupMenuEvent });
            if (strings != null && strings.Count > 0)
            {
                string eventName = strings[0];
                dataContext.Add(ViewFactoryConstants.PopupMenuEvent, eventName);
                IBindingMemberInfo member = BindingServiceProvider
                    .MemberProvider
                    .GetBindingMember(view.GetType(), AttachedMemberNames.PopupMenuEvent, false, false);
                if (member != null)
                    member.SetValue(view, new object[] { eventName });
            }

            strings = ReadStringAttributeValue(context, attrs, Resource.Styleable.Menu,
                new[] { Resource.Styleable.Menu_PlacementTargetPath });
            if (strings != null && strings.Count > 0)
            {
                string path = strings[0];
                dataContext.Add(ViewFactoryConstants.PlacementTargetPath, path);
                IBindingMemberInfo member = BindingServiceProvider
                    .MemberProvider
                    .GetBindingMember(view.GetType(), AttachedMemberNames.PlacementTargetPath, false, false);
                if (member != null)
                    member.SetValue(view, new object[] { path });
            }

            return dataContext;
        }

        private static void SetAttributeValue(View view, Context context, IAttributeSet attrs, int[] groupId,
            int requiredAttributeId, string attachedMemberName, IDataContext dataContext, DataConstant<int> constant)
        {
            int? value = ReadAttributeValueId(context, attrs, groupId, requiredAttributeId);
            if (!value.HasValue)
                return;
            dataContext.Add(constant, value.Value);
            IBindingMemberInfo member = BindingServiceProvider
                .MemberProvider
                .GetBindingMember(view.GetType(), attachedMemberName, false, false);
            if (member != null)
                member.SetValue(view, new object[] { value });
        }

        internal static List<string> ReadStringAttributeValue(Context context, IAttributeSet attrs, int[] groupId,
            ICollection<int> requiredAttributeIds)
        {
            TypedArray typedArray = context.Theme.ObtainStyledAttributes(attrs, groupId, 0, 0);
            try
            {
                List<string> result = null;
                foreach (int attributeId in requiredAttributeIds)
                {
                    string s = typedArray.GetString(attributeId);
                    if (string.IsNullOrEmpty(s))
                        continue;
                    if (result == null)
                        result = new List<string>();
                    result.Add(s);
                }
                return result;
            }
            finally
            {
                typedArray.Recycle();
            }
        }

        private static int? ReadAttributeValueId(Context context, IAttributeSet attrs, int[] groupId,
            int requiredAttributeId)
        {
            TypedArray typedArray = context.Theme.ObtainStyledAttributes(attrs, groupId, 0, 0);
            try
            {
                int result = typedArray.GetResourceId(requiredAttributeId, int.MinValue);
                return result == int.MinValue ? (int?)null : result;
            }
            finally
            {
                typedArray.Recycle();
            }
        }

        #endregion
    }
}