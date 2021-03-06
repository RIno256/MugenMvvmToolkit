#region Copyright

// ****************************************************************************
// <copyright file="MvvmDialogFragment.cs">
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
using System.ComponentModel;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using MugenMvvmToolkit.Models;
#if APPCOMPAT
using MugenMvvmToolkit.AppCompat.Interfaces.Mediators;
using MugenMvvmToolkit.AppCompat.Interfaces.Views;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Fragment = Android.Support.V4.App.Fragment;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace MugenMvvmToolkit.AppCompat.Views.Fragments
#else
using MugenMvvmToolkit.FragmentSupport.Interfaces.Mediators;
using MugenMvvmToolkit.FragmentSupport.Interfaces.Views;

namespace MugenMvvmToolkit.FragmentSupport.Views.Fragments
#endif
{
    public abstract class MvvmDialogFragment : DialogFragment, IWindowView
    {
        #region Fields

        private readonly int? _viewId;
        private IMvvmFragmentMediator _mediator;

        #endregion

        #region Constructors

        protected MvvmDialogFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected MvvmDialogFragment(int? viewId)
        {
            _viewId = viewId;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the current <see cref="IMvvmFragmentMediator" />.
        /// </summary>
        protected IMvvmFragmentMediator Mediator
        {
            get
            {
                if (_mediator == null)
                    Interlocked.CompareExchange(ref _mediator,
                        FragmentExtensions.MvvmFragmentMediatorFactory(this, MugenMvvmToolkit.Models.DataContext.Empty), null);
                return _mediator;
            }
        }

        #endregion

        #region Implementation of IView

        /// <summary>
        ///     Gets or sets the data context of the current view.
        /// </summary>
        public object DataContext
        {
            get { return Mediator.DataContext; }
            set { Mediator.DataContext = value; }
        }

        /// <summary>
        ///     Occurred on closing window.
        /// </summary>
        public event EventHandler<IWindowView, CancelEventArgs> Closing
        {
            add { Mediator.Closing += value; }
            remove { Mediator.Closing -= value; }
        }

        /// <summary>
        ///     Occurred on closed window.
        /// </summary>
        public event EventHandler<IWindowView, EventArgs> Canceled
        {
            add { Mediator.Canceled += value; }
            remove { Mediator.Canceled -= value; }
        }

        /// <summary>
        ///     Occurred on destroyed view.
        /// </summary>
        public event EventHandler<IWindowView, EventArgs> Destroyed
        {
            add { Mediator.Destroyed += value; }
            remove { Mediator.Destroyed -= value; }
        }

        /// <summary>
        ///     Occurs when the DataContext property changed.
        /// </summary>
        public event EventHandler<Fragment, EventArgs> DataContextChanged
        {
            add { Mediator.DataContextChanged += value; }
            remove { Mediator.DataContextChanged -= value; }
        }

        #endregion

        #region Overrides of Fragment

        public override void Dismiss()
        {
            Mediator.Dismiss(base.Dismiss);
        }

        public override void OnAttach(Activity activity)
        {
            Mediator.OnAttach(activity, base.OnAttach);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            Mediator.OnCreate(savedInstanceState, base.OnCreate);
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            Mediator.OnCreateOptionsMenu(menu, inflater, base.OnCreateOptionsMenu);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return Mediator.OnCreateView(_viewId, inflater, container, savedInstanceState, base.OnCreateView);
        }

        public override void OnDestroy()
        {
            Mediator.OnDestroy(base.OnDestroy);
        }

        public override void OnDetach()
        {
            Mediator.OnDetach(base.OnDetach);
        }

        public override void OnInflate(Activity activity, IAttributeSet attrs, Bundle savedInstanceState)
        {
            Mediator.OnInflate(activity, attrs, savedInstanceState, base.OnInflate);
        }

        public override void OnPause()
        {
            Mediator.OnPause(base.OnPause);
        }

        public override void OnResume()
        {
            Mediator.OnResume(base.OnResume);
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            Mediator.OnSaveInstanceState(outState, base.OnSaveInstanceState);
        }

        public override void OnStart()
        {
            Mediator.OnStart(base.OnStart);
        }

        public override void OnStop()
        {
            Mediator.OnStop(base.OnStop);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            Mediator.OnViewCreated(view, savedInstanceState, base.OnViewCreated);
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            Mediator.OnCancel(dialog, base.OnDismiss);
        }

        #endregion
    }
}