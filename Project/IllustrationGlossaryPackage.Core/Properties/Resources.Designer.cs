﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IllustrationGlossaryPackage.Core.Properties {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("IllustrationGlossaryPackage.Core.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Archive test package already exists in Archive directory. Please remove or rename: .
        /// </summary>
        internal static string ArchiveAlreadyExists {
            get {
                return ResourceManager.GetString("ArchiveAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Archive.
        /// </summary>
        internal static string ArchiveDir {
            get {
                return ResourceManager.GetString("ArchiveDir", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to _Archive.zip.
        /// </summary>
        internal static string ArchiveSuffix {
            get {
                return ResourceManager.GetString("ArchiveSuffix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .csv.
        /// </summary>
        internal static string CsvFileExtention {
            get {
                return ResourceManager.GetString("CsvFileExtention", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File {0} does not exist:.
        /// </summary>
        internal static string FileDNE {
            get {
                return ResourceManager.GetString("FileDNE", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Illustration Glossary Spreadsheet must be a csv.
        /// </summary>
        internal static string IllMustCsv {
            get {
                return ResourceManager.GetString("IllMustCsv", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Test package must be a zip file.
        /// </summary>
        internal static string TestMustZip {
            get {
                return ResourceManager.GetString("TestMustZip", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .zip.
        /// </summary>
        internal static string ZipFileExtention {
            get {
                return ResourceManager.GetString("ZipFileExtention", resourceCulture);
            }
        }
    }
}
