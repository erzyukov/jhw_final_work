#if UNITY_2022_1_OR_NEWER
#define UNLOAD_BUNDLE_ASYNC
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

using System;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityEngine.Localization.Operations
{
    /// <summary>
    /// Performs all initialization work for the LocalizationSettings.
    /// </summary>
    class InitializationOperation : WaitForCurrentOperationAsyncOperationBase<LocalizationSettings>
    {
        #if UNLOAD_BUNDLE_ASYNC
        // We need to wait for unloading to complete first to avoid any potential conflicts with unloading and loading the same bundle at the same time. (LOC-1021)
        class UnloadBundlesOperation : AsyncOperationBase<object>
        {
            readonly Action<AsyncOperation> m_OperationCompleted;
            readonly List<AsyncOperation> m_UnloadBundleOperations = new List<AsyncOperation>();

            public UnloadBundlesOperation()
            {
                m_OperationCompleted = OnOperationCompleted;
            }

            protected override void Execute()
            {
                if (AssetBundleProvider.UnloadingAssetBundleCount == 0)
                {
                    Complete(null, true, null);
                    return;
                }

                m_UnloadBundleOperations.Clear();
                foreach (var operation in AssetBundleProvider.UnloadingBundles.Values)
                {
                    if (!operation.isDone)
                    {
                        m_UnloadBundleOperations.Add(operation);
                        operation.completed += m_OperationCompleted;
                    }
                }
            }

            private void OnOperationCompleted(AsyncOperation obj)
            {
                m_UnloadBundleOperations.Remove(obj);
                if (m_UnloadBundleOperations.Count == 0)
                {
                    Complete(null, true, null);
                }
            }

            protected override bool InvokeWaitForCompletion()
            {
                AssetBundleProvider.WaitForAllUnloadingBundlesToComplete();
                return true;
            }

            protected override void Destroy() => GenericPool<UnloadBundlesOperation>.Release(this);
        }

        AsyncOperationHandle m_UnloadBundlesOperationHandle;
        readonly Action<AsyncOperationHandle> m_LoadLocales;
        #endif

        readonly Action<AsyncOperationHandle<Locale>> m_PreloadTablesAction;
        readonly Action<AsyncOperationHandle> m_FinishInitializingAction;

        LocalizationSettings m_Settings;
        readonly List<AsyncOperationHandle> m_LoadDatabasesOperations = new List<AsyncOperationHandle>();

        AsyncOperationHandle<IList<AsyncOperationHandle>> m_PreloadDatabasesOperation;
        int m_RemainingSteps;
        const int k_PreloadSteps = 3;

        protected override float Progress
        {
            get
            {
                if (CurrentOperation.IsValid())
                    return (k_PreloadSteps - m_RemainingSteps + CurrentOperation.PercentComplete) / (k_PreloadSteps + 1);
                return base.Progress;
            }
        }

        protected override string DebugName => "Localization Settings Initialization";

        public InitializationOperation()
        {
            m_PreloadTablesAction = a => PreloadTables();
            m_FinishInitializingAction = FinishInitializing;

            #if UNLOAD_BUNDLE_ASYNC
            m_LoadLocales = _ => LoadLocales();
            #endif
        }

        public void Init(LocalizationSettings settings)
        {
            m_Settings = settings;
            m_LoadDatabasesOperations.Clear();
            m_RemainingSteps = k_PreloadSteps;
        }

        protected override void Execute()
        {
            #if UNLOAD_BUNDLE_ASYNC
            var unloadBundlesOpeeration = GenericPool<UnloadBundlesOperation>.Get();
            m_UnloadBundlesOperationHandle = AddressablesInterface.ResourceManager.StartOperation(unloadBundlesOpeeration, default);
            if (!m_UnloadBundlesOperationHandle.IsDone)
            {
                CurrentOperation = m_UnloadBundlesOperationHandle;
                m_UnloadBundlesOperationHandle.Completed += m_LoadLocales;
                return;
            }
            #endif

            LoadLocales();
        }

        void LoadLocales()
        {
            #if UNLOAD_BUNDLE_ASYNC
            AddressablesInterface.SafeRelease(m_UnloadBundlesOperationHandle);
            #endif

            var localeOp = m_Settings.GetSelectedLocaleAsync();
            if (!localeOp.IsDone)
            {
                CurrentOperation = localeOp;
                localeOp.Completed += m_PreloadTablesAction;
            }
            else
            {
                PreloadTables();
            }
        }

        void PreloadTables()
        {
            m_RemainingSteps--;

            if (m_Settings.GetAssetDatabase() is IPreloadRequired assetOperation && !assetOperation.PreloadOperation.IsDone)
                m_LoadDatabasesOperations.Add(assetOperation.PreloadOperation);
            else
                m_RemainingSteps--;

            if (m_Settings.GetStringDatabase() is IPreloadRequired stringOperation && !stringOperation.PreloadOperation.IsDone)
                m_LoadDatabasesOperations.Add(stringOperation.PreloadOperation);
            else
                m_RemainingSteps--;

            if (m_LoadDatabasesOperations.Count > 0)
            {
                m_PreloadDatabasesOperation = AddressablesInterface.CreateGroupOperation(m_LoadDatabasesOperations);
                CurrentOperation = m_PreloadDatabasesOperation;
                m_PreloadDatabasesOperation.CompletedTypeless += m_FinishInitializingAction;
            }
            else
            {
                FinishInitializing(true, null);
            }
        }

        void PostInitializeExtensions()
        {
            foreach (var startupLocaleSelector in m_Settings.GetStartupLocaleSelectors())
            {
                (startupLocaleSelector as IInitialize)?.PostInitialization(m_Settings);
            }

            (m_Settings.GetAvailableLocales() as IInitialize)?.PostInitialization(m_Settings);
            (m_Settings.GetAssetDatabase() as IInitialize)?.PostInitialization(m_Settings);
            (m_Settings.GetStringDatabase() as IInitialize)?.PostInitialization(m_Settings);
        }

        void FinishInitializing(AsyncOperationHandle op) => FinishInitializing(op.Status == AsyncOperationStatus.Succeeded, op.OperationException?.Message);

        void FinishInitializing(bool success, string error)
        {
            AddressablesInterface.ReleaseAndReset(ref m_PreloadDatabasesOperation);
            PostInitializeExtensions();
            Complete(m_Settings, success, error);
        }

        protected override void Destroy()
        {
            base.Destroy();
            GenericPool<InitializationOperation>.Release(this);
        }
    }
}
