﻿using System;
using JetBrains.ReSharper.Daemon.Stages.Dispatcher;
using JetBrains.ReSharper.Feature.Services.Daemon;
using JetBrains.ReSharper.Plugins.Unity.Feature.Services.Daemon;
using JetBrains.ReSharper.Psi.CSharp.Tree;
using JetBrains.ReSharper.Psi.Tree;

namespace JetBrains.ReSharper.Plugins.Unity.Daemon.Highlighting
{
    [ElementProblemAnalyzer(typeof(IMethodDeclaration), HighlightingTypes = new[] {typeof(UnityMarkOnGutter)})]
    public class UnityEventFunctionDetector : ElementProblemAnalyzer<IMethodDeclaration>
    {
        private readonly UnityApi myUnityApi;

        public UnityEventFunctionDetector(UnityApi unityApi)
        {
            myUnityApi = unityApi;
        }

        protected override void Run(IMethodDeclaration element, ElementProblemAnalyzerData data, IHighlightingConsumer consumer)
        {
            if (data.ProcessKind != DaemonProcessKind.VISIBLE_DOCUMENT)
                return;

            if (element.GetProject().IsUnityProject())
            {
                var method = element.DeclaredElement;
                if (method != null)
                {
                    var eventFunction = myUnityApi.GetUnityEventFunction(method);
                    if (eventFunction != null)
                    {
                        // Use the name as the range, rather than the range of the whole
                        // method declaration (including body). Rider will remove the highlight
                        // if anything inside the range changes, causing ugly flashes. It
                        // might be nicer to use the whole of the method declaration (name + params)
                        var documentRange = element.GetNameDocumentRange();
                        var tooltip = "Unity Event Function";
                        if (!string.IsNullOrEmpty(eventFunction.Description))
                            tooltip += Environment.NewLine + Environment.NewLine + eventFunction.Description;
                        var highlighting = new UnityMarkOnGutter(element, documentRange, tooltip);

                        consumer.AddHighlighting(highlighting, documentRange);
                    }
                }
            }
        }
    }
}