﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Apollo.Core.Base.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Apollo.Core.Base.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to A channel type must inherit from IChannelType..
        /// </summary>
        internal static string Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType {
            get {
                return ResourceManager.GetString("Exceptions_Messages_AChannelTypeMustDeriveFromIChannelType", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A machine needs at least one disk..
        /// </summary>
        internal static string Exceptions_Messages_AMachineNeedsAtLeastOneDisk {
            get {
                return ResourceManager.GetString("Exceptions_Messages_AMachineNeedsAtLeastOneDisk", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A machine needs at least one processor..
        /// </summary>
        internal static string Exceptions_Messages_AMachineNeedsAtLeastOneProcessor {
            get {
                return ResourceManager.GetString("Exceptions_Messages_AMachineNeedsAtLeastOneProcessor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A message must have an ID number. It cannot have the None ID as ID..
        /// </summary>
        internal static string Exceptions_Messages_AMessageNeedsToHaveAnId {
            get {
                return ResourceManager.GetString("Exceptions_Messages_AMessageNeedsToHaveAnId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value for a base line ID must not be smaller than that of the ID for an unknown value..
        /// </summary>
        internal static string Exceptions_Messages_BaseLineIdValueMustNotBeLessThanTheUnknownIdValue {
            get {
                return ResourceManager.GetString("Exceptions_Messages_BaseLineIdValueMustNotBeLessThanTheUnknownIdValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A dataset cannot be created without a creator..
        /// </summary>
        internal static string Exceptions_Messages_CannotCreateDatasetWithoutCreator {
            get {
                return ResourceManager.GetString("Exceptions_Messages_CannotCreateDatasetWithoutCreator", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given dataset cannot be made a parent..
        /// </summary>
        internal static string Exceptions_Messages_DatasetCannotBecomeParent {
            get {
                return ResourceManager.GetString("Exceptions_Messages_DatasetCannotBecomeParent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The given dataset with ID: {0} cannot be made a parent..
        /// </summary>
        internal static string Exceptions_Messages_DatasetCannotBecomeParent_WithId {
            get {
                return ResourceManager.GetString("Exceptions_Messages_DatasetCannotBecomeParent_WithId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The domain name of a machine must not be an empty string..
        /// </summary>
        internal static string Exceptions_Messages_MachineDomainNameMustNotBeEmpty {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MachineDomainNameMustNotBeEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The maximum of a dataset distribution range must be equal or larger than the minimum. The minimum was {0} and the maximum was {1}..
        /// </summary>
        internal static string Exceptions_Messages_MaximumDistributionRangeMustBeLargerThanMinimum_WithValues {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MaximumDistributionRangeMustBeLargerThanMinimum_WithValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The minimum of a dataset distribution range must be equal or larger than one. The given value is {0}..
        /// </summary>
        internal static string Exceptions_Messages_MimimumDistributionRangeMustBeLargerThanOne_WithValue {
            get {
                return ResourceManager.GetString("Exceptions_Messages_MimimumDistributionRangeMustBeLargerThanOne_WithValue", resourceCulture);
            }
        }
    }
}
