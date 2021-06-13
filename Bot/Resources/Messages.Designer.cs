﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Bot.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Messages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Messages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Bot.Resources.Messages", typeof(Messages).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Order date: *{0}*
        ///Order amount: *{1}* gr.
        ///Total amount taken from the current order: *{2}* gr.
        ///You owe: *{3}* UAH
        ///Your takeouts:
        ///{4}
        ///        .
        /// </summary>
        internal static string DefaultMessage {
            get {
                return ResourceManager.GetString("DefaultMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You don&apos;t have any expenses to remove.
        /// </summary>
        internal static string NoExpenses {
            get {
                return ResourceManager.GetString("NoExpenses", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You are not allowed to create new orders!.
        /// </summary>
        internal static string NotAuthorized {
            get {
                return ResourceManager.GetString("NotAuthorized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recorded.
        /// </summary>
        internal static string Recorded {
            get {
                return ResourceManager.GetString("Recorded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Recorded.
        /// </summary>
        internal static string Removed {
            get {
                return ResourceManager.GetString("Removed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Select order from the list below:.
        /// </summary>
        internal static string SelectOrder {
            get {
                return ResourceManager.GetString("SelectOrder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wrong data provided. Message template is: /neworder %amount% %price%.
        /// </summary>
        internal static string WrongNewOrder {
            get {
                return ResourceManager.GetString("WrongNewOrder", resourceCulture);
            }
        }
    }
}
