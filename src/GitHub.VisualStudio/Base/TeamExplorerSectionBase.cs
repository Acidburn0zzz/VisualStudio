﻿using System;
using GitHub.VisualStudio.Helpers;
using Microsoft.TeamFoundation.Controls;
using NullGuard;
using GitHub.Services;
using System.Diagnostics;
using System.Threading;
using GitHub.Extensions;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using GitHub.Api;
using GitHub.Models;

namespace GitHub.VisualStudio.Base
{
    public class TeamExplorerSectionBase : TeamExplorerItemBase, ITeamExplorerSection
    {
        protected IConnectionManager connectionManager;

        bool isBusy;
        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; this.RaisePropertyChange(); }
        }

        bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; this.RaisePropertyChange(); }
        }

        object sectionContent;
        [AllowNull]
        public object SectionContent
        {
            get { return sectionContent; }
            set { sectionContent = value; this.RaisePropertyChange(); }
        }

        string title;
        [AllowNull]
        public string Title
        {
            get { return title; }
            set { title = value; this.RaisePropertyChange(); }
        }

        [return: AllowNull]
        public virtual object GetExtensibilityService(Type serviceType)
        {
            return null;
        }

        public TeamExplorerSectionBase(ISimpleApiClientFactory apiFactory, ITeamExplorerServiceHolder holder)
            : base(apiFactory, holder)
        {
            IsVisible = false;
            IsEnabled = true;
            IsExpanded = true;
        }

        public TeamExplorerSectionBase(ISimpleApiClientFactory apiFactory, ITeamExplorerServiceHolder holder,
            IConnectionManager cm) : this(apiFactory, holder)
        {
            connectionManager = cm;
        }

        public virtual void Cancel()
        {
        }

        public virtual void Initialize(object sender, SectionInitializeEventArgs e)
        {
#if DEBUG
//            VsOutputLogger.WriteLine("{0:HHmmssff}\t{1} Initialize", DateTime.Now, GetType());
#endif
            ServiceProvider = e.ServiceProvider;
            Debug.Assert(holder != null, "Could not get an instance of TeamExplorerServiceHolder");
            if (holder == null)
                return;
            holder.ServiceProvider = e.ServiceProvider;
            SubscribeToRepoChanges();
#if DEBUG
//            VsOutputLogger.WriteLine("{0:HHmmssff}\t{1} Initialize DONE", DateTime.Now, GetType());
#endif
        }

        public virtual void Loaded(object sender, SectionLoadedEventArgs e)
        {
        }

        public virtual void Refresh()
        {
        }

        public virtual void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
        }

        void SubscribeToRepoChanges()
        {
            holder.Subscribe(this, (ISimpleRepositoryModel repo) =>
            {
                ActiveRepo = repo;
                RepoChanged();
            });
        }

        void Unsubscribe()
        {
            holder.Unsubscribe(this);
            holder.ClearServiceProvider(ServiceProvider);
        }

        bool disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!disposed)
                {
                    Unsubscribe();
                    disposed = true;
                }
            }
            base.Dispose(disposing);
        }
    }
}
