using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WASMAuthenticationDemo.Client.Helpers
{
    public static class IJSRuntimeExtensionMethods
    {
        public static async ValueTask WriteToConsole(this IJSRuntime js, string message)
        {
            await js.InvokeVoidAsync("console.log", message);
        }
    }
}
