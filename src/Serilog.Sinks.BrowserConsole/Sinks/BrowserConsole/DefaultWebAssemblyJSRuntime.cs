// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
// Copyright 2020 Serilog Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop.Infrastructure;
using Microsoft.JSInterop.WebAssembly;

namespace Serilog.Sinks.BrowserConsole
{
    sealed class DefaultWebAssemblyJSRuntime : WebAssemblyJSRuntime
    {
        internal static readonly DefaultWebAssemblyJSRuntime Instance = new DefaultWebAssemblyJSRuntime();

        public ElementReferenceContext ElementReferenceContext { get; }

        DefaultWebAssemblyJSRuntime()
        {
            ElementReferenceContext = new WebElementReferenceContext(this);
            JsonSerializerOptions.Converters.Add(new ElementReferenceJsonConverter(ElementReferenceContext));
        }

        #pragma warning disable IDE0051 // Remove unused private members. Invoked via Mono's JS interop mechanism (invoke_method)
        static string InvokeDotNet(string assemblyName, string methodIdentifier, string dotNetObjectId, string argsJson)
        {
            var callInfo = new DotNetInvocationInfo(assemblyName, methodIdentifier, dotNetObjectId == null ? default : long.Parse(dotNetObjectId), callId: null);
            return DotNetDispatcher.Invoke(Instance, callInfo, argsJson);
        }

        // Invoked via Mono's JS interop mechanism (invoke_method)
        static void EndInvokeJS(string argsJson)
            => DotNetDispatcher.EndInvokeJS(Instance, argsJson);

        // Invoked via Mono's JS interop mechanism (invoke_method)
        static void BeginInvokeDotNet(string callId, string assemblyNameOrDotNetObjectId, string methodIdentifier, string argsJson)
        {
            // Figure out whether 'assemblyNameOrDotNetObjectId' is the assembly name or the instance ID
            // We only need one for any given call. This helps to work around the limitation that we can
            // only pass a maximum of 4 args in a call from JS to Mono WebAssembly.
            string assemblyName;
            long dotNetObjectId;
            if (char.IsDigit(assemblyNameOrDotNetObjectId[0]))
            {
                dotNetObjectId = long.Parse(assemblyNameOrDotNetObjectId);
                assemblyName = null;
            }
            else
            {
                dotNetObjectId = default;
                assemblyName = assemblyNameOrDotNetObjectId;
            }

            var callInfo = new DotNetInvocationInfo(assemblyName, methodIdentifier, dotNetObjectId, callId);
            DotNetDispatcher.BeginInvokeDotNet(Instance, callInfo, argsJson);
        }
        #pragma warning restore IDE0051
    }
}
